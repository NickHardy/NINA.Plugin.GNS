# N.I.N.A. - Nighttime Imaging 'N' Astronomy GNS Plugin#

This repository contains the source code distribution of the N.I.N.A. imaging software GNS Plugin.

The good night system by LunaticoAstro is a monitoring system that let's you sleep at night.

https://lunaticoastro.com/gns-observatory-monitoring/

https://nighttime-imaging.eu/


# Plugin Information: #

Just check for the available plugins and select the GNS plugin. Click the install button and restart Nina.

This can only be used in the Advanced Sequencer in Nina 2.0 or later.

Version 3.0 and later of this plugin require Nina 3.3 or later. Nina 3.2 keeps offering version 2.1.0.5, which is maintained on the nina-3.2 branch.

Please report any issues in the Nina discord server.
https://discord.gg/rWRbVbw and tag me: @NickHolland#5257 

! These instructions will not pause the sequencer. Timeouts are only for the GNS server. When a timeout is reached GNS will trigger an alarm.

Instructions:
GNS Message - Send a message to the GNS server. Enter your own message and timeout in seconds. The message can contain expressions in braces, like {TargetName}, which are evaluated when the message is sent. Hover over the message field to see the result.
GNS Pause session - Pause the GNS session. (For instance if you run a script and don't know how long it could take) The GNS trigger and the meridian flip messages stay quiet until a GNS Message resumes the session.
GNS Alert session - Send a session Alert. The GNS server will send an alert to your phone. You can enter your own message, which can contain expressions just like GNS Message. Usually followed by an alert box in Nina or a script.
GNS End session - End the current GNS session. Nothing is sent after this, so instructions that follow, like parking the mount, won't start a new session. The next sequence run starts fresh.

Triggers:
GNS trigger - This will trigger for every instruction within the instruction set and send a message based on the estimated runtime of the instruction plus its timeout (5 minutes by default). An optional prefix, for example {TargetName}, is evaluated for every message and put in front of the instruction name.
GNS MeridianFlip trigger (legacy) - This basically calls the MeridianFlip trigger within Nina and does the same. No longer needed, it is only kept so existing sequences keep working.

Meridian flips:
You can use any meridian flip trigger, like the one in Nina or the new programmable meridian flip trigger. While a GNS session is active, the plugin sends a message when the flip starts, with a timeout covering the wait for the meridian, the flip and the settle times plus the meridian flip timeout from the plugin options (default 15 minutes). When the flip finishes it sends another message, and a failed flip sends a session alert. This can be switched off in the plugin options.
With the programmable meridian flip trigger the before flip actions and the wait for the flip window run before the flip starts. Add a GNS Message instruction to the before flip actions if they can take longer than the current GNS timeout.
