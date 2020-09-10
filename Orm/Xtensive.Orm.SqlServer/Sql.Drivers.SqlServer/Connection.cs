// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.08.11

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using SqlServerConnection = System.Data.SqlClient.SqlConnection;

namespace Xtensive.Sql.Drivers.SqlServer
{
  internal class Connection : SqlConnection
  {
    private const string DefaultCheckConnectionQuery = "SELECT TOP(0) 0;";

    private SqlServerConnection underlyingConnection;
    private SqlTransaction activeTransaction;
    private bool checkConnectionIsAlive;

    /// <inheritdoc/>
    public override DbConnection UnderlyingConnection { get { return underlyingConnection; } }

    /// <inheritdoc/>
    public override DbTransaction ActiveTransaction { get { return activeTransaction; } }

    /// <inheritdoc/>
    public override DbParameter CreateParameter()
    {
      return new SqlParameter();
    }

    /// <inheritdoc/>
    public override void Open()
    {
      if (!checkConnectionIsAlive)
        base.Open();
      else
        OpenWithCheck(DefaultCheckConnectionQuery);
    }

    /// <inheritdoc/>
    public override Task OpenAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (!checkConnectionIsAlive)
        return base.OpenAsync(cancellationToken);
      else
        return OpenWithCheckAsync(DefaultCheckConnectionQuery, cancellationToken);
    }

    /// <inheritdoc/>
    public override void OpenAndInitialize(string initializationScript)
    {
      if (!checkConnectionIsAlive) {
        base.OpenAndInitialize(initializationScript);
        return;
      }

      var script = string.IsNullOrEmpty(initializationScript.Trim())
        ? DefaultCheckConnectionQuery
        : initializationScript;
      OpenWithCheck(script);
    }

    /// <inheritdoc/>
    public override Task OpenAndInitializeAsync(string initializationScript, CancellationToken cancellationToken)
    {
      if (!checkConnectionIsAlive)
        return base.OpenAndInitializeAsync(initializationScript, cancellationToken);

      var script = string.IsNullOrEmpty(initializationScript.Trim())
        ? DefaultCheckConnectionQuery
        : initializationScript;
      return OpenWithCheckAsync(script, cancellationToken);
    }

    /// <inheritdoc/>
    public override void BeginTransaction()
    {
      EnsureIsNotDisposed();
      EnsureTransactionIsNotActive();
      activeTransaction = underlyingConnection.BeginTransaction();
    }

    /// <inheritdoc/>
    public override void BeginTransaction(IsolationLevel isolationLevel)
    {
      EnsureIsNotDisposed();
      EnsureTransactionIsNotActive();
      activeTransaction = underlyingConnection.BeginTransaction(isolationLevel);
    }

    /// <inheritdoc/>
    public override void MakeSavepoint(string name)
    {
      EnsureIsNotDisposed();
      EnsureTransactionIsActive();
      activeTransaction.Save(name);
    }

    /// <inheritdoc/>
    public override void RollbackToSavepoint(string name)
    {
      EnsureIsNotDisposed();
      EnsureTransactionIsActive();
      activeTransaction.Rollback(name);
    }

    /// <inheritdoc/>
    public override void ReleaseSavepoint(string name)
    {
      EnsureIsNotDisposed();
      EnsureTransactionIsActive();
      // nothing
    }

    protected override void ClearUnderlyingConnection()
    {
      underlyingConnection = null;
    }

    /// <inheritdoc/>
    protected override void ClearActiveTransaction()
    {
      activeTransaction = null;
    }

    private void OpenWithCheck(string checkQueryString)
    {
      bool connectionChecked = false;
      bool restoreTriggered = false;
      while (!connectionChecked) {
        base.Open();
        try {
          using (var command = underlyingConnection.CreateCommand()) {
            command.CommandText = checkQueryString;
            command.ExecuteNonQuery();
          }
          connectionChecked = true;
        }
        catch (Exception exception) {
          if (InternalHelpers.ShouldRetryOn(exception)) {
            if (restoreTriggered)
              throw;

            var newConnection = new SqlServerConnection(underlyingConnection.ConnectionString);
            try {
              underlyingConnection.Close();
              underlyingConnection.Dispose();
            }
            catch { }

            underlyingConnection = newConnection;
            restoreTriggered = true;
            continue;
          }
          else
            throw;
        }
      }
    }

    private async Task OpenWithCheckAsync(string checkQueryString, CancellationToken cancellationToken)
    {
      bool connectionChecked = false;
      bool restoreTriggered = false;

      while (!connectionChecked) {
        cancellationToken.ThrowIfCancellationRequested();
        await base.OpenAsync(cancellationToken).ConfigureAwait(false);
        try {
          using (var command = underlyingConnection.CreateCommand()) {
            command.CommandText = checkQueryString;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
          }
          connectionChecked = true;
        }
        catch (Exception exception) {
          if (InternalHelpers.ShouldRetryOn(exception)) {
            if (restoreTriggered) {
              throw;
            }
            var newConnection = new SqlServerConnection(underlyingConnection.ConnectionString);
            try {
              underlyingConnection.Close();
              underlyingConnection.Dispose();
            }
            catch { }

            underlyingConnection = newConnection;
            restoreTriggered = true;
            continue;
          }
          else
            throw;
        }
      }
    }

    // Constructors

    public Connection(SqlDriver driver, bool checkConnection)
      : base(driver)
    {
      underlyingConnection = new SqlServerConnection();
      checkConnectionIsAlive = checkConnection;
    }
  }
}