using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace BloggingCrm.Account.BusinessLogic.Interfaces
{
    public interface IAccountQueries
    {

        List<BloggingCrm.CrmObject.Account> GetAllAccounts();

        List<CrmObject.Account> GetChildAccountsForAnAccount(EntityReference parentAccount);

        List<CrmObject.Account> GetAllChildrenAndChildrenOfChildrenAccounts(EntityReference parentAccount);
    }
}
