// =======================================================
// WARNING! Looking below this line may explode your brain
// =======================================================

using System;
using System.Collections;
using System.Data.Common;
using NUnit.Framework;
using Xtensive.Sql.Common;
using Xtensive.Sql.Common.Mssql;
using Xtensive.Sql.Dom.Database;
using Xtensive.Sql.Dom.Database.Providers;

namespace Xtensive.Sql.Dom.Tests.MsSql
{
  public class AssertUtility
  {
    public static void AssertArraysAreEqual(Array a1, Array a2)
    {
      if (a1==a2)
        return;
      if (a1==null || a2==null)
        throw new AssertionException("One of arrays is null.");
      if (a1.Length!=a2.Length)
        throw new AssertionException("Lengths are different.");
      for (int i=0;i<a1.Length;i++)
        Assert.AreEqual(a1.GetValue(i), a2.GetValue(i));
    }

    public static void AssertCollectionsAreEqual(IEnumerable col1, IEnumerable col2)
    {
      if (col1==col2)
        return;
      if (col2==null || col1==null)
        throw new AssertionException("One of arrays is null.");
      IEnumerator enumerator1 = col1.GetEnumerator();
      IEnumerator enumerator2 = col2.GetEnumerator();
      enumerator1.Reset();
      enumerator2.Reset();
      while(enumerator1.MoveNext()) {
        if (!enumerator2.MoveNext())
          throw new AssertionException("Different count.");
        Assert.AreEqual(enumerator1.Current, enumerator2.Current);
      }
      if (enumerator2.MoveNext())
        throw new AssertionException("Different count.");
    }
  }

  public abstract class MSSQLExtractorTestBase
  {
    public static string ConnectionString = TestUrl.MsSql2005;

    public virtual string CleanUpScript
    {
      get {return null;}
    }
    
    protected SqlConnection CreateConnection(string ConnectString)
    {
      var driver = new Mssql.v2005.MssqlDriver(new MssqlVersionInfo((new Version(9, 0))));
      var connection = (SqlConnection)driver.CreateConnection(new ConnectionInfo(ConnectionString));
      connection.Open();
      return connection;
    }

    protected void ExecuteQuery(string sqlQuery, SqlConnection connection)
    {
      DbCommand command = connection.RealConnection.CreateCommand();
      command.CommandText = sqlQuery;
      command.ExecuteNonQuery();
    }

    protected void ExecuteQuery(string sqlQuery, string ConnectString)
    {
      if (string.IsNullOrEmpty(sqlQuery))
        return;
      using (SqlConnection connection = CreateConnection(ConnectString)) {
        ExecuteQuery(sqlQuery, connection);
      }
    }

    protected virtual Model ExtractModel(string ConnectString)
    {
      var smd = new SqlModelProvider(CreateConnection(ConnectString));
      return Model.Build(smd);
    }

    [TestFixtureTearDown]
    public virtual void TearDown()
    {
      ExecuteQuery(CleanUpScript, ConnectionString);
    }
  }

  public class MSSQLExtractor_TestSchemaExtraction : MSSQLExtractorTestBase
  {

    public override string CleanUpScript
    {
      get {
        return 
          "\n drop table role1.table1" +
            "\n drop table role2.table2" +
              "\n drop table role3.table3" +
                "\n drop table role3.table31" +
                  "\n exec sp_droprole role1" +
                    "\n exec sp_droprole role2" +
                      "\n exec sp_droprole role3"; ;
      }
    }

