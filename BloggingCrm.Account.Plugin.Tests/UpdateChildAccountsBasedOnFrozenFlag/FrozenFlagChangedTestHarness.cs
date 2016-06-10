using BloggingCrm.Account.BusinessLogic.Interfaces;
using BloggingCrm.Account.Plugin.UpdateChildAccountsBasedOnFrozenFlag;
using FakeItEasy;
using ThinkCrm.Core.Interfaces;

namespace BloggingCrm.Account.Plugin.Tests.UpdateChildAccountsBasedOnFrozenFlag
{
    class FrozenFlagChangedTestHarness : FrozenFlagChanged
    {
        public FrozenFlagChangedTestHarness() : base(true)
        {           
            ObjectProviderService.RegisterType<IFrozenAccountManagement>(A.Fake<IFrozenAccountManagement>());
        }

        public FrozenFlagChangedTestHarness(IFrozenAccountManagement frozenAccountManagement) : base(true)
        {
            ObjectProviderService.RegisterType<IFrozenAccountManagement>(frozenAccountManagement);
        }

        public new bool ExecutePostOpSync(IPluginSetup pluginSetup)
        {
            return base.ExecutePostOpSync(pluginSetup);
        }

        public FrozenFlagChangedTestHarness(bool swicher) : base()
        { }

        public bool HasInjectableType<T>()
        {
            return ObjectProviderService.Contains<T>();
        }

    }
}
