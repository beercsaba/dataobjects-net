// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.11.26

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xtensive.Core;
using Xtensive.Core.Collections;
using Xtensive.Core.Parameters;
using Xtensive.Core.Reflection;
using Xtensive.Core.Tuples;
using Xtensive.Storage.Internals;
using Xtensive.Storage.Linq.Expressions;
using Xtensive.Storage.Linq.Rewriters;
using Xtensive.Storage.Model;
using Xtensive.Storage.Rse.Providers.Compilable;

namespace Xtensive.Storage.Linq
{
  /// <summary>
  /// Base class for <see cref="IQueryProvider"/> implementation.
  /// </summary>
  internal sealed class QueryProvider : IQueryProvider
  {
    public static QueryProvider Current = new QueryProvider();

    /// <inheritdoc/>
    IQueryable IQueryProvider.CreateQuery(Expression expression)
    {
      Type elementType = SequenceHelper.GetElementType(expression.Type);
      try {
        var query = (IQueryable) typeof (Query<>).Activate(new[] {elementType}, new object[] {expression});
        return query;
      }
      catch (TargetInvocationException tie) {
        throw tie.InnerException;
      }
    }

    /// <inheritdoc/>
    IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
    {
      return new Query<TElement>(expression);
    }

    /// <inheritdoc/>
    object IQueryProvider.Execute(Expression expression)
    {
      throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public TResult Execute<TResult>(Expression expression)
    {
      var compiled = Compile(expression);
      var compiledQueryScope = CompiledQueryScope.Current;
      if (compiledQueryScope != null)
        compiledQueryScope.Context = compiled;
      return compiled.GetResult<TResult>();
    }

    internal ResultExpression Compile(Expression expression)
    {
      var model = Domain.Demand().Model;
      ResultExpression result;
      if (expression.IsResult())
        result = (ResultExpression) expression;
      else {
        var context = new TranslatorContext(expression, model);
        result = context.Translator.Translate();
      }
      result = Optimize(result, model);
      return result;
    }

    private ResultExpression Optimize(ResultExpression origin, DomainModel model)
    {
      var mappingsGatherer = new ItemProjectorAnalyzer();

      var originProvider = origin.RecordSet.Provider;
      var usedColumns = mappingsGatherer
        .Gather(origin.ItemProjector, originProvider, model, origin.Mapping)
        .Distinct()
        .OrderBy(i => i)
        .ToList();

      if (usedColumns.Count==0)
        usedColumns.Add(0);
      if (usedColumns.Count < origin.RecordSet.Header.Length) {
        var resultProvider = new SelectProvider(originProvider, usedColumns.ToArray());

        var rs = resultProvider.Result;
        var groupMap = MappingHelper.BuildGroupMapping(usedColumns, originProvider, resultProvider);
        var mappingsReplacer = new ItemProjectorRewriter(usedColumns, groupMap, origin.RecordSet.Header);
        var itemProjector = mappingsReplacer.Rewrite(origin.ItemProjector);
        var result = new ResultExpression(origin.Type, rs, null, (LambdaExpression) itemProjector, origin.ResultType);
        return result;
      }
      return origin;
    }


    private QueryProvider()
    {
      
    }
  }
}
