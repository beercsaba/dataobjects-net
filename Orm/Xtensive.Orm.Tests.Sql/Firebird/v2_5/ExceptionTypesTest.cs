﻿// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.11.20

using System;
using NUnit.Framework;

namespace Xtensive.Orm.Tests.Sql.Firebird.v2_5
{
  [TestFixture, Explicit]
  public class ExceptionTypesTest : Firebird.ExceptionTypesTest
  {
    protected override void CheckRequirements()
    {
      Require.ProviderVersionAtLeast(new Version(2, 5));
    }
  }
}
