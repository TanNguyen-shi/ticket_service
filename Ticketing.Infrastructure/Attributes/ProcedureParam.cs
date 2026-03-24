using System.Data;

namespace Ticketing.Infrastructure.Helpers.Model;

public class ProcedureParam
{
    public object? Value { get; set; }
    public DbType? DbType { get; set; }
    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

    public static ProcedureParam In(object? value, DbType? dbType = null)
        => new()
        {
            Value = value,
            DbType = dbType,
            Direction = ParameterDirection.Input
        };

    public static ProcedureParam Out(DbType? dbType = null)
        => new()
        {
            Value = null,
            DbType = dbType,
            Direction = ParameterDirection.Output
        };

    public static ProcedureParam InOut(object? value, DbType? dbType = null)
        => new()
        {
            Value = value,
            DbType = dbType,
            Direction = ParameterDirection.InputOutput
        };
}