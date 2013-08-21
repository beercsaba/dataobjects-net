﻿// Copyright (C) 2013 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2013.08.19

using System;
using Mono.Cecil;

namespace Xtensive.Orm.Weaver.Tasks
{
  internal sealed class ReplaceAutoPropertyTask : WeavingTask
  {
    private readonly TypeDefinition type;
    private readonly PropertyDefinition property;

    public override ActionResult Execute(ProcessorContext context)
    {
      return ActionResult.Success;
    }

    public ReplaceAutoPropertyTask(TypeDefinition type, PropertyDefinition property)
    {
      if (type==null)
        throw new ArgumentNullException("type");
      if (property==null)
        throw new ArgumentNullException("property");

      this.type = type;
      this.property = property;
    }
  }
}