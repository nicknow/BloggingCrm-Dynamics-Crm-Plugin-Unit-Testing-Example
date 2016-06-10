using System;
using System.Collections.Generic;
using System.Reflection;
using BloggingCrm.Account.BusinessLogic.Interfaces;
using FakeItEasy;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;
using Xunit;

namespace BloggingCrm.Account.Plugin.Tests.UpdateChildAccountsBasedOnFrozenFlag
{
    public class Prove_That_Frozen_Flag_Plugin_Works
    {
        [Fact]
        public void Check_For_Required_Object_Setup()
        {

            var pluginObj = new FrozenFlagChangedTestHarness(true);

            Assert.True(pluginObj.HasInjectableType<IFrozenAccountManagement>());
            
        }

        [Fact]
        public void Runs_When_Given_An_Account_Target_And_Pre_Image()
        {
            var fakedContext = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmObject.Account)) };
            var ctx = fakedContext.GetDefaultPluginContext();
            ctx.MessageName = "Create";
            ctx.Mode = (int)ThinkCrm.Core.PluginCore.Helper.ExecutionMode.Synchronous;
            ctx.IsolationMode = (int)ThinkCrm.Core.PluginCore.Helper.IsolationMode.None;
            ctx.Stage = (int)ThinkCrm.Core.PluginCore.Helper.PipelineStage.Postoperation;

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.Yes
            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            ctx.InputParameters = new ParameterCollection { new KeyValuePair<string, object>("Target", target) };
            ctx.PreEntityImages = new EntityImageCollection {{"PreImage", preImage}};

            ctx.PrimaryEntityName = target.LogicalName;
            ctx.PrimaryEntityId = target.Id;
                                   
            fakedContext.ExecutePluginWith<FrozenFlagChangedTestHarness>(ctx);
            
        }

        [Fact]
        public void Execption_If_Not_An_Account()
        {
            var fakedContext = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmObject.Account)) };
            var ctx = fakedContext.GetDefaultPluginContext();
            ctx.MessageName = "Create";
            ctx.Mode = (int)ThinkCrm.Core.PluginCore.Helper.ExecutionMode.Synchronous;
            ctx.IsolationMode = (int)ThinkCrm.Core.PluginCore.Helper.IsolationMode.None;
            ctx.Stage = (int)ThinkCrm.Core.PluginCore.Helper.PipelineStage.Postoperation;

            var targetGuid = Guid.NewGuid();
            var target = new Entity("contact")
            {
                Id = targetGuid,
            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            ctx.InputParameters = new ParameterCollection { new KeyValuePair<string, object>("Target", target) };
            ctx.PreEntityImages = new EntityImageCollection { { "PreImage", preImage } };

            ctx.PrimaryEntityName = target.LogicalName;
            ctx.PrimaryEntityId = target.Id;

            Assert.Throws<InvalidPluginExecutionException>(() =>
                fakedContext.ExecutePluginWith<FrozenFlagChangedTestHarness>(ctx));
        }

        [Fact]
        public void Exception_If_Pre_Image_Is_Missing_Attribute()
        {
            var fakedContext = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmObject.Account)) };
            var ctx = fakedContext.GetDefaultPluginContext();
            ctx.MessageName = "Create";
            ctx.Mode = (int)ThinkCrm.Core.PluginCore.Helper.ExecutionMode.Synchronous;
            ctx.IsolationMode = (int)ThinkCrm.Core.PluginCore.Helper.IsolationMode.None;
            ctx.Stage = (int)ThinkCrm.Core.PluginCore.Helper.PipelineStage.Postoperation;

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.Yes
            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
            };

            ctx.InputParameters = new ParameterCollection { new KeyValuePair<string, object>("Target", target) };
            ctx.PreEntityImages = new EntityImageCollection { { "PreImage", preImage } };

            ctx.PrimaryEntityName = target.LogicalName;
            ctx.PrimaryEntityId = target.Id;

            Assert.Throws<InvalidPluginExecutionException>(() =>
                fakedContext.ExecutePluginWith<FrozenFlagChangedTestHarness>(ctx));

        }

        [Fact]
        public void Exception_If_Pre_Image_Is_Missing()
        {
            var fakedContext = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmObject.Account)) };
            var ctx = fakedContext.GetDefaultPluginContext();
            ctx.MessageName = "Create";
            ctx.Mode = (int)ThinkCrm.Core.PluginCore.Helper.ExecutionMode.Synchronous;
            ctx.IsolationMode = (int)ThinkCrm.Core.PluginCore.Helper.IsolationMode.None;
            ctx.Stage = (int)ThinkCrm.Core.PluginCore.Helper.PipelineStage.Postoperation;

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.Yes
            };

            ctx.InputParameters = new ParameterCollection { new KeyValuePair<string, object>("Target", target) };

            ctx.PrimaryEntityName = target.LogicalName;
            ctx.PrimaryEntityId = target.Id;

            Assert.Throws<InvalidPluginExecutionException>(() =>
                fakedContext.ExecutePluginWith<FrozenFlagChangedTestHarness>(ctx));
        }

        [Fact]
        public void Calls_Frozen_Account_Management_When_Frozen_Field_Is_Changed()
        {
            var pluginSetup = A.Fake<IPluginSetup>();
            var ctx = A.Fake<IPluginExecutionContext>();
            A.CallTo(() => pluginSetup.Context).Returns(ctx);
            A.CallTo(() => pluginSetup.Helper.GetTargetEntity<CrmObject.Account>()).ReturnsLazily(() => ((Entity)pluginSetup.Context.InputParameters["Target"]).ToEntity<CrmObject.Account>());
            A.CallTo(() => ctx.ParentContext).Returns(null);

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.Yes
            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            A.CallTo(() => ctx.InputParameters)
                .Returns(new ParameterCollection {new KeyValuePair<string, object>("Target", target)});
            A.CallTo(() => ctx.PreEntityImages).Returns(new EntityImageCollection {{"PreImage", preImage}});

            var fakeFam = A.Fake<IFrozenAccountManagement>();            

            var pluginObj = new FrozenFlagChangedTestHarness(fakeFam);

            pluginObj.ExecutePostOpSync(pluginSetup);

            A.CallTo(
                () =>
                    fakeFam.ProcessChangedFrozenFlag(A<ICrmService>._, A<ILogging>._, A<CrmObject.Account>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);


        }

        [Fact]
        public void Does_Not_Call_Frozen_Account_Management_When_Frozen_Attribute_Not_In_Target()
        {
            var pluginSetup = A.Fake<IPluginSetup>();
            var ctx = A.Fake<IPluginExecutionContext>();
            A.CallTo(() => pluginSetup.Context).Returns(ctx);
            A.CallTo(() => pluginSetup.Helper.GetTargetEntity<CrmObject.Account>()).ReturnsLazily(() => ((Entity)pluginSetup.Context.InputParameters["Target"]).ToEntity<CrmObject.Account>()); A.CallTo(() => ctx.ParentContext).Returns(null);

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            A.CallTo(() => ctx.InputParameters)
                .Returns(new ParameterCollection { new KeyValuePair<string, object>("Target", target) });
            A.CallTo(() => ctx.PreEntityImages).Returns(new EntityImageCollection { { "PreImage", preImage } });

            var fakeFam = A.Fake<IFrozenAccountManagement>();            

            var pluginObj = new FrozenFlagChangedTestHarness(fakeFam);

            pluginObj.ExecutePostOpSync(pluginSetup);

            A.CallTo(
                () =>
                    fakeFam.ProcessChangedFrozenFlag(A<ICrmService>._, A<ILogging>._, A<CrmObject.Account>._, A<bool>._))
                .MustHaveHappened(Repeated.Never);

        }

        [Fact]
        public void Does_Not_Call_Frozen_Account_Management_When_Frozen_Attribute_Did_Not_Change()
        {
            var pluginSetup = A.Fake<IPluginSetup>();
            var ctx = A.Fake<IPluginExecutionContext>();
            A.CallTo(() => pluginSetup.Context).Returns(ctx);
            A.CallTo(() => pluginSetup.Helper.GetTargetEntity<CrmObject.Account>()).ReturnsLazily(() => ((Entity)pluginSetup.Context.InputParameters["Target"]).ToEntity<CrmObject.Account>());
            A.CallTo(() => ctx.ParentContext).Returns(null);

            var targetGuid = Guid.NewGuid();
            var target = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            var preImage = new CrmObject.Account()
            {
                Id = targetGuid,
                think_frozenEnum = CrmObject.Account_think_frozen.No
            };

            A.CallTo(() => ctx.InputParameters)
                .Returns(new ParameterCollection { new KeyValuePair<string, object>("Target", target) });
            A.CallTo(() => ctx.PreEntityImages).Returns(new EntityImageCollection { { "PreImage", preImage } });

            var fakeFam = A.Fake<IFrozenAccountManagement>();            

            var pluginObj = new FrozenFlagChangedTestHarness(fakeFam);

            pluginObj.ExecutePostOpSync(pluginSetup);

            A.CallTo(
                () =>
                    fakeFam.ProcessChangedFrozenFlag(A<ICrmService>._, A<ILogging>._, A<CrmObject.Account>._, A<bool>._))
                .MustHaveHappened(Repeated.Never);
        }


    }
}
