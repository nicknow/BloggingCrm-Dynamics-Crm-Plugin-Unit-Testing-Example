using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloggingCrm.Account.BusinessLogic.Implementations;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.CrmService;
using ThinkCrm.Core.Interfaces;
using Xunit;

namespace BloggingCrm.Account.BusinessLogic.Tests
{
    // ReSharper disable once InconsistentNaming
    public class Prove_that_AccountQueries_Is_Working
    {

        private static readonly List<Entity> AccountList = GetAccountList();
        
        private static List<Entity> GetAccountList()
        {
            var act1 = new CrmObject.Account()
            {
                Id = Guid.Parse("{FCFD0F00-B18A-45E8-9CF5-2A4C6BB6923D}"),
                Name = "Account 1"
            };
            var act1a = new CrmObject.Account()
            {
                Id = Guid.Parse("{E063D3AF-13E3-4A89-9E26-3FE9E88D491B}"),
                Name = "Account 1-A",
                ParentAccountId = act1.ToEntityReference()
            };
            var act1ai = new CrmObject.Account()
            {
                Id = Guid.Parse("{66F541B7-2A0C-4668-B2D6-4E6C6F556946}"),
                Name = "Account 1-A-i",
                ParentAccountId = act1a.ToEntityReference()
            };
            var act1aii = new CrmObject.Account()
            {
                Id = Guid.Parse("{7C4DB64C-0D80-4092-8F31-D6EB7558ED39}"),
                Name = "Account 1-A-ii",
                ParentAccountId = act1a.ToEntityReference()
            };
            var act1c = new CrmObject.Account()
            {
                Id = Guid.Parse("{70446EE5-8591-4141-9429-632F231B02F6}"),
                Name = "Account 1-C",
                ParentAccountId = act1.ToEntityReference()
            };
            var act2 = new CrmObject.Account()
            {
                Id = Guid.Parse("{433AE6D5-3329-4167-9B98-A9E10A1D2F40}"),
                Name = "Account 2"
            };
            var act3 = new CrmObject.Account()
            {
                Id = Guid.Parse("{985F7DBF-F8D7-41AF-A3FE-BE6B8912C931}"),
                Name = "Account 3"
            };
            var act3a = new CrmObject.Account()
            {
                Id = Guid.Parse("{D0FF2AEA-A87D-4A08-90FD-523E4B7B52F3}"),
                Name = "Account 3-A",
                ParentAccountId = act3.ToEntityReference()
            };
            var act4 = new CrmObject.Account()
            {
                Id = Guid.Parse("{184D539D-B869-41BB-AFE5-1BC8FA4EDAD0}"),
                Name = "Account 4"
            };

            var accounts = new List<Entity>() {act1, act1a, act1ai, act1aii, act1c, act2, act3, act3a, act4};
            return accounts;
        }

        private ICrmService CreateTestingContext()
        {
            var context = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmObject.Account)) };

            var accounts = AccountList;

            context.Initialize(accounts);

            return new CrmService(context.GetFakedOrganizationService());
        }


        #region TestsOfGetAllAccountsMethod

        [Fact]
        public void GetAllAccounts_Retrieves_All_Accounts()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            var results = queryEngine.GetAllAccounts();

            Assert.True(results.Count == 9);

            Assert.All(results, (x) => x.Contains(CrmObject.Account.Fields.AccountId));
            Assert.All(results, x => x.Id.Equals(x.AccountId.Value));

        }

        [Fact]
        public void GetChildAccountsForAnAccount_When_Account_Exists()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            var results =
                queryEngine.GetChildAccountsForAnAccount(new EntityReference(CrmObject.Account.EntityLogicalName,
                    Guid.Parse("{FCFD0F00-B18A-45E8-9CF5-2A4C6BB6923D}")));

            Assert.True(results.Count == 2);
            Assert.True(results.Any(x => x.Id == Guid.Parse("{70446EE5-8591-4141-9429-632F231B02F6}")));
            Assert.True(results.Any(x => x.Id == Guid.Parse("{E063D3AF-13E3-4A89-9E26-3FE9E88D491B}")));

        }

        [Fact]
        public void GetChildAccountsForAnAccount_When_No_Account_Exists()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            Assert.False(queryEngine.GetChildAccountsForAnAccount(new EntityReference(CrmObject.Account.EntityLogicalName, Guid.NewGuid())).Any());
        }

        #endregion

        #region Check Null Exceptions

        [Fact]
        public void AccountQueriesConstructor_Throws_Error_On_Null_Service()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountQueries(null));
        }

        [Fact]
        public void GetAllChildrenAndChildrenOfChildrenAccounts_Throws_Error_On_Null_EntityReference()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            Assert.Throws<ArgumentNullException>(() => queryEngine.GetAllChildrenAndChildrenOfChildrenAccounts(null));
        }

        [Fact]
        public void GetChildAccountsForAnAccount_Throws_Error_On_Null_EntityReference()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            Assert.Throws<ArgumentNullException>(() => queryEngine.GetChildAccountsForAnAccount(null));
        } 

        #endregion

        [Fact]
        public void GetAllChildrenAndChildrenOfChildrenAccounts_With_Multiple_Levels()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            var results =
                queryEngine.GetAllChildrenAndChildrenOfChildrenAccounts(new EntityReference(CrmObject.Account.EntityLogicalName,
                    Guid.Parse("{FCFD0F00-B18A-45E8-9CF5-2A4C6BB6923D}")));

            Assert.True(results.Count == 4);
            Assert.True(results.Any(x => x.Id == Guid.Parse("{70446EE5-8591-4141-9429-632F231B02F6}")));
            Assert.True(results.Any(x => x.Id == Guid.Parse("{E063D3AF-13E3-4A89-9E26-3FE9E88D491B}")));

            Assert.True(results.Any(x => x.Id == Guid.Parse("{66F541B7-2A0C-4668-B2D6-4E6C6F556946}")));
            Assert.True(results.Any(x => x.Id == Guid.Parse("{E063D3AF-13E3-4A89-9E26-3FE9E88D491B}")));
        }

        [Fact]
        public void GetAllChildrenAndChildrenOfChildrenAccounts_With_No_Children()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            var results =
                queryEngine.GetAllChildrenAndChildrenOfChildrenAccounts(new EntityReference(CrmObject.Account.EntityLogicalName,
                    Guid.Parse("{66F541B7-2A0C-4668-B2D6-4E6C6F556946}")));

            Assert.True(results.Count == 0);
        }

        [Fact]
        public void GetAllChildrenAndChildrenOfChildrenAccounts_With_Children_And_A_Parent()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            var results =
                queryEngine.GetAllChildrenAndChildrenOfChildrenAccounts(new EntityReference(CrmObject.Account.EntityLogicalName,
                    Guid.Parse("{E063D3AF-13E3-4A89-9E26-3FE9E88D491B}")));

            Assert.True(results.Count == 2);
            Assert.True(results.Any(x => x.Id == Guid.Parse("{66F541B7-2A0C-4668-B2D6-4E6C6F556946}")));
            Assert.True(results.Any(x => x.Id == Guid.Parse("{7C4DB64C-0D80-4092-8F31-D6EB7558ED39}")));
        }

        [Fact]
        public void GetAllChildrenAndChildrenOfChildrenAccounts_When_No_Account_Exists()
        {
            var orgService = CreateTestingContext();

            IAccountQueries queryEngine = new AccountQueries(orgService);

            Assert.False(queryEngine.GetAllChildrenAndChildrenOfChildrenAccounts(new EntityReference(CrmObject.Account.EntityLogicalName, Guid.NewGuid())).Any());
        }



    }
}