    [Test]
    public virtual void Main()
    {
      ExecuteQuery(
        " exec sp_addrole 'role1'" + 
          "\n exec sp_addrole 'role2'" +
            "\n exec sp_addrole 'role3'" +
              "\n create table role1.table1(test int, test2 int, test3 int)" +
                "\n create table role2.table2(test int, test2 int, test3 int)" +
                  "\n create table role3.table3(test int, test2 int, test3 int)" + 
                    "\n create table role3.table31(test int, test2 int, test3 int)", ConnectionString);
      SqlModelProvider smd = new SqlModelProvider(CreateConnection(ConnectionString));
      Model model = Model.Build(smd);

      // Validating.
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role1"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role1"].Tables["table1"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role1"].Tables["table1"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role1"].Tables["table1"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role1"].Tables["table1"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["role1"].Tables.Count==1);

      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role2"].Tables["table2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role2"].Tables["table2"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role2"].Tables["table2"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role2"].Tables["table2"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["role2"].Tables.Count==1);

      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role3"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role3"].Tables["table3"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role3"].Tables["table3"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role3"].Tables["table3"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["role3"].Tables["table3"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["role3"].Tables.Count==2);
    }
  }

  public class MSSQLExtractor_TestColumnTypeExtraction : MSSQLExtractorTestBase
  {
    public override string CleanUpScript
    {
      get { return "drop table dataTypesTestTable"; }
    }

