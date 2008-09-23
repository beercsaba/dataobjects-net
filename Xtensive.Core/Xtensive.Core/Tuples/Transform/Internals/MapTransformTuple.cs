// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.05.07

using System;
using System.Linq;
using Xtensive.Core.Collections;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Core.Tuples.Transform.Internals
{
  /// <summary>
  /// A <see cref="MapTransform"/> result tuple mapping arbitrary count of source tuples to a single one (this).
  /// </summary>
  [Serializable]
  public sealed class MapTransformTuple : TransformedTuple<MapTransform>
  {
    private readonly Tuple[] tuples;

    /// <inheritdoc/>
    public override object[] Arguments {
      get {
        return tuples.Copy();
      }
    }

    #region GetFieldState, GetValueOrDefault, SetValue methods

    /// <inheritdoc/>
    public override TupleFieldState GetFieldState(int fieldIndex)
    {
      Pair<int, int> indexes = TypedTransform.map[fieldIndex];
      return tuples[indexes.First].GetFieldState(indexes.Second);
    }

    /// <inheritdoc/>
    public override object GetValueOrDefault(int fieldIndex)
    {
      Pair<int, int> indexes = TypedTransform.map[fieldIndex];
      return tuples[indexes.First].GetValueOrDefault(indexes.Second);
    }

    /// <inheritdoc/>
    public override void SetValue(int fieldIndex, object fieldValue)
    {
      if (Transform.IsReadOnly)
        throw Exceptions.ObjectIsReadOnly(null);
      Pair<int, int> indexes = TypedTransform.map[fieldIndex];
      tuples[indexes.First].SetValue(indexes.Second, fieldValue);
    }

    #endregion


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="transform">The transform.</param>
    /// <param name="sources">Source tuples.</param>
    public MapTransformTuple(MapTransform transform, params Tuple[] sources)
      : base(transform)
    {
      ArgumentValidator.EnsureArgumentNotNull(sources, "tuples");
      // Other checks are omitted: this transform should be fast, so delayed errors are ok
      this.tuples = sources;
    }
  }
}