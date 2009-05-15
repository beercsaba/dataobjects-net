// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.05.15

using System;
using System.Linq.Expressions;

namespace Xtensive.Core.Linq.SerializableExpressions
{
  /// <summary>
  /// A serializable representation of <see cref="MemberMemberBinding"/>.
  /// </summary>
  [Serializable]
  public sealed class SerializableMemberMemberBinding : SerializableMemberBinding
  {
    /// <summary>
    /// <see cref="MemberMemberBinding.Bindings"/>
    /// </summary>
    public SerializableMemberBinding[] Bindings;
  }
}