    [Test]
    public void Main()
    {
      string createTableQuery = 
        "CREATE TABLE dataTypesTestTable (" + 
          "[int_l4] [int] NULL ," + 
            "[binary_l50] [binary] (50) NULL ," + 
              "[bit_l1] [bit] NULL , " + 
                "[char_10] [char] (10) COLLATE Cyrillic_General_CI_AS NULL ," + 
                  "[datetime_l8] [datetime] NULL ," + 
                    "[decimal_p18_s0] [decimal](18, 0) NULL ," + 
                      "[decimal_p12_s11_l9] [decimal](12, 11) NULL ," + 
                        "[float_p53] [float] NULL ," + 
                          "[image_16] [image] NULL ," + 
                            "[money_p19_s4_l8] [money] NULL ," + 
                              "[nchar_l100] [nchar] (100) COLLATE Cyrillic_General_CI_AS NULL ," + 
                                "[ntext] [ntext] COLLATE Cyrillic_General_CI_AS NULL ," + 
                                  "[numeric_p5_s5] [numeric](5, 5) NULL ," + 
                                    "[nvarchar_l50] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," + 
                                      "[real_p24_s0_l4] [real] NULL ," + 
                                        "[smalldatetime_l4] [smalldatetime] NULL ," + 
                                          "[smallint_l2] [smallint] NULL ," + 
                                            "[small_money_p10_s4_l4] [smallmoney] NULL ," + 
                                              "[sql_variant_] [sql_variant] NULL ," + 
                                                "[text_16] [text] COLLATE Cyrillic_General_CI_AS NULL ," + 
                                                  "[timestamp_l8] [timestamp] NULL ," + 
                                                    "[tinyint_1_p3_s0_l1] [tinyint] NULL ," + 
                                                      "[uniqueidentifier_l16] [uniqueidentifier] NULL ," + 
                                                        "[varbinary_l150] [varbinary] (150) NULL ," + 
                                                          "[varchar_l50] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL)";
      ExecuteQuery(createTableQuery, ConnectionString);
      Model model = ExtractModel( ConnectionString);

      Table table = model.DefaultServer.DefaultCatalog.DefaultSchema.Tables["dataTypesTestTable"];
      Assert.IsTrue(table.TableColumns["int_l4"].DataType.DataType==SqlDataType.Int32);
      Assert.IsTrue(table.TableColumns["binary_l50"].DataType.Size==50);
      Assert.IsTrue(table.TableColumns["binary_l50"].DataType.DataType==SqlDataType.Binary);
      Assert.IsTrue(table.TableColumns["bit_l1"].DataType.DataType==SqlDataType.Boolean);
      Assert.IsTrue(table.TableColumns["char_10"].DataType.Size==10);
      Assert.IsTrue(table.TableColumns["char_10"].DataType.DataType==SqlDataType.AnsiChar);
      Assert.IsTrue(table.TableColumns["datetime_l8"].DataType.DataType==SqlDataType.DateTime);
      Assert.IsTrue(table.TableColumns["decimal_p18_s0"].DataType.DataType==SqlDataType.Decimal);
      Assert.IsTrue(table.TableColumns["decimal_p18_s0"].DataType.Precision==18);
      Assert.IsTrue(table.TableColumns["decimal_p18_s0"].DataType.Scale==0);
      Assert.IsTrue(table.TableColumns["decimal_p12_s11_l9"].DataType.DataType==SqlDataType.Decimal);
      Assert.IsTrue(table.TableColumns["decimal_p12_s11_l9"].DataType.Precision==12);
      Assert.IsTrue(table.TableColumns["decimal_p12_s11_l9"].DataType.Scale==11);
      Assert.IsTrue(table.TableColumns["float_p53"].DataType.DataType==SqlDataType.Double);
      Assert.IsTrue(table.TableColumns["float_p53"].DataType.Precision==53);
      Assert.IsTrue(table.TableColumns["image_16"].DataType.DataType==SqlDataType.Image);
      Assert.IsTrue(table.TableColumns["money_p19_s4_l8"].DataType.DataType==SqlDataType.Money);
      Assert.IsTrue(table.TableColumns["money_p19_s4_l8"].DataType.Precision==19);
      Assert.IsTrue(table.TableColumns["money_p19_s4_l8"].DataType.Scale==4);
      Assert.IsTrue(table.TableColumns["nchar_l100"].DataType.DataType==SqlDataType.Char);
      Assert.IsTrue(table.TableColumns["nchar_l100"].DataType.Size==100);
      Assert.IsTrue(table.TableColumns["ntext"].DataType.DataType==SqlDataType.Text);
      Assert.IsTrue(table.TableColumns["numeric_p5_s5"].DataType.DataType==SqlDataType.Decimal);
      Assert.IsTrue(table.TableColumns["numeric_p5_s5"].DataType.Precision==5);
      Assert.IsTrue(table.TableColumns["nvarchar_l50"].DataType.DataType==SqlDataType.VarChar);
      Assert.IsTrue(table.TableColumns["nvarchar_l50"].DataType.Size==50);
      Assert.IsTrue(table.TableColumns["real_p24_s0_l4"].DataType.DataType==SqlDataType.Float);
      Assert.IsTrue(table.TableColumns["real_p24_s0_l4"].DataType.Precision==24);
      Assert.IsTrue(table.TableColumns["real_p24_s0_l4"].DataType.Scale==0);
      Assert.IsTrue(table.TableColumns["smalldatetime_l4"].DataType.DataType==SqlDataType.SmallDateTime);
      Assert.IsTrue(table.TableColumns["smallint_l2"].DataType.DataType==SqlDataType.Int16);
      Assert.IsTrue(table.TableColumns["small_money_p10_s4_l4"].DataType.DataType==SqlDataType.SmallMoney);
      Assert.IsTrue(table.TableColumns["sql_variant_"].DataType.DataType==SqlDataType.Variant);
      Assert.IsTrue(table.TableColumns["text_16"].DataType.DataType==SqlDataType.AnsiText);
      Assert.IsTrue(table.TableColumns["timestamp_l8"].DataType.DataType==SqlDataType.TimeStamp);
      Assert.IsTrue(table.TableColumns["tinyint_1_p3_s0_l1"].DataType.DataType==SqlDataType.Byte);
      Assert.IsTrue(table.TableColumns["uniqueidentifier_l16"].DataType.DataType==SqlDataType.Guid);
      Assert.IsTrue(table.TableColumns["varbinary_l150"].DataType.DataType==SqlDataType.VarBinary);
      Assert.IsTrue(table.TableColumns["varbinary_l150"].DataType.Size==150);
      Assert.IsTrue(table.TableColumns["varchar_l50"].DataType.DataType==SqlDataType.AnsiVarChar);
      Assert.IsTrue(table.TableColumns["varchar_l50"].DataType.Size==50);
    }
  }

  public class MSSQLExtractor_TestExtractingViews : MSSQLExtractorTestBase
  {
    public override string CleanUpScript
    {
      get {
        return "\n drop table role1.table1" + 
          "\n drop view role1.view1" + 
            "\n drop view role1.view2" + 
              "\n exec sp_droprole role1";
      }
    }

