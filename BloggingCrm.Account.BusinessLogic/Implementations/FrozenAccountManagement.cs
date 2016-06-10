using System;
using System.Collections.Generic;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore.Logging;

namespace BloggingCrm.Account.BusinessLogic.Implementations
{
    public class FrozenAccountManagement : IFrozenAccountManagement
    {
        public void ProcessChangedFrozenFlag(ICrmService service, ILogging logging, CrmObject.Account account, bool forceChanges = false)
        {
            var l = new LocalLogger(logging, this.GetType().Name);

            if (service == null) throw new ArgumentNullException(nameof(service));
            if (logging == null) throw new ArgumentNullException(nameof(logging));
            if (account == null) throw new ArgumentNullException(nameof(account));

            var isFrozen = this.IsAccountFrozen(account);

            l.Write($"IsFrozen={isFrozen}");

            var accountQuery = new AccountQueries(service);

            List<CrmObject.Account> childAccounts;

            try
            {
                childAccounts = accountQuery.GetChildAccountsForAnAccount(account.ToEntityReference());
                l.Write($"Retrieved {childAccounts.Count} child records.");

            }
            catch (Exception ex)
            {
                l.Write($"Exception retrieving accounts to update for account id: {account.Id}");
                logging.Write(ex);
                throw;
            }

            var accountsToUpdate = this.UpdateAccountsFrozenFlag(childAccounts, isFrozen, forceChanges);
            l.Write($"Updating {accountsToUpdate.Count} records.");

            try
            {
                accountsToUpdate.ForEach(service.Update);
                l.Write("Completed CRM Updates.");
            }
            catch (Exception ex)
            {
                l.Write("Exception while updating records.");
                l.Write(ex);
                throw;
            }


        }

        public List<CrmObject.Account> UpdateAccountsFrozenFlag(List<CrmObject.Account> accountsToUpdate, bool isFrozen, bool forceUpdate = true)
        {
            var result = new List<CrmObject.Account>();

            accountsToUpdate.ForEach(x =>
            {
                if (forceUpdate)
                    result.Add(new CrmObject.Account()
                    {
                        Id = x.Id,
                        think_frozenEnum =
                            isFrozen ? CrmObject.Account_think_frozen.Yes : CrmObject.Account_think_frozen.No
                    });
                else
                {
                    if (x.think_frozenEnum !=
                        (isFrozen ? CrmObject.Account_think_frozen.Yes : CrmObject.Account_think_frozen.No))
                        result.Add(new CrmObject.Account()
                        {
                            Id = x.Id,
                            think_frozenEnum =
                                isFrozen ? CrmObject.Account_think_frozen.Yes : CrmObject.Account_think_frozen.No
                        });
                }
            });

            return result;
        }

        public bool IsAccountFrozen(CrmObject.Account accountEntity)
        {
            if (accountEntity == null) throw new ArgumentNullException(nameof(accountEntity));

            return accountEntity.think_frozenEnum.HasValue &&
                   accountEntity.think_frozenEnum.Value == CrmObject.Account_think_frozen.Yes;
        }
    }
}
