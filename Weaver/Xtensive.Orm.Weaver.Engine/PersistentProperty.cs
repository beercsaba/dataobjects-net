// Copyright (C) 2013 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2013.08.26

using System;
using Mono.Cecil;

namespace Xtensive.Orm.Weaver
{
  internal sealed class PersistentProperty
  {
    public PropertyDefinition Definition { get; set; }

    public bool IsKey { get; set; }

    public bool IsExplicitInterfaceImplementation { get { return ExplicitlyImplementedInterface!=null; } }

    public TypeReference ExplicitlyImplementedInterface { get; set; }

    public PersistentProperty(PropertyDefinition definition)
    {
      if (definition==null)
        throw new ArgumentNullException("definition");

      Definition = definition;
    }
  }
}