    [Test]
    public virtual void Main()
    {
      ExecuteQuery(
        " EXEC sp_addrole 'role1'" +
          "\n CREATE TABLE role1.table1(column1 int, column2 int)", ConnectionString);

      ExecuteQuery(
        "CREATE VIEW role1.view1 " +
          "\n as Select column1 From role1.table1", ConnectionString);

      ExecuteQuery(
        "CREATE VIEW role1.view2 " +
          "\n as Select column1, column2 From role1.table1", ConnectionString);

      SqlConnection connection = CreateConnection(ConnectionString);
      SqlModelProvider smd = new SqlModelProvider(connection);
      Model model = Model.Build(smd);
      Schema schema = model.DefaultServer.DefaultCatalog.Schemas["role1"];

      Assert.IsNotNull(schema);
      Assert.IsNotNull(schema.Views["view1"]);
      Assert.IsNotNull(schema.Views["view2"]);
      Assert.IsNotNull(schema.Views["view1"].ViewColumns["column1"]);
      Assert.IsNotNull(schema.Views["view2"].ViewColumns["column1"]);
      Assert.IsNotNull(schema.Views["view2"].ViewColumns["column2"]);
    }
  }

  public class MSSQLExtractor_TestExtractingForeignKeys : MSSQLExtractorTestBase
  {
    public override string CleanUpScript
    {
      get
      {
        return
          "   drop table A1" +
            "\n drop table A2" +
              "\n drop table A3" + 
                "\n drop table B1" +
                  "\n drop table B2" + 
                    "\n drop table B3";
      }
    }

    [Test]
    public void Main()
    {
      string query = "\n create table B1 (b_id int primary key)" +
        "\n create table A1 (b_id int references B1(b_id))" +
          "\n create table B2 (" +
            "\n   b_id_1 int, " +
              "\n   b_id_2 int, " +
                "\n   CONSTRAINT [B2_PK] PRIMARY KEY  CLUSTERED (b_id_1, b_id_2)  ON [PRIMARY])" +
                  "\n create table A2 (" +
                    "\n   b_id_1 int,    " +
                      "\n   b_id_2 int,    " +
                        "\n   constraint [A2_FK] FOREIGN KEY (b_id_1, b_id_2) " +
                          "\n                    REFERENCES B2 (b_id_1, b_id_2) ON DELETE CASCADE ON UPDATE NO ACTION)" +
                            "\n create table B3 (" + 
                              "\n   b_id_1 int," +
                                "\n   b_id_2 int," +
                                  "\n   b_id_3 int," +
                                    "\n   CONSTRAINT [B3_PK] PRIMARY KEY  CLUSTERED (b_id_1, b_id_2, b_id_3)  ON [PRIMARY])" +
                                      "\n create table A3 (" +
                                        "\n   A_col1 int," +
                                          "\n   b_id_3 int," +
                                            "\n   b_id_1 int," +
                                              "\n   b_id_2 int," +
                                                "\n   constraint [A3_FK] FOREIGN KEY (b_id_1, b_id_2, b_id_3) " +
                                                  "\n                    REFERENCES B3 (b_id_1, b_id_2, b_id_3) ON DELETE NO ACTION ON UPDATE CASCADE)";
      ExecuteQuery(query, ConnectionString);


      Model model = ExtractModel( ConnectionString);
      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;

      // Validating.
      ForeignKey fk1 = (ForeignKey)schema.Tables["A1"].TableConstraints[0];
      Assert.IsNotNull(fk1);
      Assert.IsTrue(fk1.Columns[0].Name=="b_id");
      Assert.IsTrue(fk1.ReferencedColumns[0].Name=="b_id");
      Assert.IsTrue(fk1.ReferencedColumns.Count==1);
      Assert.IsTrue(fk1.Columns.Count==1);

      ForeignKey fk2 = (ForeignKey)schema.Tables["A2"].TableConstraints[0];
      Assert.IsNotNull(fk1);
      Assert.IsTrue(fk2.Name=="A2_FK");
      Assert.IsTrue(fk2.Columns[0].Name=="b_id_1");
      Assert.IsTrue(fk2.ReferencedColumns[0].Name=="b_id_1");
      Assert.IsTrue(fk2.Columns[1].Name=="b_id_2");
      Assert.IsTrue(fk2.ReferencedColumns[1].Name=="b_id_2");
      Assert.IsTrue(fk2.ReferencedColumns.Count==2);
      Assert.IsTrue(fk2.Columns.Count==2);
      Assert.IsTrue(fk2.OnDelete==ReferentialAction.Cascade);
      Assert.IsTrue(fk2.OnUpdate==ReferentialAction.NoAction);

      ForeignKey fk3 = (ForeignKey)schema.Tables["A3"].TableConstraints[0];
      Assert.IsNotNull(fk3);
      Assert.IsTrue(fk3.Name=="A3_FK");
      Assert.IsTrue(fk3.Columns[0].Name=="b_id_1");
      Assert.IsTrue(fk3.ReferencedColumns[0].Name=="b_id_1");
      Assert.IsTrue(fk3.Columns[1].Name=="b_id_2");
      Assert.IsTrue(fk3.ReferencedColumns[1].Name=="b_id_2");
      Assert.IsTrue(fk3.Columns[2].Name=="b_id_3");
      Assert.IsTrue(fk3.ReferencedColumns[2].Name=="b_id_3");
      Assert.IsTrue(fk3.ReferencedColumns.Count==3);
      Assert.IsTrue(fk3.Columns.Count==3);
      Assert.IsTrue(fk3.OnDelete==ReferentialAction.NoAction);
      Assert.IsTrue(fk3.OnUpdate==ReferentialAction.Cascade);
    }
  }

