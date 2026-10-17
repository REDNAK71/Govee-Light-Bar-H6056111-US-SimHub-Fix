# NitroRedNAK Govee Bridge for SimHub

## Fix delayed Govee H6056/H6056111-US racing-light effects in SimHub

This plugin was created to provide fast, direct Govee light-bar control for the newer Govee H6056 hardware revisions that can experience severe delays when controlled through SimHub's normal Govee communication path.

The plugin is designed primarily for racing effects such as:

- Green flag
- Yellow/caution
- Checkered flag
- Other direct racing-light effects

The plugin communicates directly with the Govee light bars using their local IP address instead of relying on the problematic device-discovery behavior that can cause several-second delays.

### The result

Flag effects can respond essentially immediately instead of taking several seconds to reach the lights.

---

# Installation

## Recommended: Windows Installer

Download the latest **NitroRedNAK Govee Bridge Installer** from the GitHub Releases page.

Run the installer and follow the prompts.

The installer will:

1. Locate your SimHub installation.
2. Allow you to select a different SimHub installation folder if necessary.
3. Verify that the selected folder contains `SimHubWPF.exe`.
4. Install the NitroRedNAK Govee Bridge plugin.
5. Detect an existing NitroRedNAK installation when upgrading.

The installer does **not** replace SimHub's built-in Govee components.

Our plugin is installed as its own file:

`NitroRedNAK.GoveeBridge.dll`

### After installation

1. Start or restart SimHub.
2. Open the SimHub Plugins section.
3. Find **NitroRedNAK Govee Bridge**.
4. Activate the plugin.
5. Open the plugin settings.

---

# Configure Your Govee IP Address

The plugin uses the local IPv4 address of your Govee light bar/controller.

Open the **NitroRedNAK Govee Bridge** settings in SimHub.

Enter the IPv4 address assigned to your Govee device.

Example:

`10.0.0.4`

Then click:

**SAVE IP ADDRESS**

The plugin validates the address and saves it to your Windows user profile.

The saved address is automatically loaded when SimHub starts.

### Important

Your Govee device may have a different IP address than the example above.

Use the actual local IP address assigned to your Govee device.

If your router uses DHCP, consider creating a DHCP reservation for the Govee device so its IP address does not change.

---

# Using the Plugin

Once the plugin is installed, activated, and configured, it can provide direct racing-light effects through the Govee connection.

The current custom racing effects include:

### Green Flag

Provides the custom green-flag behavior used by the NitroRedNAK racing-light setup.

### Yellow / Caution

Provides the custom rapid yellow caution-light behavior.

### Checkered Flag

Provides the custom checkered-flag effect used by the NitroRedNAK racing-light setup.

The plugin is designed so that the Govee light bars return to their normal state when the racing condition ends.

---

# Troubleshooting

## The plugin does not appear in SimHub

Make sure:

- SimHub is completely restarted after installation.
- `NitroRedNAK.GoveeBridge.dll` is located in the SimHub installation folder.
- The plugin has been activated in SimHub.

## The lights do not respond

Check:

1. The Govee light bars are powered on.
2. The computer and Govee device are connected to the same local network.
3. The Govee device has LAN control enabled.
4. The IP address entered in the plugin is correct.
5. The IP address was saved successfully.
6. SimHub has been restarted after installation if necessary.

## The IP address changed

Open the NitroRedNAK Govee Bridge settings and enter the new IPv4 address.

For a more permanent solution, create a DHCP reservation for the Govee device in your router.

## The lights respond slowly

Make sure the plugin is active and that the correct Govee IP address is configured.

The purpose of this plugin is to provide a direct communication path to the Govee hardware rather than relying on the device-discovery behavior that can cause significant delays with affected hardware revisions.

---

# Supported Hardware

This project was developed specifically to address communication problems encountered with newer revisions of the Govee H6056 light bars, including hardware identified as:

`H6056111-US`

Behavior can vary between Govee hardware revisions and firmware versions.

---

# Technical Information

## Why was this plugin created?

Some newer Govee H6056 hardware revisions can report their network information incorrectly during local device discovery.

When SimHub cannot reliably determine the device's IP address, communication with the light bars can become extremely slow or unreliable.

In testing, normal Govee control could experience delays of several seconds.

That is unacceptable for racing effects.

A yellow flag that appears three seconds after the caution is already happening isn't much use to a driver. 

## The NitroRedNAK approach

Instead of depending on automatic device discovery, the plugin allows the user to provide the Govee device's IPv4 address directly.

The plugin then communicates with the Govee device using its local network interface.

Conceptually:

    iRacing
       |
       v
    SimHub telemetry
       |
       v
    NitroRedNAK Govee Bridge
       |
       v
    Direct Govee UDP communication
       |
       v
    Govee light bars

This removes the problematic discovery step from the direct racing-effect path.

## Saved Settings

The configured Govee IP address is stored in the Windows user profile and loaded automatically when the plugin starts.

This means users do not need to edit the source code to change their Govee IP address.

---

# Open Source

The complete project source is available in this repository.

If you are interested in how the plugin works, you can:

- Inspect the source code.
- Build the plugin yourself.
- Modify the plugin.
- Experiment with additional effects.
- Submit improvements or fixes.

The prebuilt installer is intended for normal SimHub users who simply want the fix without installing development tools.

If you're downloading the source code and opening the Visual Studio project, you're officially past "Joe Racer."

You're a digital mechanic now. 😎

---

# Development

The plugin is developed using the SimHub Plugin SDK.

The source project is located under:

`User.PluginSdkDemo`

The compiled plugin is:

`NitroRedNAK.GoveeBridge.dll`

Developers can build the project themselves using the included Visual Studio solution.

---

# Future Development

Potential future work includes investigating whether SimHub's existing Ambient Lights processing can be redirected through the NitroRedNAK direct Govee communication path.

The goal would be to preserve SimHub's existing ambient-light processing while eliminating the communication delay affecting supported Govee hardware.

This is experimental and is not required for the current racing-flag functionality.

---

# Credits

Created by **RedNAK / NitroRedNAK**

Built for dirt-track racing and SimHub users dealing with delayed Govee H6056/H6056111-US communication.

If this project helps your racing setup, enjoy it — and go turn some laps. 🏁

---

## License

See the repository license file for the terms under which this project may be used, modified, and distributed.
