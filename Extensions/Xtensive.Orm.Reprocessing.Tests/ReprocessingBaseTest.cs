﻿using NUnit.Framework;
using TestCommon;
using TestCommon.Model;
using Xtensive.Orm.Configuration;
using Xtensive.Orm.Providers;
using Xtensive.Orm.Tests;

namespace Xtensive.Orm.Reprocessing.Tests
{
  [TestFixture]
  public abstract class ReprocessingBaseTest : CommonModelTest
  {
    protected override void CheckRequirements()
    {
      Require.AllFeaturesNotSupported(ProviderFeatures.ExclusiveWriterConnection);
    }

    protected override DomainConfiguration BuildConfiguration()
    {
      var configuration = base.BuildConfiguration();
      configuration.Types.Register(typeof (IExecuteActionStrategy).Assembly);
      configuration.Types.Register(typeof (ReprocessingBaseTest).Assembly);
      return configuration;
    }
  }
}