using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// The name of your plugin
[assembly: AssemblyTitle("GNS Plugin")]
// A short description of your plugin
[assembly: AssemblyDescription("A plugin for using GoodNightSystem by Lunatico")]
[assembly: AssemblyConfiguration("")]

//Your name
[assembly: AssemblyCompany("Nick Hardy")]
//The product name that this plugin is part of
[assembly: AssemblyProduct("NINA.Plugin.GNS")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6d0e03f2-8743-4229-bf2c-f451e23f977a")]

//The assembly versioning
//Should be incremented for each new release build of a plugin
[assembly: AssemblyVersion("1.1.0.1")]
[assembly: AssemblyFileVersion("1.1.0.1")]

//The minimum Version of N.I.N.A. that this plugin is compatible with
[assembly: AssemblyMetadata("MinimumApplicationVersion", "2.0.0.2001")]

//Your plugin homepage - omit if not applicaple
[assembly: AssemblyMetadata("Homepage", "https://lunaticoastro.com")]
//The license your plugin code is using
[assembly: AssemblyMetadata("License", "MPL-2.0")]
//The url to the license
[assembly: AssemblyMetadata("LicenseURL", "https://www.mozilla.org/en-US/MPL/2.0/")]
//The repository where your pluggin is hosted
[assembly: AssemblyMetadata("Repository", "https://bitbucket.org/NickHardy/nina.plugin.gns/src/main/")]

//Common tags that quickly describe your plugin
[assembly: AssemblyMetadata("Tags", "GNS,Sequencer")]

//The featured logo that will be displayed in the plugin list next to the name
[assembly: AssemblyMetadata("FeaturedImageURL", "https://bitbucket.org/NickHardy/nina.plugin.gns/downloads/gns-ancho-min-300x300.png")]
//An example screenshot of your plugin in action
[assembly: AssemblyMetadata("ScreenshotURL", "https://bitbucket.org/NickHardy/nina.plugin.gns/downloads/GNS-Plugin-1.jpg")]
//An additional example screenshot of your plugin in action
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
[assembly: AssemblyMetadata("LongDescription", @"N.I.N.A. - Nighttime Imaging 'N' Astronomy GNS Plugin

The good night system by LunaticoAstro is a monitoring system that let's you sleep at night.

https://lunaticoastro.com/gns-observatory-monitoring/

! These instructions will not pause the sequencer. Timeouts are only for the GNS server. When a timeout is reached GNS will trigger an alarm.

Triggers:
GNS trigger - This will trigger for every instruction within the instruction set and send a message based on the estimated runtime of the instruction plus 5 minutes.
GNS MeridianFlip trigger - This basically calls the MeridianFlip trigger within Nina and does the same. Timeout is based on the estimated runtime for the flip plus 15 minutes.

Instructions:
GNS Message - Send a message to the GNS server. Enter your own message and timeout in seconds.
GNS Pause session - Pause the GNS session. (For instance if you run a script and don't know how long it could take)
GNS Alert session - Send a session Alert. The GNS server will send an alert to your phone. You can enter your own message. Usually followed by an alert box in Nina or a script.
GNS End session - End the current GNS session.

Please report any issues in the [Nina discord server](https://discord.gg/rWRbVbw) and tag me: @NickHolland#5257 

If you would like to buy me a whisky: [click here](https://www.paypal.com/paypalme/NickHardyHolland)
")] 