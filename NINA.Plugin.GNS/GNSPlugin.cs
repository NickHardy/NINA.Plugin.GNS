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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NINA.Plugin;
using NINA.Plugin.Interfaces;

namespace NINA.Plugin.GNS {

    /// <summary>
    /// This class exports the IPlugin interface and will be used for the general plugin information and options
    /// An instance of this class will be created and set as datacontext on the plugin options tab in N.I.N.A. to be able to configure global plugin settings
    /// The user interface for the settings will be defined in the Options.xaml
    /// </summary>
    [Export(typeof(IPluginManifest))]
    public class GNSPlugin : PluginBase {

        [ImportingConstructor]
        public GNSPlugin() {
        }
    }
}