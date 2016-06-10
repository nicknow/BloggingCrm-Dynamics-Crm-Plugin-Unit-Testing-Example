using System.Collections.Generic;
using ThinkCrm.Core.Interfaces;

namespace BloggingCrm.Account.BusinessLogic.Interfaces
{
    public interface IFrozenAccountManagement
    {
        void ProcessChangedFrozenFlag(ICrmService service, ILogging logging, CrmObject.Account account,
            bool forceChanges = false);

        List<CrmObject.Account> UpdateAccountsFrozenFlag(List<CrmObject.Account> accountsToUpdate, bool isFrozen, bool forceUpdate = true);

        bool IsAccountFrozen(CrmObject.Account accountEntity);
    }
}
