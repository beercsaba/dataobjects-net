// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Aleksey Gamzov
// Created:    2008.08.19

using System;

namespace Xtensive.Sql.Dom.Database.Comparer
{
  [Serializable]
  internal class ViewComparer : WrappingNodeComparer<View, ViewColumn, Index>
  {
    public override IComparisonResult<View> Compare(View originalNode, View newNode)
    {
      var result = ComparisonContext.Current.Factory.CreateComparisonResult<View, ViewComparisonResult>(originalNode, newNode);
      bool hasChanges = false;
      result.CheckOptions = CompareSimpleStruct(originalNode==null ? (CheckOptions?) null : originalNode.CheckOptions, newNode==null ? (CheckOptions?) null : newNode.CheckOptions, ref hasChanges);
      result.Definition = CompareSimpleNode(originalNode==null ? null : originalNode.Definition, newNode==null ? null : newNode.Definition, ref hasChanges);
      hasChanges |= CompareNestedNodes(originalNode==null ? null : originalNode.ViewColumns, newNode==null ? null : newNode.ViewColumns, BaseNodeComparer1, result.Columns);
      hasChanges |= CompareNestedNodes(originalNode==null ? null : originalNode.Indexes, newNode==null ? null : newNode.Indexes, BaseNodeComparer2, result.Indexes);
      if (hasChanges && result.ResultType==ComparisonResultType.Unchanged)
        result.ResultType = ComparisonResultType.Modified;
      return result;
    }

    public ViewComparer(INodeComparerProvider provider)
      : base(provider)
    {
    }
  }
}