  public class MSSQLExtractor_TestExtractingUniqueConstraints : MSSQLExtractorTestBase
  {
    public override string CleanUpScript
    {
      get { return "drop table A"; }
    }
    
    [Test]
    public void Main()
    {
      ExecuteQuery(
        "   Create table A (" +
          "\n col_11 int, col_12 int, col_13 int," +
            "\n col_21 int, col_22 int, col_23 int," +
              "\n CONSTRAINT A_UNIQUE_1 UNIQUE(col_11,col_12,col_13)," +
                "\n CONSTRAINT A_UNIQUE_2 UNIQUE(col_21,col_22,col_23))", ConnectionString);

      Model model = ExtractModel( ConnectionString);
      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;

      // Validating.
      UniqueConstraint A_UNIQUE_1 = (UniqueConstraint)schema.Tables["A"].TableConstraints["A_UNIQUE_1"];
      Assert.IsNotNull(A_UNIQUE_1);
      Assert.IsTrue(A_UNIQUE_1.Columns[0].Name=="col_11");
      Assert.IsTrue(A_UNIQUE_1.Columns[1].Name=="col_12");
      Assert.IsTrue(A_UNIQUE_1.Columns[2].Name=="col_13");
      Assert.IsTrue(A_UNIQUE_1.Columns.Count==3);

      UniqueConstraint A_UNIQUE_2 = (UniqueConstraint)schema.Tables["A"].TableConstraints["A_UNIQUE_2"];
      Assert.IsNotNull(A_UNIQUE_2);
      Assert.IsTrue(A_UNIQUE_2.Columns[0].Name=="col_21");
      Assert.IsTrue(A_UNIQUE_2.Columns[1].Name=="col_22");
      Assert.IsTrue(A_UNIQUE_2.Columns[2].Name=="col_23");
      Assert.IsTrue(A_UNIQUE_2.Columns.Count==3);
    }
  }

  public class MSSQLExtractor_TestIndexesExtracted : MSSQLExtractorTestBase
  {
    public override string CleanUpScript
    {
      get { return "drop table table1"; }
    }

