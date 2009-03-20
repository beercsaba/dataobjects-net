// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexis Kochetov
// Created:    2009.03.13

using System;

namespace Xtensive.Storage.Rse.Providers.Compilable
{
  /// <summary>
  /// Enumerates <see cref="ApplyProvider"/> types.
  /// </summary>
  [Serializable]
  public enum ApplyType
  {
    /// <summary>
    /// Default apply type - <see cref="Cross"/>.
    /// </summary>
    Default = Cross,
    /// <summary>
    /// Cross apply.
    /// </summary>
    Cross = 0,
    /// <summary>
    /// Outer apply.
    /// </summary>
    Outer,
    /// <summary>
    /// Enumerates left items for which right source is not empty.
    /// </summary>
    Existing,
    /// <summary>
    /// Enumerates left items for which right source is empty.
    /// </summary>
    NotExisting,
//    /// <summary>
//    /// Applies <see cref="bool"/> column. Value is <see langword="true" /> if right source is not empty; otherwise <see langword="false" />.
//    /// </summary>
//    ExistenceColumn
  }
}