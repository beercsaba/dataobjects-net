// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Kofman
// Created:    2009.04.23

namespace Xtensive.Storage.Building
{
  /// <summary>
  /// Schema upgrade modes.
  /// </summary>
  public enum SchemaUpgradeMode
  {
    /// <summary>
    /// Validate schema to be equal to the domain model.
    /// </summary>
    ValidateExact,

    /// <summary>
    /// Validate schema to be compatible (equal or greater) with the domain model.
    /// </summary>
    ValidateCompatible,

    /// <summary>
    /// Upgrade schema to domain model.
    /// </summary>
    Upgrade,

    /// <summary>
    /// Upgrade schema to domain model safely - 
    /// i.e. without any operations leading to data lost.
    /// </summary>
    UpgradeSafely,

    /// <summary>
    /// Completely recreate the schema.
    /// </summary>
    Recreate
  }
}