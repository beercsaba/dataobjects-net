// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2009.03.23

using System;
using Xtensive.Core;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Modelling.Validation
{
  /// <summary>
  /// Model validation scope.
  /// </summary>
  public class ValidationScope : Scope<ValidationContext>
  {
    /// <summary>
    /// Gets the current context.
    /// </summary>
    public new static ValidationContext CurrentContext {
      get {
        return Scope<ValidationContext>.CurrentContext;
      }
    }
    
    /// <summary>
    /// Gets the associated validation context.
    /// </summary>
    public new ValidationContext Context {
      get {
        return base.Context;
      }
    }
    
    /// <summary>
    /// Opens a validation context and scope.
    /// </summary>
    /// <returns>A new <see cref="ValidationScope"/>, if there is no 
    /// <see cref="ValidationContext.Current"/> validation context;
    /// otherwise, <see langword="null" />.</returns>
    public static ValidationScope Open()
    {
      if (CurrentContext==null)
        return new ValidationContext().Activate();
      else
        return null;
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="context">The context.</param>
    public ValidationScope(ValidationContext context)
      : base(context)
    {
    }
  }
}