using Npgsql;

internal class Program
{
    private static void Main(string[] args)
    {
        // Читаем строку подключения к БД из переменной окружения
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Connection string not found in environment variable 'ConnectionStrings__DefaultConnection'.");
            return;
        }

        Console.WriteLine("Trying to connect to PostgreSQL...");
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connection successful!");

            // Пример: создаём таблицу, если её ещё нет
            using var createTableCmd = new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS test_table (id SERIAL PRIMARY KEY, value TEXT);", connection);
            createTableCmd.ExecuteNonQuery();
            Console.WriteLine("Table 'test_table' created if it didn't exist.");
            
            using var insertCmd = new NpgsqlCommand("INSERT INTO test_table (value) VALUES (@value);", connection);
            insertCmd.Parameters.AddWithValue("@value", "Hello from .NET!");
            insertCmd.ExecuteNonQuery();
            Console.WriteLine("Inserted sample row into 'test_table'.");
            
            using var selectCmd = new NpgsqlCommand("SELECT id, value FROM test_table;", connection);
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var value = reader.GetString(1);
                Console.WriteLine($"Row: id={id}, value={value}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while connecting to the database: {e.Message}");
        }
    }
}