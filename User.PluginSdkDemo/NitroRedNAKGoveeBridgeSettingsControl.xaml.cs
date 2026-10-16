
using System.Windows;
using System.Windows.Controls;

namespace NitroRedNAK.GoveeBridge
{
    /// <summary>
    /// Settings page for the NitroRedNAK Govee Bridge.
    /// </summary>
    public partial class NitroRedNAKGoveeBridgeSettingsControl
        : UserControl
    {
        public NitroRedNAKGoveeBridgePlugin Plugin { get; }

        // Used by the WPF designer.
        public NitroRedNAKGoveeBridgeSettingsControl()
        {
            InitializeComponent();
        }

        // Used when SimHub opens the plugin settings.
        public NitroRedNAKGoveeBridgeSettingsControl(
            NitroRedNAKGoveeBridgePlugin plugin) : this()
        {
            Plugin = plugin;

            if (Plugin != null)
            {
                GoveeIpAddressTextBox.Text =
                    Plugin.CurrentGoveeIpAddress;
            }
        }


        private void SaveIpAddressButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (Plugin == null)
            {
                StatusTextBlock.Text =
                    "Plugin is not available. Open settings through SimHub.";
                return;
            }

            string message;
            bool saved = Plugin.SaveGoveeIpAddress(
                GoveeIpAddressTextBox.Text,
                out message);

            StatusTextBlock.Text = message;
        }
    }
}
