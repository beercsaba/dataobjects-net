// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Aleksey Gamzov
// Created:    2008.08.21

using System;
using System.Collections.Generic;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Sql.Dom.Dml;
using Xtensive.Core.Helpers;
using Xtensive.Core.Collections;
using System.Linq;

namespace Xtensive.Sql.Dom.Database.Comparer
{
  /// <summary>
  /// <see cref="View"/> comparison result.
  /// </summary>
  [Serializable]
  public class ViewComparisonResult : DataTableComparisonResult,
    IComparisonResult<View>
  {
    private ComparisonResult<CheckOptions> checkOptions;
    private ComparisonResult<SqlNative> definition;
    private readonly ComparisonResultCollection<ViewColumnComparisonResult> columns = new ComparisonResultCollection<ViewColumnComparisonResult>();
    private readonly ComparisonResultCollection<IndexComparisonResult> indexes = new ComparisonResultCollection<IndexComparisonResult>();

    /// <inheritdoc/>
    public override IEnumerable<IComparisonResult> NestedComparisons
    {
      get
      {
        return base.NestedComparisons
          .AddOne(checkOptions)
          .AddOne(definition)
          .Union<IComparisonResult>(columns)
          .Union<IComparisonResult>(indexes);
      }
    }

    /// <inheritdoc/>
    public new View NewValue
    {
      get { return (View) base.NewValue; }
    }

    /// <inheritdoc/>
    public new View OriginalValue
    {
      get { return (View) base.OriginalValue; }
    }

    /// <summary>
    /// Gets comparison result of check options.
    /// </summary>
    public ComparisonResult<CheckOptions> CheckOptions
    {
      get { return checkOptions; }
      set
      {
        this.EnsureNotLocked();
        checkOptions = value;
      }
    }

    /// <summary>
    /// Gets comparison result of definition.
    /// </summary>
    public ComparisonResult<SqlNative> Definition
    {
      get { return definition; }
      set
      {
        this.EnsureNotLocked();
        definition = value;
      }
    }

    /// <summary>
    /// Gets comparison results of nested columns.
    /// </summary>
    public ComparisonResultCollection<ViewColumnComparisonResult> Columns
    {
      get { return columns; }
    }

    /// <summary>
    /// Gets comparison results of nested indexes.
    /// </summary>
    public ComparisonResultCollection<IndexComparisonResult> Indexes
    {
      get { return indexes; }
    }

    /// <inheritdoc/>
    public override void Lock(bool recursive)
    {
      base.Lock(recursive);
      if (recursive) {
        columns.Lock(recursive);
        indexes.Lock(recursive);
        checkOptions.LockSafely(recursive);
        definition.LockSafely(recursive);
      }
    }
  }
}