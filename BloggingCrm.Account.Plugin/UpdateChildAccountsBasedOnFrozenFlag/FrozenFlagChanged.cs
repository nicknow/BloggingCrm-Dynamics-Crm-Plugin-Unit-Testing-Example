using System;
using BloggingCrm.Account.BusinessLogic.Implementations;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore;
using ThinkCrm.Core.PluginCore.Attributes;
using ThinkCrm.Core.PluginCore.Logging;

namespace BloggingCrm.Account.Plugin.UpdateChildAccountsBasedOnFrozenFlag
{
    [TargetEntity(true, CrmObject.Account.EntityLogicalName)]
    [PreImageValidator(ImageName, CrmObject.Account.EntityLogicalName, true, CrmObject.Account.Fields.think_frozen)]
    public class FrozenFlagChanged : CorePlugin
    {
        private const string ImageName = "PreImage";

        /// <summary>
        /// This constructor will skip configuration of the ObjectProviderService.
        /// Call this from a derived class being used as a testing harness 
        /// </summary>
        /// <param name="switcher">value does not matter</param>
        protected FrozenFlagChanged(bool switcher)
        {

        }

        public FrozenFlagChanged()
        {
            ObjectProviderService.RegisterType<IFrozenAccountManagement>(new FrozenAccountManagement());
        }

        protected override bool ExecutePostOpSync(IPluginSetup p)
        {
            if (!ObjectProviderService.Contains<IFrozenAccountManagement>()) throw new NullReferenceException(nameof(IFrozenAccountManagement));

            var l = new LocalLogger(p.Logging, ClassName);            

            var target = p.Helper.GetTargetEntity<CrmObject.Account>();

            if (!target.Contains(CrmObject.Account.Fields.think_frozen))
                return l.WriteAndReturn(false, $"Attribute not in target: {CrmObject.Account.Fields.think_frozen}.");

            var preImage = p.Context.PreEntityImages[ImageName].ToEntity<CrmObject.Account>();

            if (!DidValueChange(preImage, target, CrmObject.Account.Fields.think_frozen))
                return l.WriteAndReturn(false, "Frozen flag did not change, Exiting plugin.");

            try
            {
                ObjectProviderService.GetObject<IFrozenAccountManagement>()
                    .ProcessChangedFrozenFlag(p.SystemService, l, target);
            }
            catch (Exception ex)
            {
                p.Logging.Write("Exception while updating Frozen flag on Account.");
                p.Logging.Write(ex);
                throw new InvalidPluginExecutionException(
                    "Plugin Error while updating Frozen Flag on Account records. Please try again. If this issue continues click download log file and contact support.");
            }

            return l.WriteAndReturn(false, "Execution Completed.");
        }

        public bool DidValueChange(Entity initialEntity, Entity updatedEntity, string attribute)
        {
            if (initialEntity == null) throw new ArgumentNullException(nameof(initialEntity));
            if (updatedEntity == null) throw new ArgumentNullException(nameof(updatedEntity));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            return !initialEntity.Contains(attribute) || !updatedEntity.Contains(attribute) ||
                   !initialEntity[attribute].Equals(updatedEntity[attribute]);
        }
    }



}
