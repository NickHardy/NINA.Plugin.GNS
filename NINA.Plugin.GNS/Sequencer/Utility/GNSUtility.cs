#region "copyright"

/*
    Copyright © 2016 - 2020 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using NINA.Core.Utility;
using NINA.Sequencer.Interfaces.Mediator;
using NINA.Sequencer.Logic;
using NINA.Sequencer.SequenceItem;
using System;
using System.Threading.Tasks;

namespace NINA.Plugin.GNS.Sequencer.Utility {

    public enum GNSSessionState {
        None,
        Active,
        Paused,
        Ended
    }

    public class GNSUtility {
        private dynamic GNSService;

        public void InitializeService() {
            this.GNSService = null;
            const string progID = "GNS.OWL";
            Type OwlType = Type.GetTypeFromProgID(progID);

            if (OwlType != null)
                this.GNSService = Activator.CreateInstance(OwlType);
        }

        public dynamic GetGNSService() {
            if (this.GNSService == null) this.InitializeService();
            return this.GNSService;
        }

        // A timeout of -1 pauses and -2 ends the GNS session, any other value keeps it running
        public static GNSSessionState SessionState { get; private set; } = GNSSessionState.None;

        public static bool SessionActive => SessionState == GNSSessionState.Active;

        // Paused or ended explicitly with a GNS Pause or End instruction. Only an explicit GNS message resumes it
        public static bool SessionStopped => SessionState == GNSSessionState.Paused || SessionState == GNSSessionState.Ended;

        // Called when a new sequence starts, so a session ended by a previous run doesn't block the triggers
        public static void ResetSession() {
            if (SessionStopped) SessionState = GNSSessionState.None;
        }

        private static readonly object hookLock = new object();
        private static ISequenceMediator hookedSequenceMediator;

        /// <summary>
        /// Resets the session state whenever a sequence starts. The sequencer isn't available yet while plugins load,
        /// so this is hooked lazily from a running sequence instead of from the plugin constructor
        /// </summary>
        public static void HookSequenceStart(ISequenceMediator sequenceMediator) {
            if (sequenceMediator == null) return;
            lock (hookLock) {
                if (hookedSequenceMediator != null || !sequenceMediator.Initialized) return;
                sequenceMediator.SequenceStarting += (sender, e) => {
                    ResetSession();
                    return Task.CompletedTask;
                };
                hookedSequenceMediator = sequenceMediator;
            }
        }

        // Timeout of the last regular message, so it can be restored after an interruption like a meridian flip
        public static int LastTimeout { get; private set; }

        // Maximum length of the user text in a message, as the message field allowed before expressions were supported
        public const int MaxTextLength = 65;

        /// <summary>
        /// Evaluates N.I.N.A. expressions in braces, like {TargetName}, in the context of the given sequence item
        /// </summary>
        public static string ExpandText(string text, ISymbolBroker symbolBroker, ISequenceItem context) {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string expanded;
            try {
                expanded = ExpressionExpander.Expand(text, symbolBroker, context) ?? string.Empty;
            } catch (Exception ex) {
                Logger.Error($"GNS: Failed to evaluate expressions in \"{text}\"", ex);
                expanded = text.Trim();
            }
            return expanded.Length > MaxTextLength ? expanded.Substring(0, MaxTextLength) : expanded;
        }

        public void sendMessage(int timeout, string message) {
            sendMessage(timeout, message, true);
        }

        public void sendMessage(int timeout, string message, bool rememberTimeout) {
            try {
                this.GNSService.NewMsg = message;
                this.GNSService.NewTimeout = timeout;
            } catch (Exception e) {
                this.InitializeService();
                this.GNSService.NewMsg = message;
                this.GNSService.NewTimeout = timeout;
            }
            SessionState = timeout == -2 ? GNSSessionState.Ended : timeout == -1 ? GNSSessionState.Paused : GNSSessionState.Active;
            if (rememberTimeout) LastTimeout = Math.Max(timeout, 0);
        }
    }
}