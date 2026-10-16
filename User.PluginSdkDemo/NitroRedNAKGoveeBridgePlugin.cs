using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace NitroRedNAK.GoveeBridge
{
    /// <summary>
    /// NitroRedNAK Govee Bridge.
    /// Routes live iRacing flag states to custom Govee lighting effects.
    /// </summary>
    [PluginDescription("Direct UDP racing-light effects for compatible Govee RGBIC bars.")]
    [PluginAuthor("NitroRedNAK")]
    [PluginName("NitroRedNAK Govee Bridge")]
    public class NitroRedNAKGoveeBridgePlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        // Prototype default. A future settings page will make this configurable.
        private volatile string GoveeIpAddress = "10.0.0.4";
        private static readonly string GoveeSettingsDirectory =
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NitroRedNAK",
        "GoveeBridge");

        private static readonly string GoveeSettingsFilePath =
            Path.Combine(GoveeSettingsDirectory, "settings.txt");

        public string CurrentGoveeIpAddress
        {
            get { return GoveeIpAddress; }
        }

        private volatile string requestedEffectName = null;

        private CancellationTokenSource effectControllerCancellation;
        private Task effectControllerTask;

        public PluginManager PluginManager { get; set; }

        public ImageSource PictureIcon => null;

        public string LeftMenuTitle => "NitroRedNAK Govee Bridge";

        /// <summary>
        /// Starts the background effect controller.
        /// </summary>
        // Provides the settings page shown when the user opens this plugin.
        public System.Windows.Controls.Control GetWPFSettingsControl(
            PluginManager pluginManager)
        {
            return new NitroRedNAKGoveeBridgeSettingsControl(this);
        }
        public void Init(PluginManager pluginManager)
        {
            PluginManager = pluginManager;
            LoadGoveeIpAddress();

            SimHub.Logging.Current.Info(
                "Starting NitroRedNAK Govee Bridge.");

            effectControllerCancellation =
                new CancellationTokenSource();

            effectControllerTask = Task.Run(
                () => RunFlagEffectControllerAsync(
                    effectControllerCancellation.Token));
        }

        private void LoadGoveeIpAddress()
        {
            try
            {
                if (!File.Exists(GoveeSettingsFilePath))
                    return;

                string savedAddress = File.ReadAllText(
                    GoveeSettingsFilePath).Trim();

                IPAddress parsedAddress;

                if (IPAddress.TryParse(savedAddress, out parsedAddress) &&
                    parsedAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    GoveeIpAddress = parsedAddress.ToString();
                }
            }
            catch
            {
                // Keep the default address if the settings file cannot be read.
            }
        }

        public bool SaveGoveeIpAddress(string address, out string message)
        {
            address = (address ?? string.Empty).Trim();

            IPAddress parsedAddress;

            if (!IPAddress.TryParse(address, out parsedAddress) ||
                parsedAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                message = "Enter a valid IPv4 address, such as 192.168.1.50.";
                return false;
            }

            try
            {
                string validatedAddress = parsedAddress.ToString();

                Directory.CreateDirectory(GoveeSettingsDirectory);
                File.WriteAllText(
                    GoveeSettingsFilePath,
                    validatedAddress);

                GoveeIpAddress = validatedAddress;

                message = "IP address saved. The next flag effect will use it.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not save the IP address: " + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// Reads live race flags and requests the appropriate custom effect.
        /// Checkered has priority over yellow. Green stops our custom effects.
        /// </summary>
        public void DataUpdate(
            PluginManager pluginManager,
            ref GameData data)
        {
            string effectToRun = null;

            if (data.GameRunning && data.NewData != null)
            {
                int yellowFlag = data.NewData.Flag_Yellow;
                int greenFlag = data.NewData.Flag_Green;
                int checkeredFlag = data.NewData.Flag_Checkered;

                if (checkeredFlag != 0)
                {
                    effectToRun = "checkered";
                }
                else if (yellowFlag != 0 && greenFlag == 0)
                {
                    effectToRun = "yellow";
                }
            }

            requestedEffectName = effectToRun;
        }

        /// <summary>
        /// Runs one effect. Its cleanup completes before another starts.
        /// </summary>
        private async Task RunSingleFlagEffectAsync(
            string effectName,
            CancellationToken cancellationToken)
        {
            try
            {
                using (var sender = new GoveeUdpSender(GoveeIpAddress))
                {
                    if (effectName == "yellow")
                    {
                        await sender.RunAlternatingFlashingYellowAsync(
                            cancellationToken);
                    }
                    else if (effectName == "checkered")
                    {
                        await sender.RunAlternatingCheckeredChaseAsync(
                            cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when a flag changes or the plugin stops.
            }
            catch (Exception ex)
            {
                SimHub.Logging.Current.Info(
                    "Govee flag effect failed: " + ex);
            }
        }

        /// <summary>
        /// Switches effects without letting two routines control the lights
        /// simultaneously.
        /// </summary>
        private async Task RunFlagEffectControllerAsync(
            CancellationToken controllerToken)
        {
            string activeEffect = null;
            CancellationTokenSource activeCancellation = null;
            Task activeTask = null;

            try
            {
                while (true)
                {
                    controllerToken.ThrowIfCancellationRequested();

                    string requested = requestedEffectName;

                    if (!string.Equals(
                        requested, activeEffect,
                        StringComparison.Ordinal))
                    {
                        // Finish the previous effect and its cleanup first.
                        if (activeCancellation != null)
                        {
                            activeCancellation.Cancel();

                            if (activeTask != null)
                            {
                                await activeTask;
                            }

                            activeCancellation.Dispose();
                            activeCancellation = null;
                            activeTask = null;
                        }

                        activeEffect = null;

                        // Read the latest request after waiting for cleanup.
                        requested = requestedEffectName;

                        if (requested == "yellow" ||
                            requested == "checkered")
                        {
                            activeCancellation =
                                CancellationTokenSource
                                    .CreateLinkedTokenSource(
                                        controllerToken);

                            activeEffect = requested;

                            activeTask = RunSingleFlagEffectAsync(
                                requested,
                                activeCancellation.Token);
                        }
                    }

                    await Task.Delay(50, controllerToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the plugin shuts down.
            }
            finally
            {
                if (activeCancellation != null)
                {
                    activeCancellation.Cancel();

                    if (activeTask != null)
                    {
                        await activeTask;
                    }

                    activeCancellation.Dispose();
                }
            }
        }

        /// <summary>
        /// Stops active effects and releases controller resources.
        /// </summary>
        public void End(PluginManager pluginManager)
        {
            requestedEffectName = null;

            if (effectControllerCancellation != null)
            {
                effectControllerCancellation.Cancel();

                try
                {
                    if (effectControllerTask != null &&
                        !effectControllerTask.Wait(3000))
                    {
                        SimHub.Logging.Current.Info(
                            "Govee effect controller did not stop within 3 seconds.");
                    }
                }
                catch (AggregateException ex)
                {
                    SimHub.Logging.Current.Info(
                        "Error stopping Govee effect controller: " + ex);
                }

                effectControllerCancellation.Dispose();
                effectControllerCancellation = null;
                effectControllerTask = null;
            }

            SimHub.Logging.Current.Info(
                "Stopped NitroRedNAK Govee Bridge.");
        }
    }
}
