# Govee-Light-Bar-H6056111-US-SimHub-Fix

Govee H6056111-US SimHub Lag Fix
NitroRedNAK Govee Bridge is an experimental community project investigating and working around delayed lighting updates observed with the Govee H6056 / H6056111-US RGBIC TV Light Bars, tested with firmware version 1.00.03 as shown in the Govee mobile app.
Created by REDNAK71 with AI-assisted development (“Nitro”). The goal is to help sim racers get responsive, race-telemetry-driven lighting without multi-second delays.
Status: working proof of concept, not a universal fix or official Govee/SimHub patch. Testing so far has been on one local setup with the hardware and firmware listed above. Other firmware versions, hardware revisions, network setups, and SimHub versions still need testing.

The problem we observed
On the developer's setup, SimHub's normal Govee Ambient Lights output developed severe delays after a Windows reinstall/update and reconfiguration. Flag effects that had previously responded quickly could take several seconds to reach the light bars, making rapid flashing patterns unusable.
The following comparisons helped narrow the problem:
- The Govee mobile app could control the lights promptly.
- A direct local UDP color command to the light controller changed the lights promptly.
- A Python proof-of-concept that sent the controller's local Razer-compatible razer command format could rapidly alternate all six RGB zones.
- The same fast command method was then implemented in a custom SimHub plugin, where the manual test action successfully drove the six zones.
These tests show that responsive local control is possible on the tested setup. They do not prove that firmware 1.00.03 itself is the root cause, nor that every H6056111-US controller behaves identically.
What this project does
Rather than patching SimHub's built-in Govee driver, the prototype sends RGB color frames directly to the light controller over the local network. It uses UDP on port 4003 and wraps the controller payload in the local Razer-compatible command format tested on the developer's device.
The six RGB zones are addressed as one frame:
- Zones 1–3: left light bar
- Zones 4–6: right light bar
The current project separates effect behavior from packet sending so more patterns and settings can be added without rewriting the network code.
Effects demonstrated on the developer's setup
- AlternatingFlashingYellow — alternates the left group (zones 1–3) and right group (zones 4–6) during yellow caution. The intended rule is to stop yellow when yellow clears or checkered takes priority.
- AlternatingCheckeredChase — a custom white–black–white pattern sliding across the six zones, starting at 150 ms per step.
- Green and white effects — these were already configured in SimHub Ambient Lights on the developer's setup; they are not presented here as custom effects created by this plugin.
- Cleanup — custom effect routines send an all-black frame when cancelled, so the last custom frame is not intentionally left illuminated.
During replay testing, the developer observed green, yellow caution, white, and the custom checkered behavior working with the bridge installed. The tested setup also required the Ambient Lights master switch to be enabled for the successful manual test. The reason for that dependency has not yet been isolated; keep Ambient Lights enabled when reproducing the current setup.
Tested setup
- Light bars: Govee H6056 RGBIC TV Light Bars, hardware label/revision H6056111-US
- Firmware shown in the Govee app: 1.00.03
- Operating system: Windows 11
- Simulator integration: SimHub with iRacing replay testing
- Controller local UDP port: 4003
- Build target: .NET Framework 4.8
The controller IP used during development was a private LAN address. It is intentionally not treated as a universal value; other users must configure their own controller address before building/running the prototype.
Current limitations and cautions
- The controller IP is currently hard-coded in source. A user-configurable IP/port field has not yet been completed.
- This project is a prototype, not a packaged installer. Installation and troubleshooting still require manual steps.
- The current code began from SimHub's User.PluginSdkDemo; some demo UI, names, and unrelated demo functionality remain and should be cleaned up before a wider release.
- The effect patterns, flag transitions, cancellation, and interaction with Ambient Lights need further tests across more sessions and hardware configurations.
- The local command format was validated by testing on one controller. It is not claimed to be an officially supported Govee API.
- Back up SimHub configuration and the existing plugin DLL before testing. Test in replay first, not during a race you cannot safely interrupt.
- Do not replace SimHub's own DLLs. Install only the uniquely named NitroRedNAK.GoveeBridge.dll built from this project.
Build from source
1. Install SimHub and its Plugin SDK.
2. Install Visual Studio with the .NET desktop development workload and .NET Framework 4.8 targeting support.
3. Clone or download this repository and open the solution in Visual Studio.
4. Review the project references and build configuration for your SimHub installation; the project began as a copy of SimHub's SDK demo, so local paths may need adjustment.
5. Set the controller's local IP address in the source to the address assigned by your own network.
6. Build the solution and check the build output.
7. Back up your current configuration, install the resulting uniquely named DLL into the SimHub installation folder, restart SimHub, and test in a replay.
8. Verify that custom yellow stops on green/checkered, the checkered chase takes priority, and no effects continue after the game or plugin stops.
See the official SimHub plugin and extension SDK guide. SimHub notes that undocumented APIs or internal behavior may change with updates.
Roadmap
- Add settings for controller IP/port and zone mapping.
- Make pattern speed, color, and style configurable in SimHub.
- Complete consistent priority/cleanup handling for all supported race flags.
- Replace demo-only UI and remove unused SDK-demo content where appropriate.
- Test additional H6056111-US devices, firmware versions, network conditions, and SimHub versions.
- Publish a versioned release package only after broader testing and a licensing review.
Contributions and bug reports
Feedback and testing from other sim racers are welcome. When reporting an issue, include:
- Exact Govee model/hardware label and firmware version shown in the app
- SimHub version and Windows version
- Whether LAN control is enabled
- Whether the Govee app and the plugin's manual test action respond promptly
- The effect/flag being tested and what the lights actually did
Review logs and packet captures before sharing them; remove local IP addresses and any unrelated personal information.
Project relationship and licensing
This is an independent community project and is not affiliated with or endorsed by Govee or SimHub. Govee and SimHub are trademarks of their respective owners. The project uses SimHub plugin SDK APIs and began from an SDK demo. A project license has not yet been assigned; the source and included assets should be reviewed before choosing a license or redistributing third-party SDK material.
