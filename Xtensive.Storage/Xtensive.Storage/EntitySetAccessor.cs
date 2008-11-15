// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.11.06

using System;
using Xtensive.Core;
using Xtensive.Core.Aspects;
using Xtensive.Storage.Model;
using Xtensive.Storage.Rse;

namespace Xtensive.Storage
{
  public sealed class EntitySetAccessor : SessionBound
  {
    [Infrastructure]
    public RecordSet GetRecordSet(EntitySetBase target)
    {
      ValidateArguments(target);
      return target.items;
    }

    [Infrastructure]
    public RecordSet GetRecordSet(Persistent target, FieldInfo field)
    {
      ValidateArguments(target, field);
      return GetRecordSet(target.GetField<EntitySetBase>(field, false));
    }

    [Infrastructure]
    public bool Add(EntitySetBase target, Entity item)
    {
      ValidateArguments(target, item);
      throw new NotImplementedException();
    }

    [Infrastructure]
    public bool Add(Entity target, FieldInfo field, Entity item)
    {
      ValidateArguments(target, field, item);
      return Add(target.GetField<EntitySetBase>(field, false), item);
    }

    [Infrastructure]
    public bool Remove(EntitySetBase target, Entity item)
    {
      ValidateArguments(target, item);
      throw new NotImplementedException();
    }

    [Infrastructure]
    public bool Remove(Entity target, FieldInfo field, Entity item)
    {
      ValidateArguments(target, field, item);
      return Remove(target.GetField<EntitySetBase>(field, false), item);
    }

    [Infrastructure]
    public void Clear(EntitySetBase target)
    {
      ValidateArguments(target);
      throw new NotImplementedException();
    }

    [Infrastructure]
    public void Clear(Entity target, FieldInfo field)
    {
      ValidateArguments(target, field);
      Clear(target.GetField<EntitySetBase>(field, false));
    }

    private static void ValidateArguments(object target)
    {
      ArgumentValidator.EnsureArgumentNotNull(target, "target");
    }

    private static void ValidateArguments(Persistent target, FieldInfo field)
    {
      ValidateArguments(target);
      ArgumentValidator.EnsureArgumentNotNull(field, "field");
      if (!target.Type.Fields.Contains(field))
        throw new InvalidOperationException(string.Format("Type '{0}' doesn't contain '{1}' field.", target.Type.Name, field.Name));
      if (!field.IsEntitySet)
        throw new InvalidOperationException(string.Format("Field '{0}' is not an EntitySet field.", field.Name));
    }

    private static void ValidateArguments(EntitySetBase target, Entity item)
    {
      ValidateArguments(target);
      ArgumentValidator.EnsureArgumentNotNull(item, "item");
    }

    private static void ValidateArguments(Persistent target, FieldInfo field, Entity item)
    {
      ValidateArguments(target, field);
      ArgumentValidator.EnsureArgumentNotNull(item, "item");
    }


    public EntitySetAccessor(Session session)
      : base(session)
    {
    }
  }
}