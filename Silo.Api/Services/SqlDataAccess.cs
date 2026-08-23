using System.Data.Common;
using System.Diagnostics;
using Silo.Application.Exceptions;
using Microsoft.Data.SqlClient;
using Silo.Application.Contracts;

namespace Silo.Api.Services;
public partial class SqlDataAccess(ILogger<SqlDataAccess> Logger
    , IConfiguration Configuration) : IDataAccess
{
    public int CmdSqlExecuteNonQueryWithTransaction(Dictionary<string, KeyValuePair<string, object>[]> listCommands, string connectionString)
    {
        if (connectionString.HasNoValue())
        {
            throw new SqlServerConnectionStringException();
        }

        using var SqlConnection = new SqlConnection(connectionString);
        var result = 1;
        SqlConnection.Open();
        using var sqlTran = SqlConnection.BeginTransaction();
        SqlCommand currentCommand = null;
        try
        {
            foreach (var command in listCommands)
            {
                using var myCommand = new SqlCommand(command.Key, SqlConnection, sqlTran)
                {
                    CommandTimeout = 120
                };
                
                foreach (var param in command.Value)
                {
                    myCommand.Parameters.AddWithValue(param.Key, param.Value);
                }

                currentCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            sqlTran.Commit();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, currentCommand?.CommandText, currentCommand?.Parameters);
            sqlTran.Rollback();
#if DEBUG
            Debugger.Break();
#endif
            result = 0;
        }
        return result;
    }

    public int CmdSqlExecuteNonQueryWithTransaction(Dictionary<string, KeyValuePair<string, object>[]> listCommands)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        var result = 1;
        SqlConnection.Open();
        using var sqlTran = SqlConnection.BeginTransaction();
        SqlCommand currentCommand = null;
        try
        {
            foreach (var command in listCommands)
            {
                using var myCommand = new SqlCommand(command.Key, SqlConnection, sqlTran)
                {
                    CommandTimeout = 120
                };
                
                foreach (var param in command.Value)
                {
                    myCommand.Parameters.AddWithValue(param.Key, param.Value);
                }

                currentCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            sqlTran.Commit();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, currentCommand?.CommandText, currentCommand?.Parameters);
            sqlTran.Rollback();
#if DEBUG
            Debugger.Break();
#endif
            result = 0;
        }
        return result;
    }
 
    public int CmdSqlExecuteNonQueryWithTransaction(List<string> listCommands)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        var result = 1;
        SqlConnection.Open();
        using var sqlTran = SqlConnection.BeginTransaction();
        SqlCommand currentCommand = null;
        try
        {
            foreach (var command in listCommands)
            {
                using var myCommand = new SqlCommand(command, SqlConnection, sqlTran)
                {
                    CommandTimeout = 120
                };

                currentCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            sqlTran.Commit();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, currentCommand?.CommandText, currentCommand?.Parameters);
            sqlTran.Rollback();
#if DEBUG
            Debugger.Break();
#endif
            result = 0;
        }
        return result;
    }
    
    public int CmdSqlExecuteNonQueryWithTransaction(List<KeyValuePair<string, KeyValuePair<string, object>[]>> listCommands)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var sqlConnection = new SqlConnection(conStr);
        var result = 1;
        sqlConnection.Open();
        using var sqlTran = sqlConnection.BeginTransaction();
        SqlCommand currentCommand = null;
        try
        {
            foreach (var command in listCommands)
            {
                using var myCommand = new SqlCommand(command.Key, sqlConnection, sqlTran)
                {
                    CommandTimeout = 120
                };
                
                if (command.Value != null)
                {
                    foreach (var param in command.Value)
                    {
                        myCommand.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                currentCommand = myCommand;
                myCommand.ExecuteNonQuery();
            }
            sqlTran.Commit();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, currentCommand?.CommandText, currentCommand?.Parameters);
            sqlTran.Rollback();
#if DEBUG
            Debugger.Break();
#endif
            result = 0;
        }
        return result;
    }
    
    public int CmdSqlExecuteScalar(string command, params KeyValuePair<string, object>[] parameter)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var result = 0;
        try
        {
            SqlConnection.Open();
            var scalarResult = cmd.ExecuteScalar();
            if (scalarResult != null)
            {
                result = Convert.ToInt32(scalarResult);
            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return result;
    }

    public int CmdSqlExecuteScalar(string command, string connectionString, params KeyValuePair<string, object>[] parameter)
    {
        if (connectionString.HasNoValue())
        {
            throw new SqlServerConnectionStringException();
        }

        using var SqlConnection = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var result = 0;
        try
        {
            SqlConnection.Open();
            var scalarResult = cmd.ExecuteScalar();
            if (scalarResult != null)
            {
                result = Convert.ToInt32(scalarResult);
            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return result;
    }

    public int CmdSqlExecuteNonQuery(string command, params KeyValuePair<string, object>[] parameter)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var result = 0;
        try
        {
            SqlConnection.Open();
            result = cmd.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return result;
    }

    public int CmdSqlExecuteNonQuery(string command, string connectionString, params KeyValuePair<string, object>[] parameter)
    {
        if (connectionString.HasNoValue())
        {
            throw new SqlServerConnectionStringException();
        }

        using var SqlConnection = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var result = 0;
        try
        {
            SqlConnection.Open();
            result = cmd.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return result;
    }

    public DataTable SqlDataAdapter(string command, params KeyValuePair<string, object>[] parameter)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var dt = new DataTable();
        try
        {
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
            #if DEBUG 
            Debugger.Break();
            #endif
        }

        return dt;
    }

    public DataTable SqlDataAdapter(string command, int timeout, params KeyValuePair<string, object>[] parameter)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = timeout
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var dt = new DataTable();
        try
        {
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return dt;
    }

    public DataTable SqlDataAdapter(string command, string connectionString, params KeyValuePair<string, object>[] parameter)
    {
        if (connectionString.HasNoValue())
        {
            throw new SqlServerConnectionStringException();
        }

        using var SqlConnection = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var dt = new DataTable();
        try
        {
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return dt;
    }

    public DataTable SqlDataAdapter(string command
        , int timeout
        , string connectionString
        , params KeyValuePair<string, object>[] parameter)
    {
        if (connectionString.HasNoValue())
        {
            throw new SqlServerConnectionStringException();
        }

        using var SqlConnection = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = timeout
        };

        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        var dt = new DataTable();
        try
        {
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
        }
        return dt;
    }

    public DbParameter[] SqlDataReaders(string command, params KeyValuePair<string, object>[] parameter)
    {
        var conStr = Configuration.GetConnectionString("SqlDefaultConnectionString");

        if (conStr.HasNoValue())
        {
            throw new SqlServerConnectionStringException("SqlDefaultConnectionString");
        }

        using var SqlConnection = new SqlConnection(conStr);
        using var cmd = new SqlCommand(command, SqlConnection)
        {
            CommandTimeout = 120
        };
        
        foreach (var param in parameter)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }

        SqlParameter[] result = null;
        try
        {
            SqlConnection.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                result = new SqlParameter[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    result[i] = new SqlParameter(reader.GetName(i), reader.GetValue(i));
                }
            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning(exception, cmd.CommandText);
#if DEBUG
            Debugger.Break();
#endif
            result = null;
        }

        return result;
    }

    public DbParameter[] SqlDataReaders(string command, string connectionString, params KeyValuePair<string, object>[] parameter)
    {
        throw new NotImplementedException();
    }
}
