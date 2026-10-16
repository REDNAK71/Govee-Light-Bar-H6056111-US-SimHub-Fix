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

        // Used when SimHub opens our plugin settings.
        public NitroRedNAKGoveeBridgeSettingsControl(
            NitroRedNAKGoveeBridgePlugin plugin) : this()
        {
            Plugin = plugin;
        }
    }
}
