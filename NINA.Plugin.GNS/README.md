# N.I.N.A. - Nighttime Imaging 'N' Astronomy GNS Plugin#

This repository contains the source code distribution of the N.I.N.A. imaging software GNS Plugin.

The good night system by LunaticoAstro is a monitoring system that let's you sleep at night.

https://lunaticoastro.com/gns-observatory-monitoring/

https://nighttime-imaging.eu/


# Plugin Information: #

Just check for the available plugins and select the GNS plugin. Click the install button and restart Nina.

This can only be used in the Advanced Sequencer in Nina 2.0 or later.

Please report any issues in the Nina discord server.
https://discord.gg/rWRbVbw and tag me: @NickHolland#5257 

! These instructions will not pause the sequencer. Timeouts are only for the GNS server. When a timeout is reached GNS will trigger an alarm.

Instructions:
GNS Message - Send a message to the GNS server. Enter your own message and timeout in seconds.
GNS Pause session - Pause the GNS session. (For instance if you run a script and don't know how long it could take)
GNS Alert session - Send a session Alert. The GNS server will send an alert to your phone. You can enter your own message. Usually followed by an alert box in Nina or a script.
GNS End session - End the current GNS session.

Triggers:
GNS trigger - This will trigger for every instruction within the instruction set and send a message based on the estimated runtime of the instruction plus 5 minutes.
GNS MeridianFlip trigger - This basically calls the MeridianFlip trigger within Nina and does the same. Timeout is based on the estimated runtime for the flip plus 15 minutes.
