# NitroRedNAK Govee Bridge — Source Code

This folder is being prepared for a clean, standalone source
project for the NitroRedNAK Govee Bridge SimHub plugin.

## Project goals

- Investigate multi-second lighting delays affecting Govee
  H6056 / H6056111-US RGBIC TV Light Bars.
- Send rapid local UDP lighting commands to compatible Govee
  hardware.
- Connect custom lighting effects to live iRacing race flags.
- Support configurable effects so racers can choose their
  preferred colors, patterns, and timing.

## Tested prototype

The prototype was tested with an H6056-series light-bar setup
whose Govee app reported firmware 1.00.03.

The working test demonstrated rapid control of six LED zones.
Green, yellow caution, white, and the custom checkered effect
were also observed working in SimHub replay testing.

These results describe the tested setup and are not a guarantee
of compatibility with every hardware or firmware revision.

## Development status

The prototype works on the developer's setup, but the source
project is still being organized for community use.

Before publishing a complete buildable source release, we will
separate our implementation from SDK demonstration code, review
applicable attribution and redistribution terms, and document
the build and installation process.

This is a development project, not yet a general-purpose release.
