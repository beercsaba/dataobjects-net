﻿// Copyright (C) 2012 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2012.02.24

using System;

namespace Xtensive.Orm.Providers
{
  public interface ISqlTaskProcessor
  {
    /// <summary>
    /// Processes the specified task.
    /// </summary>
    /// <param name="task">The task to process.</param>
    void ProcessTask(SqlLoadTask task, Guid uniqueIdentifier);

    /// <summary>
    /// Processes the specified task.
    /// </summary>
    /// <param name="task">The task to process.</param>
    void ProcessTask(SqlPersistTask task, Guid uniqueIdentifier);
  }
}