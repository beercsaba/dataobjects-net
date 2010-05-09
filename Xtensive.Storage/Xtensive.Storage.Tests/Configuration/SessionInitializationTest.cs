// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Ivan Galkin
// Created:    2009.03.13

using System;
using NUnit.Framework;
using Xtensive.Core.Caching;
using Xtensive.Core;
using Xtensive.Core.Reflection;
using Xtensive.Core.Testing;
using Xtensive.Storage.Configuration;

namespace Xtensive.Storage.Tests.Configuration
{
  [TestFixture]
  public class SessionInitializationTest
  {
    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      Require.ProviderIs(StorageProvider.Memory);
    }

    [Test]
    public void TestSessionCache()
    {
      // Default CacheType
      var configuration = DomainConfigurationFactory.Create();
      configuration.UpgradeMode = DomainUpgradeMode.Recreate;
      TestCacheType(configuration, typeof (LruCache<,>));
      // Lru CacheType
      configuration = DomainConfigurationFactory.Create();
      configuration.Sessions.Add(new SessionConfiguration(WellKnown.Sessions.Default) {CacheType = SessionCacheType.LruWeak});
      TestCacheType(configuration, typeof (LruCache<,>));
      // Infinite CacheType
      configuration = DomainConfigurationFactory.Create();
      configuration.Sessions.Add(new SessionConfiguration(WellKnown.Sessions.Default) {CacheType = SessionCacheType.Infinite});
      TestCacheType(configuration, typeof (InfiniteCache<,>));
    }

    public void TestCacheType(DomainConfiguration config, Type expectedType)
    {
      var d = Domain.Build(config);
      using (var s = Session.Open(d)) {
        var cacheType = s.EntityStateCache.GetType();
        Log.Debug("Session CacheType: {0}", cacheType.Name);
        Assert.IsTrue(cacheType.IsOfGenericType(expectedType));
      }
      d.DisposeSafely();
    }

    [Test]
    public void TestNamedConfigurations()
    {
      var config = DomainConfigurationFactory.Create();
      AssertEx.ThrowsArgumentNullException(() => config.Sessions.Add(new SessionConfiguration()));
      config.Sessions.Add(new SessionConfiguration("SomeName"));
      AssertEx.ThrowsInvalidOperationException(() => config.Sessions.Add(new SessionConfiguration("SomeName")));
    }
  }
}