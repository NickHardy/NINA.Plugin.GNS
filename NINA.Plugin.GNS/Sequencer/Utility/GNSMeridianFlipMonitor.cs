#region "copyright"

/*
    Copyright © 2016 - 2021 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using NINA.Core.Utility;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Profile.Interfaces;
using System;
using System.Threading.Tasks;

namespace NINA.Plugin.GNS.Sequencer.Utility {

    /// <summary>
    /// Listens to the meridian flip events N.I.N.A. raises for every flip (classic, programmable or plugin based triggers)
    /// and keeps the GNS timeout in line with the flip, so any meridian flip trigger can be used together with GNS.
    /// Messages are only sent while a GNS session is active.
    /// </summary>
    public class GNSMeridianFlipMonitor {
        private readonly IProfileService profileService;
        private readonly ITelescopeMediator telescopeMediator;
        private readonly Func<bool> isEnabled;
        private readonly Func<int> flipTimeout;
        private readonly GNSUtility GNSUtil = new GNSUtility();

        public GNSMeridianFlipMonitor(IProfileService profileService, ITelescopeMediator telescopeMediator, Func<bool> isEnabled, Func<int> flipTimeout) {
            this.profileService = profileService;
            this.telescopeMediator = telescopeMediator;
            this.isEnabled = isEnabled;
            this.flipTimeout = flipTimeout;
            telescopeMediator.BeforeMeridianFlip += BeforeMeridianFlip;
            telescopeMediator.AfterMeridianFlip += AfterMeridianFlip;
        }

        public void Teardown() {
            telescopeMediator.BeforeMeridianFlip -= BeforeMeridianFlip;
            telescopeMediator.AfterMeridianFlip -= AfterMeridianFlip;
        }

        private bool ShouldSend() {
            return isEnabled() && GNSUtility.SessionActive;
        }

        private Task BeforeMeridianFlip(object sender, BeforeMeridianFlipEventArgs e) {
            if (!ShouldSend()) return Task.CompletedTask;
            try {
                var timeout = Convert.ToInt32(CalculateWaitTime().TotalSeconds) + profileService.ActiveProfile.MeridianFlipSettings.SettleTime
                    + profileService.ActiveProfile.GuiderSettings.SettleTimeout + flipTimeout();
                Logger.Info($"GNS: Meridian flip started. Sending timeout of {timeout} seconds");
                GNSUtil.sendMessage(timeout, "Meridian flip started.", false);
            } catch (Exception ex) {
                Logger.Error("GNS: Failed to send meridian flip start message", ex);
            }
            return Task.CompletedTask;
        }

        private Task AfterMeridianFlip(object sender, AfterMeridianFlipEventArgs e) {
            if (!ShouldSend()) return Task.CompletedTask;
            try {
                if (e.Success) {
                    // Cover the actions that may still follow the flip, but never shorten the timeout the sequence expected before the flip
                    var timeout = Math.Max(flipTimeout(), GNSUtility.LastTimeout);
                    Logger.Info($"GNS: Meridian flip finished. Sending timeout of {timeout} seconds");
                    GNSUtil.sendMessage(timeout, "Meridian flip finished.", false);
                } else {
                    Logger.Info("GNS: Meridian flip failed. Sending session alert");
                    GNSUtil.sendMessage(0, "Session Alert: Meridian flip failed.", false);
                }
            } catch (Exception ex) {
                Logger.Error("GNS: Failed to send meridian flip end message", ex);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// The classic meridian flip raises its start event before waiting for the meridian to pass, so the wait is part of the timeout.
        /// Same calculation as N.I.N.A.'s meridian flip trigger: a remaining time of more than two hours means the flip is late and happens immediately.
        /// </summary>
        private TimeSpan CalculateWaitTime() {
            var telescopeInfo = telescopeMediator.GetInfo();
            if (!telescopeInfo.Connected || double.IsNaN(telescopeInfo.TimeToMeridianFlip)) return TimeSpan.Zero;

            var settings = profileService.ActiveProfile.MeridianFlipSettings;
            var delta = settings.MaxMinutesAfterMeridian - settings.MinutesAfterMeridian;
            var time = TimeSpan.FromHours(telescopeInfo.TimeToMeridianFlip) - TimeSpan.FromMinutes(delta);
            if (time < TimeSpan.Zero || time > TimeSpan.FromHours(2)) {
                time = TimeSpan.Zero;
            }
            return time;
        }
    }
}
