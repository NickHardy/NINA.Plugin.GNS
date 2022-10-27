#region "copyright"

/*
    Copyright © 2016 - 2020 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using System;

namespace NINA.Plugin.GNS.Sequencer.Utility {

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

        public void sendMessage(int timeout, string message) {
            try {
                this.GNSService.NewMsg = message;
                this.GNSService.NewTimeout = timeout;
            } catch (Exception e) {
                this.InitializeService();
                this.GNSService.NewMsg = message;
                this.GNSService.NewTimeout = timeout;
            }
        }
    }
}