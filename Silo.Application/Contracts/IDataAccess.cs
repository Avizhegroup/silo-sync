using System.Data.Common;

namespace Silo.Application.Contracts;

public interface IDataAccess
{
    int CmdSqlExecuteScalar(string command, params KeyValuePair<string, object>[] parameter);
    int CmdSqlExecuteScalar(string command, string connectionString, params KeyValuePair<string, object>[] parameter);
    int CmdSqlExecuteNonQuery(string command, params KeyValuePair<string, object>[] parameter);
    int CmdSqlExecuteNonQuery(string command, string connectionString, params KeyValuePair<string, object>[] parameter);
    int CmdSqlExecuteNonQueryWithTransaction(Dictionary<string, KeyValuePair<string, object>[]> listCommands);
    int CmdSqlExecuteNonQueryWithTransaction(List<KeyValuePair<string, KeyValuePair<string, object>[]>> listCommands);
    int CmdSqlExecuteNonQueryWithTransaction(Dictionary<string, KeyValuePair<string, object>[]> listCommands, string connectionString);
    int CmdSqlExecuteNonQueryWithTransaction(List<string> listCommands);
    DataTable SqlDataAdapter(string command, params KeyValuePair<string, object>[] parameter);
    DataTable SqlDataAdapter(string command, int timeout, params KeyValuePair<string, object>[] parameter);
    DataTable SqlDataAdapter(string command, string connectionString, params KeyValuePair<string, object>[] parameter);
    DataTable SqlDataAdapter(string command, int timeout, string connectionString, params KeyValuePair<string, object>[] parameter);
    DbParameter[] SqlDataReaders(string command, params KeyValuePair<string, object>[] parameter);
    DbParameter[] SqlDataReaders(string command, string connectionString, params KeyValuePair<string, object>[] parameter);
}
