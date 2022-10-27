#region "copyright"

/*
    Copyright © 2016 - 2020 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Trigger;
using NINA.Sequencer.Validations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.Plugin.GNS.Sequencer.Utility;
using NINA.Sequencer.Trigger.MeridianFlip;
using NINA.Profile.Interfaces;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using NINA.WPF.Base.Interfaces;

namespace NINA.Plugin.GNS.Sequencer.Trigger {

    /// <summary>
    /// This Item shows the basic principle on how to add a new Sequence Item to the N.I.N.A. sequencer via the plugin interface
    /// For ease of use this item inherits the abstract SequenceItem which already handles most of the running logic, like logging, exception handling etc.
    /// A complete custom implementation by just implementing ISequenceItem is possible too
    /// The following MetaData can be set to drive the initial values
    /// --> Name - The name that will be displayed for the item
    /// --> Description - a brief summary of what the item is doing. It will be displayed as a tooltip on mouseover in the application
    /// --> Icon - a string to the key value of a Geometry inside N.I.N.A.'s geometry resources
    ///
    /// If the item has some preconditions that should be validated, it shall also extend the IValidatable interface and add the validation logic accordingly.
    /// </summary>
    [ExportMetadata("Name", "GNS MeridianFlip trigger")]
    [ExportMetadata("Description", "This item will trigger the meridian flip and send a message to the GNS system.")]
    [ExportMetadata("Icon", "Plugin_GNS_SVG")]
    [ExportMetadata("Category", "GNS")]
    [Export(typeof(ISequenceTrigger))]
    [JsonObject(MemberSerialization.OptIn)]
    public class PluginGNSMeridianFlipTrigger : SequenceTrigger, IValidatable {
        private GNSUtility GNSUtil;

        protected IProfileService profileService;
        protected ITelescopeMediator telescopeMediator;
        protected IApplicationStatusMediator applicationStatusMediator;
        protected ICameraMediator cameraMediator;
        protected IFocuserMediator focuserMediator;
        protected IMeridianFlipVMFactory meridianFlipVMFactory;

        private MeridianFlipTrigger MeridianFlipTrigger;

        [ImportingConstructor]
        public PluginGNSMeridianFlipTrigger(IProfileService profileService, ICameraMediator cameraMediator, ITelescopeMediator telescopeMediator,
            IFocuserMediator focuserMediator, IApplicationStatusMediator applicationStatusMediator, IMeridianFlipVMFactory meridianFlipVMFactory) : base() {
            this.MeridianFlipTrigger = new MeridianFlipTrigger(profileService, cameraMediator, telescopeMediator, focuserMediator, applicationStatusMediator, meridianFlipVMFactory);
            this.profileService = profileService;
            this.telescopeMediator = telescopeMediator;
            this.applicationStatusMediator = applicationStatusMediator;
            this.cameraMediator = cameraMediator;
            this.focuserMediator = focuserMediator;
            this.meridianFlipVMFactory = meridianFlipVMFactory;
        }

        protected PluginGNSMeridianFlipTrigger(PluginGNSMeridianFlipTrigger cloneMe) : this(cloneMe.profileService, cloneMe.cameraMediator, cloneMe.telescopeMediator, cloneMe.focuserMediator, cloneMe.applicationStatusMediator, cloneMe.meridianFlipVMFactory) {
            CopyMetaData(cloneMe);
        }

        public override object Clone() {
            return new PluginGNSMeridianFlipTrigger(this);
        }

        /// <summary>
        /// The core logic when the sequence item is running resides here
        /// Add whatever action is necessary
        /// </summary>
        /// <param name="progress">The application status progress that can be sent back during execution</param>
        /// <param name="token">When a cancel signal is triggered from outside, this token can be used to register to it or check if it is cancelled</param>
        /// <returns></returns>
        public override Task Execute(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            var guiderSettings = profileService.ActiveProfile.GuiderSettings;
            GNSUtil.sendMessage(Convert.ToInt32(CalculateMinimumTimeRemaining().TotalSeconds) + guiderSettings.SettleTimeout + 900, "Meridian flip triggered.");
            return MeridianFlipTrigger.Execute(context, progress, token);
        }

        public override void Initialize() {
            // Start GNS service
            GNSUtil = new GNSUtility();
            GNSUtil.InitializeService();
        }

        private IList<string> issues = new List<string>();

        public IList<string> Issues {
            get => issues;
            set {
                issues = ImmutableList.CreateRange(value);
                RaisePropertyChanged();
            }
        }

        public override bool ShouldTrigger(ISequenceItem previousItem, ISequenceItem nextItem) {
            bool shouldTrigger = this.MeridianFlipTrigger.ShouldTrigger(previousItem, nextItem);
            RaisePropertyChanged(nameof(LatestFlipTime));
            RaisePropertyChanged(nameof(EarliestFlipTime));
            return shouldTrigger;
        }

        public virtual DateTime LatestFlipTime {
            get => this.MeridianFlipTrigger.LatestFlipTime;
        }

        public virtual DateTime EarliestFlipTime {
            get => this.MeridianFlipTrigger.EarliestFlipTime;
        }

        private TimeSpan CalculateMinimumTimeRemaining() {
            var settings = profileService.ActiveProfile.MeridianFlipSettings;
            //Substract delta from maximum to get minimum time
            var delta = settings.MaxMinutesAfterMeridian - settings.MinutesAfterMeridian;
            var time = CalculateMaximumTimeRemainaing() - TimeSpan.FromMinutes(delta);
            if (time < TimeSpan.Zero) {
                time = TimeSpan.Zero;
            }
            return time;
        }

        private TimeSpan CalculateMaximumTimeRemainaing() {
            var telescopeInfo = telescopeMediator.GetInfo();

            return TimeSpan.FromHours(telescopeInfo.TimeToMeridianFlip);
        }

        /// <summary>
        /// When items are put into the sequence via the factory, the factory will call the clone method. Make sure all the relevant fields are cloned with the object.
        /// </summary>
        /// <returns></returns>
/*        public override object Clone() {
            return new PluginGNSMeridianFlipTrigger(profileService, cameraMediator, telescopeMediator, guiderMediator, focuserMediator, imagingMediator, domeMediator, domeFollower, applicationStatusMediator, filterWheelMediator, history) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                GNSUtil = GNSUtil
            };
        }*/

        public bool Validate() {
            var i = new List<string>();

            if (GNSUtil == null) this.Initialize();
            if (GNSUtil.GetGNSService() == null) {
                i.Add("GNS server could not be initialized.");
                Issues = i;
                return i.Count == 0;
            }

            return MeridianFlipTrigger.Validate();
        }

        /// <summary>
        /// This string will be used for logging
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(PluginGNSMeridianFlipTrigger)}";
        }
    }
}