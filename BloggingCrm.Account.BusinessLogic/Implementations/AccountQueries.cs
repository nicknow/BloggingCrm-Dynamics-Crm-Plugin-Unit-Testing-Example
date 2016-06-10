using System;
using System.Collections.Generic;
using System.Linq;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using BloggingCrm.CrmObject;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace BloggingCrm.Account.BusinessLogic.Implementations
{
    public class AccountQueries : IAccountQueries
    {
        private readonly ICrmService _crmService;

        public AccountQueries(ICrmService crmService)
        {
            if (crmService == null) throw new ArgumentNullException(nameof(crmService));
            _crmService = crmService;
        }

        public List<CrmObject.Account> GetAllAccounts()
        {
            using (var ctx = new XRMContext(_crmService))
            {
                return (from act in ctx.AccountSet
                    select act).ToList();
            }
        }

        public List<CrmObject.Account> GetChildAccountsForAnAccount(EntityReference parentAccount)
        {
            if (parentAccount == null) throw new ArgumentNullException(nameof(parentAccount));

            using (var ctx = new XRMContext(_crmService))
            {
                return (from act in ctx.AccountSet
                        where act.ParentAccountId.Id == parentAccount.Id
                        select act).ToList();
            }
        }

        public List<CrmObject.Account> GetAllChildrenAndChildrenOfChildrenAccounts(EntityReference parentAccount)
        {
            if (parentAccount == null) throw new ArgumentNullException(nameof(parentAccount));

            var result = new List<CrmObject.Account>();

            var localResult = GetChildAccountsForAnAccount(parentAccount);    
            
            result.AddRange(localResult);                   

            localResult.ForEach(x=> result.AddRange(GetAllChildrenAndChildrenOfChildrenAccounts(x.ToEntityReference())));

            return result;

        }
        
    }
}
