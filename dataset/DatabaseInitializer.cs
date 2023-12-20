using Npgsql;
using Microsoft.EntityFrameworkCore;
using WebAPI.Db;

public static class DatabaseInitializer
{
   public static void InitializeDatabase(TheDbContext dbContext)
    {
    try
        {
        using (var connection = dbContext.Database.GetDbConnection() as NpgsqlConnection)
        {
            if (dbContext != null || connection != null)
            {
                connection.Open();
                CreateTables(connection);
                Init(connection);
            }

        }

            Console.WriteLine("Database initialized successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initializing database: {e.Message}");
        }
    }

    private static void InsertData(NpgsqlConnection connection, string[] row)
    {
     // Check if the row contains any "NA" values
    if (row.Any(value => value.Trim().Equals("NA", StringComparison.OrdinalIgnoreCase)))
    {
        return;
    }

         using (NpgsqlCommand cmd = new NpgsqlCommand())
        {
            // Console.WriteLine("Inserting data");
            // Insert into Categories
            cmd.Connection = connection;
            cmd.CommandText = $@"
                INSERT INTO public.""Categories"" (""Category_name"") VALUES ('{row[2]}') RETURNING ""Category_id""";
                var categoryId = cmd.ExecuteScalar();

                cmd.Connection = connection;
                cmd.CommandText = $@"
                    INSERT INTO public.""Time_units"" (""Time_unit_name"") VALUES ('{row[4]}') RETURNING ""Time_unit_id""";

                var timeUnitId = cmd.ExecuteScalar();
        
          
            cmd.Connection = connection;
            cmd.CommandText = $@"
                INSERT INTO public.""Passwords"" (
                    ""Password_value"",
                    ""Category_id"",
                    ""Value"", 
                    ""Offline_crack_sec"", 
                    ""Rank_alt"", 
                    ""Strength"", 
                    ""Font_size"",
                    ""Time_unit_id""
                ) VALUES (
                    '{row[1]}', {categoryId}, '{row[3]}', '{row[5]}', '{row[6]}', '{row[7]}', '{row[8]}', {timeUnitId}
                )";

            cmd.ExecuteNonQuery();
        }

        // Console.WriteLine("Data inserted");
    }

    private static void CreateTables(NpgsqlConnection connection)
    {
        using (NpgsqlCommand cmd = new NpgsqlCommand())
        {
            cmd.Connection = connection;
            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS public.""Categories"" (
                    ""Category_id"" SERIAL PRIMARY KEY,
                    ""Category_name"" VARCHAR(100)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS public.""Time_units"" (
                    ""Time_unit_id"" SERIAL PRIMARY KEY,
                    ""Time_unit_name"" VARCHAR(100)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS public.""Passwords"" (
                    ""Password_id"" SERIAL PRIMARY KEY,
                    ""Password_value"" VARCHAR(100),
                    ""Category_id"" INT,
                    ""Value"" FLOAT,
                    ""Time_unit"" VARCHAR(100),
                    ""Offline_crack_sec"" FLOAT,
                    ""Rank_alt"" INT,
                    ""Strength"" INT,
                    ""Font_size"" INT
                )";
            cmd.ExecuteNonQuery();
        }
    }

    private static void Init(NpgsqlConnection connection)
    {
         try
        {
            string[] headers = new string[0];

            using (var reader = new System.IO.StreamReader("dataset/passwords.csv"))
            {
                int lineCount = 0;
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine().Split(',');
                    // Console.WriteLine(row[2]);
                    if (lineCount == 0)
                    {
                        headers = row;
                        lineCount++;
                    }
                    else
                    {
                        InsertData(connection, row);
                        lineCount++;
                    }
                }
            }

            Console.WriteLine($"Processed {headers.Length} lines.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}