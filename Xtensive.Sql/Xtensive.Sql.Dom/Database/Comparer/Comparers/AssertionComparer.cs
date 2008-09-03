// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Aleksey Gamzov
// Created:    2008.08.18

using System;

namespace Xtensive.Sql.Dom.Database.Comparer
{
  [Serializable]
  internal class AssertionComparer : NodeComparerBase<Assertion>
  {
    public override IComparisonResult<Assertion> Compare(Assertion originalNode, Assertion newNode)
    {
      var result = ComparisonContext.Current.Factory.CreateComparisonResult<Assertion, AssertionComparisonResult>(originalNode, newNode);
      bool hasChanges = false;
      result.Condition = CompareSimpleNode(originalNode==null ? null : originalNode.Condition, newNode==null ? null : newNode.Condition, ref hasChanges);
      result.IsDeferrable = CompareSimpleNode(originalNode==null ? null : originalNode.IsDeferrable, newNode==null ? null : newNode.IsDeferrable, ref hasChanges);
      result.IsInitiallyDeferred = CompareSimpleNode(originalNode==null ? null : originalNode.IsInitiallyDeferred, newNode==null ? null : newNode.IsInitiallyDeferred, ref hasChanges);
      if (hasChanges && result.ResultType==ComparisonResultType.Unchanged)
        result.ResultType = ComparisonResultType.Modified;
      return result;
    }

    public AssertionComparer(INodeComparerProvider provider)
      : base(provider)
    {
    }
  }
}