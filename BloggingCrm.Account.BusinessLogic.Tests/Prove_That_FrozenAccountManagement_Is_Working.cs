using System;
using System.Collections.Generic;
using System.Linq;
using BloggingCrm.Account.BusinessLogic.Implementations;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace BloggingCrm.Account.BusinessLogic.Tests
{
    public class Prove_That_FrozenAccountManagement_Is_Working
    {
        [Fact]
        public void IsAccountFrozen_Comprehensive_Test()
        {
            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            Assert.False(
                testedClass.IsAccountFrozen(new CrmObject.Account()
                {
                    think_frozenEnum = CrmObject.Account_think_frozen.No
                }));

            Assert.True(
                testedClass.IsAccountFrozen(new CrmObject.Account()
                {
                    think_frozenEnum = CrmObject.Account_think_frozen.Yes
                }));

            Assert.Throws<ArgumentNullException>(() => testedClass.IsAccountFrozen(null));

            Assert.False(
                testedClass.IsAccountFrozen(new CrmObject.Account()
                {
                    think_frozen = new OptionSetValue(766780000)
                }));

            Assert.True(
                testedClass.IsAccountFrozen(new CrmObject.Account()
                {
                    think_frozen = new OptionSetValue(766780001)
                }));

            Assert.False(
                testedClass.IsAccountFrozen(new CrmObject.Account()
                {
                    Name = "Test of Null Frozen Field"
                }));

        }

        public void UpdateAccountsFrozenFlag_Null_Object()
        {
            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            Assert.Throws<ArgumentNullException>(() => testedClass.UpdateAccountsFrozenFlag(null, true, true));
        }

        [Fact]
        public void UpdateAccountsFrozenFlag_Set_All_False()
        {

            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            var input = new List<CrmObject.Account>()
            {
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No}
            };

            var expectedResults = input.Select(x => x.Id).ToList();

            var actualResults = testedClass.UpdateAccountsFrozenFlag(input, false, true);

            Assert.Equal(actualResults.Count(), 4);
            Assert.False(actualResults.Any(x => x.think_frozen.Value == (int)CrmObject.Account_think_frozen.Yes));
            Assert.True(expectedResults.All(x => actualResults.Any(y => y.Id == x)));

        }

        [Fact]
        public void UpdateAccountsFrozenFlag_Set_All_True()
        {

            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            var input = new List<CrmObject.Account>()
            {
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No}
            };

            var expectedResults = input.Select(x => x.Id).ToList();

            var actualResults = testedClass.UpdateAccountsFrozenFlag(input, true, true);

            Assert.Equal(actualResults.Count(), 4);
            Assert.False(actualResults.Any(x=>x.think_frozen.Value == (int)CrmObject.Account_think_frozen.No));
            Assert.True(expectedResults.All(x => actualResults.Any(y => y.Id == x)));

        }

        [Fact]
        public void UpdateAccountsFrozenFlag_Set_Changed_True()
        {

            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            var input = new List<CrmObject.Account>()
            {
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No}
            };

            var expectedResults = input.Where(x=>x.think_frozenEnum == CrmObject.Account_think_frozen.No).Select(x => x.Id).ToList();

            var actualResults = testedClass.UpdateAccountsFrozenFlag(input, true, false);

            Assert.Equal(actualResults.Count(), expectedResults.Count());
            Assert.False(actualResults.Any(x => x.think_frozen.Value == (int)CrmObject.Account_think_frozen.No));
            Assert.True(expectedResults.All(x => actualResults.Any(y => y.Id == x)));

        }

        [Fact]
        public void UpdateAccountsFrozenFlag_Set_Changed_False()
        {

            IFrozenAccountManagement testedClass = new FrozenAccountManagement();

            var input = new List<CrmObject.Account>()
            {
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.Yes},
                new CrmObject.Account() {Id = Guid.NewGuid(), think_frozenEnum = CrmObject.Account_think_frozen.No}
            };

            var expectedResults = input.Where(x => x.think_frozenEnum == CrmObject.Account_think_frozen.Yes).Select(x => x.Id).ToList();

            var actualResults = testedClass.UpdateAccountsFrozenFlag(input, false, false);

            Assert.Equal(actualResults.Count(), expectedResults.Count());
            Assert.False(actualResults.Any(x => x.think_frozen.Value == (int)CrmObject.Account_think_frozen.Yes));
            Assert.True(expectedResults.All(x => actualResults.Any(y => y.Id == x)));

        }

    }
}
