using Npgsql;

namespace Ticketing.Infrastructure.Configurations
{
    public class DapperContextAccessor
    {
        private NpgsqlConnection? _connectionCurrent { get; set; }
        private NpgsqlTransaction? _transactionCurrent { get; set; }

        public NpgsqlConnection? Connection => _connectionCurrent;

        public NpgsqlTransaction? Transaction => _transactionCurrent;

        public void SetConnection(NpgsqlConnection connection)
        {
            _connectionCurrent = connection;
            Console.WriteLine("Connection has been set.");  // Log test
        }

        public void SetTransaction(NpgsqlTransaction transaction)
        {
            _transactionCurrent = transaction;
        }

        public void Clear()
        {
            _connectionCurrent = null;
            _transactionCurrent = null;
        }
    }
}

