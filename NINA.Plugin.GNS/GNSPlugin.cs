#region "copyright"

/*
    Copyright © 2016 - 2021 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Plugin;
using NINA.Plugin.GNS.Sequencer.Utility;
using NINA.Plugin.Interfaces;
using NINA.Profile;
using NINA.Profile.Interfaces;

namespace NINA.Plugin.GNS {

    /// <summary>
    /// This class exports the IPlugin interface and will be used for the general plugin information and options
    /// An instance of this class will be created and set as datacontext on the plugin options tab in N.I.N.A. to be able to configure global plugin settings
    /// The user interface for the settings will be defined in the Options.xaml
    /// </summary>
    [Export(typeof(IPluginManifest))]
    public class GNSPlugin : PluginBase, INotifyPropertyChanged {
        private readonly IPluginOptionsAccessor pluginSettings;
        private readonly GNSMeridianFlipMonitor meridianFlipMonitor;

        [ImportingConstructor]
        public GNSPlugin(IProfileService profileService, ITelescopeMediator telescopeMediator) {
            this.pluginSettings = new PluginOptionsAccessor(profileService, Guid.Parse(this.Identifier));
            this.meridianFlipMonitor = new GNSMeridianFlipMonitor(profileService, telescopeMediator, () => MeridianFlipMessages, () => MeridianFlipTimeout);
        }

        public override Task Teardown() {
            meridianFlipMonitor.Teardown();
            return base.Teardown();
        }

        /// <summary>
        /// Send GNS messages when any meridian flip starts and finishes while a GNS session is active
        /// </summary>
        public bool MeridianFlipMessages {
            get => pluginSettings.GetValueBoolean(nameof(MeridianFlipMessages), true);
            set {
                pluginSettings.SetValueBoolean(nameof(MeridianFlipMessages), value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Extra time in seconds for the flip itself, on top of the wait for the meridian and the settle times
        /// </summary>
        public int MeridianFlipTimeout {
            get => pluginSettings.GetValueInt32(nameof(MeridianFlipTimeout), 900);
            set {
                pluginSettings.SetValueInt32(nameof(MeridianFlipTimeout), value);
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}