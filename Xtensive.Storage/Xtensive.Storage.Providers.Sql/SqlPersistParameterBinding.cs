// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.09.25

using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Sql.ValueTypeMapping;

namespace Xtensive.Storage.Providers.Sql
{
  public sealed class SqlPersistParameterBinding : SqlParameterBinding
  {
    /// <summary>
    /// Gets the type of the binding.
    /// </summary>
    public SqlPersistParameterBindingType BindingType { get; private set; }

    /// <summary>
    /// Gets the index of the field to extract value from.
    /// </summary>
    public int FieldIndex { get; private set; }

    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="fieldIndex">Index of the field that contain new value.</param>
    /// <param name="typeMapping">The type mapping.</param>
    public SqlPersistParameterBinding(int fieldIndex, TypeMapping typeMapping)
      : base(typeMapping)
    {
      FieldIndex = fieldIndex;
      BindingType = SqlPersistParameterBindingType.Regular;
    }

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="fieldIndex">Index of the field that contain new value.</param>
    /// <param name="typeMapping">The type mapping.</param>
    /// <param name="bindingType">Type of the binding.</param>
    public SqlPersistParameterBinding(int fieldIndex, TypeMapping typeMapping, SqlPersistParameterBindingType bindingType)
      : base(typeMapping)
    {
      FieldIndex = fieldIndex;
      BindingType = bindingType;
    }
  }
}