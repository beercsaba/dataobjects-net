// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.07.02

using System;
using System.Data;
using System.Data.Common;
using Xtensive.Sql.Info;

namespace Xtensive.Sql.SqlServer
{
  internal class DataAccessHandler : ValueTypeMapping.DataAccessHandler
  {
    private DateTime MinDateTimeValue;

    public override bool IsLiteralCastRequired(Type type)
    {
      switch (Type.GetTypeCode(type)) {
      case TypeCode.Byte:
      case TypeCode.SByte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int64:
      case TypeCode.UInt64:
//      case TypeCode.Single:
//      case TypeCode.Double:
//      case TypeCode.DateTime:
        return true;
      }
      if (type==typeof(TimeSpan))
        return true;
      if (type==typeof(Guid))
        return true;
      return false;
    }

    public override void SetSByteParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.Int16;
      parameter.Value = value ?? DBNull.Value;
    }

    public override void SetUShortParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.Int32;
      parameter.Value = value ?? DBNull.Value;
    }

    public override void SetUIntParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.Int64;
      parameter.Value = value ?? DBNull.Value;
    }

    public override void SetULongParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.Decimal;
      parameter.Value = value ?? DBNull.Value;
    }

    public override void SetDateTimeParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.DateTime;
      if (value==null) {
        parameter.Value = DBNull.Value;
        return;
      }
      var dateTime = (DateTime) value;
      parameter.Value = dateTime > MinDateTimeValue ? value : MinDateTimeValue;
    }

    public override void SetTimeSpanParameterValue(DbParameter parameter, object value)
    {
      parameter.DbType = DbType.Int64;
      if (value==null) {
        parameter.Value = DBNull.Value;
        return;
      }
      var timeSpan = (TimeSpan) value;
      parameter.Value = (long) timeSpan.TotalMilliseconds;
    }

    public override SqlValueType BuildSByteSqlType(int? length, int? precision, int? scale)
    {
      return new SqlValueType(SqlType.Int16);
    }

    public override SqlValueType BuildUShortSqlType(int? length, int? precision, int? scale)
    {
      return new SqlValueType(SqlType.Int32);
    }

    public override SqlValueType BuildUIntSqlType(int? length, int? precision, int? scale)
    {
      return new SqlValueType(SqlType.Int64);
    }

    public override SqlValueType BuildULongSqlType(int? length, int? precision, int? scale)
    {
      return new SqlValueType(SqlType.Decimal, 20, 0);
    }

    public override SqlValueType BuildTimeSpanSqlType(int? length, int? precision, int? scale)
    {
      return new SqlValueType(SqlType.Int64);
    }

    public override object ReadTimeSpan(DbDataReader reader, int index)
    {
      return TimeSpan.FromMilliseconds(reader.GetInt64(index));
    }

    public override void Initialize()
    {
      base.Initialize();
      MinDateTimeValue = ((ValueRange<DateTime>) Driver.ServerInfo.DataTypes.DateTime.ValueRange).MinValue;
    }

    // Constructors

    public DataAccessHandler(SqlDriver driver)
      : base(driver)
    {
    }
  }
}