    [Test]
    public virtual void Main()
    {
      ExecuteQuery(
        " create table table1 (" + 
          "\n column1 int, " +
            "\n column2 int) " + 
              "\n create index        table1_index1_desc_asc   on table1 (column1 desc, column2 asc)"  +
                "\n create unique index table1_index1_u_asc_desc on table1 (column1 asc, column2 desc)", ConnectionString);
      Model model = ExtractModel(ConnectionString);
      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;
      
      Assert.IsTrue(schema.Tables["table1"]!=null);
      Assert.IsNotNull(schema.Tables["table1"].Indexes["table1_index1_desc_asc"]);
      Assert.IsTrue(schema.Tables["table1"].Indexes["table1_index1_desc_asc"].Columns.Count==2);
      Assert.IsTrue(schema.Tables["table1"].Indexes["table1_index1_desc_asc"].Columns[0].Name=="column1");
      Assert.IsTrue(!schema.Tables["table1"].Indexes["table1_index1_desc_asc"].Columns[0].Ascending);
      Assert.IsTrue( schema.Tables["table1"].Indexes["table1_index1_desc_asc"].Columns[1].Ascending);

      Assert.IsNotNull(schema.Tables["table1"].Indexes["table1_index1_u_asc_desc"]);
      Assert.IsTrue(schema.Tables["table1"].Indexes["table1_index1_u_asc_desc"].Columns.Count==2);
      Assert.IsTrue( schema.Tables["table1"].Indexes["table1_index1_u_asc_desc"].Columns[0].Ascending);
      Assert.IsTrue(!schema.Tables["table1"].Indexes["table1_index1_u_asc_desc"].Columns[1].Ascending);
    }
  }

  [TestFixture]
  public class MSSQLExtractor2005_TestSchemaExtraction : MSSQLExtractor_TestSchemaExtraction
  {
    public override string CleanUpScript
    {
      get {
        return 
          "\n drop table schema1.table1" +
            "\n drop table schema2.table2" +
              "\n drop table schema3.table3" +
                "\n drop table schema3.table31" +
                  "\n drop schema schema1" +
                    "\n drop schema schema2" +
                      "\n drop schema schema3"; ;
      }
    }

