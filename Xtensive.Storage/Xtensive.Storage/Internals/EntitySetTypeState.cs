// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexander Nikolaev
// Created:    2009.08.04

using System;
using Xtensive.Core.Tuples;
using Xtensive.Storage.Providers;
using Xtensive.Storage.Rse.Providers;
using Tuple = Xtensive.Core.Tuples.Tuple;
using Xtensive.Core.Tuples.Transform;
using Xtensive.Storage.Rse;

namespace Xtensive.Storage.Internals
{
  [Serializable]
  internal sealed class EntitySetTypeState
  {
    private readonly ExecutableProvider seekProvider;

    public readonly MapTransform SeekTransform;

    public readonly Func<Tuple, Entity> ItemCtor;

    public readonly Func<long> ItemCountQuery;

    public RecordSet GetSeekRecordSet (SessionHandler handler)
    {
      return new RecordSet(handler.CreateEnumerationContext(), seekProvider);
    }

    public EntitySetTypeState(ExecutableProvider seekProvider, MapTransform seekTransform,
      Func<Tuple, Entity> itemCtor, Func<long> itemCountQuery)
    {
      this.seekProvider = seekProvider;
      SeekTransform = seekTransform;
      ItemCtor = itemCtor;
      ItemCountQuery = itemCountQuery;
    }
  }
}