    [Test]
    public override void Main()
    {
      ExecuteQuery(" create schema schema1", ConnectionString);
      ExecuteQuery(" create schema schema2", ConnectionString);
      ExecuteQuery(" create schema schema3", ConnectionString);
      string createTablesSql =
        "\n create table schema1.table1(test int, test2 int, test3 int)" +
          "\n create table schema2.table2(test int, test2 int, test3 int)" +
            "\n create table schema3.table3(test int, test2 int, test3 int)" + 
              "\n create table schema3.table31(test int, test2 int, test3 int)";
      ExecuteQuery(createTablesSql, ConnectionString);

      Model model = ExtractModel( ConnectionString);

      // Validating.
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema1"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema1"].Tables["table1"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema1"].Tables["table1"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema1"].Tables["table1"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema1"].Tables["table1"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["schema1"].Tables.Count==1);

      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema2"].Tables["table2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema2"].Tables["table2"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema2"].Tables["table2"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema2"].Tables["table2"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["schema2"].Tables.Count==1);

      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema3"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema3"].Tables["table3"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema3"].Tables["table3"].TableColumns["test"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema3"].Tables["table3"].TableColumns["test2"]);
      Assert.IsNotNull(model.DefaultServer.DefaultCatalog.Schemas["schema3"].Tables["table3"].TableColumns["test3"]);
      Assert.IsTrue(model.DefaultServer.DefaultCatalog.Schemas["schema3"].Tables.Count==2);
    }
  }

  [TestFixture]
  public class MSSQLExtractor2005_TestColumnTypeExtraction : MSSQLExtractor_TestColumnTypeExtraction
  {
    /*
    public override string CleanUpScript
    {
      get { return base.CleanUpScript + "\n drop table dataTypesTestTable2"; }
    }

    public void Main2()
    {
      ExecuteQuery(
        "create table dataTypesTestTable2(" +
          "\n xml_column xml," +
            "\n varbinary_max varbinary(max)," +
              "\n nvarchar_max nvarchar(max)," +
                "\n varchar_max varchar(max))", ConnectionString);
      Model model = ExtractModel(ConnectionString);

      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;
      Assert.IsNotNull(schema.Tables["dataTypesTestTable2"]);
      Assert.IsTrue(schema.Tables["dataTypesTestTable2"].TableColumns["varbinary_max"].DataType.DataType == SqlDataType.VarBinaryMax);
      Assert.IsTrue(schema.Tables["dataTypesTestTable2"].TableColumns["nvarchar_max"].DataType.DataType == SqlDataType.VarCharMax);
      Assert.IsTrue(schema.Tables["dataTypesTestTable2"].TableColumns["varchar_max"].DataType.DataType == SqlDataType.VarCharMax);
      Assert.IsTrue(schema.Tables["dataTypesTestTable2"].TableColumns["xml_column"].DataType.DataType == SqlDataType.Xml);
    }
    */
  }

  [TestFixture]
  public class MSSQLExtractor2005_TestExtractingViews : MSSQLExtractor_TestExtractingViews
  {

  }

  [TestFixture]
  public class MSSQLExtractor2005_TestExtractingForeignKeys : MSSQLExtractor_TestExtractingForeignKeys
  {

  }

  [TestFixture]
  public class MSSQLExtractor2005_TestExtractingForeignKeys2 : MSSQLExtractorTestBase
  {

    public override string CleanUpScript
    {
      get { return
        "\n Drop Table A" +
          "\n Drop Table B" +
            "\n Drop Table B2";
      }
    }

    [Test]
    public void Main()
    {
      ExecuteQuery(
        "   Create Table B (b_id int primary key)" +
          "\n Create Table B2(b_id_1 int primary key)" +
            "\n Create Table A(" + 
              "\n b_id   int ," + 
                "\n b_id_1 int ," +
                  "\n Constraint [A_FK_1] Foreign key(b_id) references B(b_id) ON DELETE SET NULL ," +
                    "\n Constraint [A_FK_2] Foreign key(b_id_1) references B2(b_id_1) ON DELETE SET DEFAULT" +
                      "\n )", ConnectionString);
      Model model = base.ExtractModel( ConnectionString);
      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;
      Assert.IsTrue(((ForeignKey)schema.Tables["A"].TableConstraints["A_FK_1"]).OnDelete == ReferentialAction.SetNull);
      Assert.IsTrue(((ForeignKey)schema.Tables["A"].TableConstraints["A_FK_2"]).OnDelete == ReferentialAction.SetDefault);
    }
  }

  [TestFixture]
  public class MSSQLExtractor2005_TestExtractingUniqueConstraints : MSSQLExtractor_TestExtractingUniqueConstraints
  {

  }

  [TestFixture]
  public class MSSQLExtractor2005_TestIndexesExtracted : MSSQLExtractor_TestIndexesExtracted
  {

  }

  public class MSSQLExtractor2005_TestPartitionsExtracted : MSSQLExtractorTestBase
  {

    public override string CleanUpScript
    {
      get { 
        return "IF DB_ID (N'MSSQL2005Extr_PartitionsTest') IS NOT NULL" +
          "\n DROP DATABASE MSSQL2005Extr_PartitionsTest;"; 
      }
    }

    public void Main()
    {
      SqlConnection connection = CreateConnection( ConnectionString);
      ExecuteQuery("USE master;", connection);
      string createTestDatabaseSql = @"-- Get the SQL Server data path
      DECLARE @data_path nvarchar(256);
      SET @data_path = (SELECT SUBSTRING(physical_name, 1, CHARINDEX(N'master.mdf', LOWER(physical_name)) - 1)
                        FROM master.sys.master_files
                        WHERE database_id = 1 AND file_id = 1);

      -- execute the CREATE DATABASE statement 
      EXECUTE ('CREATE DATABASE  MSSQL2005Extr_PartitionsTest
      ON PRIMARY
      ( NAME = SPri1_dat,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_P.mdf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 15% ),
      ( NAME = SPri2_dat, 
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_S.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 15% ),
      FILEGROUP MSSQL2005Extr_PartitionsTest_FG1
      ( NAME = MSSQL2005Extr_PartitionsTest_D11,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D11.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 ),
      ( NAME = MSSQL2005Extr_PartitionsTest_D12,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D12.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 ),
      FILEGROUP MSSQL2005Extr_PartitionsTest_FG2
      ( NAME = MSSQL2005Extr_PartitionsTest_D21,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D21.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 ),
      ( NAME = MSSQL2005Extr_PartitionsTest_D22,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D22.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 ),
      FILEGROUP MSSQL2005Extr_PartitionsTest_FG3
      ( NAME = MSSQL2005Extr_PartitionsTest_D31,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D31.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 ),
      ( NAME = MSSQL2005Extr_PartitionsTest_D32,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_D32.ndf'',
          SIZE = 10,
          MAXSIZE = 50,
          FILEGROWTH = 5 )
      LOG ON
      ( NAME = Sales_log,
          FILENAME = '''+ @data_path + 'MSSQL2005Extr_PartitionsTest_Log.ldf'',
          SIZE = 5MB,
          MAXSIZE = 25MB,
          FILEGROWTH = 5MB )'
      );";
      ExecuteQuery(createTestDatabaseSql, connection);
      ExecuteQuery("use MSSQL2005Extr_PartitionsTest", ConnectionString);
      
      // Create partition function.
      ExecuteQuery(
        "CREATE PARTITION FUNCTION MSSQL2005Extr_PartitionsTest_PFA_LEFT_1_20_30_40 (int)" + 
          "\n AS RANGE LEFT FOR VALUES (500);", connection);

      // Create partition scheme.
      ExecuteQuery(
        "CREATE PARTITION SCHEME MSSQL2005Extr_PartitionsTest_PFA_Schema" + 
          "\n AS PARTITION MSSQL2005Extr_PartitionsTest_PFA_LEFT_1_20_30_40" +
            "\n TO ( " + 
              "\n MSSQL2005Extr_PartitionsTest_FG1, " + 
                "\n MSSQL2005Extr_PartitionsTest_FG2);"
        ,connection);

      // Create partitioned tables
      ExecuteQuery(
        "CREATE TABLE MSSQL2005Extr_PartitionsTest_Table (col1 int, col2 char(10))" + 
          "\n ON MSSQL2005Extr_PartitionsTest_PFA_Schema (col1)", connection) ;
      
      ExecuteQuery(
        "CREATE TABLE MSSQL2005Extr_PartitionsTest_Table2 (col1 int, col2 int)" +
          "ON MSSQL2005Extr_PartitionsTest_PFA_Schema (col2) ;", ConnectionString);

      Model model = ExtractModel( ConnectionString);
      Schema schema = model.DefaultServer.DefaultCatalog.DefaultSchema;

      Assert.IsNotNull(schema.Tables["MSSQL2005Extr_PartitionsTest_Table"].PartitionDescriptor);
      
      Assert.IsTrue(
        schema.Tables["MSSQL2005Extr_PartitionsTest_Table"]
          .PartitionDescriptor
          .PartitionSchema.Name=="MSSQL2005Extr_PartitionsTest_PFA_Schema");

      Assert.IsTrue(
        schema.Tables["MSSQL2005Extr_PartitionsTest_Table"]
          .PartitionDescriptor
          .PartitionSchema
          .PartitionFunction
          .BoundaryType==BoundaryType.Left);

      AssertUtility.AssertArraysAreEqual(
        schema.Tables["MSSQL2005Extr_PartitionsTest_Table"]
          .PartitionDescriptor
          .PartitionSchema
          .PartitionFunction
          .BoundaryValues,
        new string[] {"0", "500"});

      AssertUtility.AssertCollectionsAreEqual(
        schema.Tables["MSSQL2005Extr_PartitionsTest_Table"]
          .PartitionDescriptor
          .PartitionSchema
          .Filegroups,
        new string[] { "MSSQL2005Extr_PartitionsTest_FG1", "MSSQL2005Extr_PartitionsTest_FG2" });

      Assert.IsTrue(schema.Tables["MSSQL2005Extr_PartitionsTest_Table"]
        .PartitionDescriptor
        .PartitionMethod == PartitionMethod.Range);
    }
  }
}