using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Task_1
{
    public class DatabaseManagment
    {
        /// <summary>
        /// Этот метод получает список с путями к файлам
        /// Затем он создает БД и таблицу, если их еще нет
        /// После уже импортирует все файлы в БД, разбивая строки по разделителям "||", поэтому их и нельзя удалить
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task CreateAndImportDatabase(List<string> files)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = "CREATE DATABASE filesdb";
                    command.Connection = connection;
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("База данных создана");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            try
            {
                string sqlExpression = @"CREATE TABLE Files 
                                (Id INT PRIMARY KEY IDENTITY, 
                                 Date NVARCHAR(50), 
                                 LatinStr NVARCHAR(50),
                                 RussianStr NVARCHAR(50),
                                 Number BIGINT,
                                 NumberFl DECIMAL(10,8))";

                using (SqlConnection connection = new SqlConnection(connectionStringToDB))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("Таблица Files создана");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            using (SqlConnection connection = new SqlConnection(connectionStringToDB))
            {
                await connection.OpenAsync();
                int j = 1;
                foreach (var file in files)
                {
                    using (var fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string textFromFile = Encoding.Default.GetString(buffer);
                        _ = textFromFile.Replace("\n", "");
                        _ = textFromFile.Remove(textFromFile.Length - 1);
                        int i = 0;
                        var arr = textFromFile.Split("||");

                        while(i < arr.Length - 1)
                        {
                            SqlCommand command = new SqlCommand();
                            command.Connection = connection;
                            command.CommandText = @"INSERT INTO Files VALUES (@Date, @LatinStr, @RussianStr, @Number, @NumberFl)";
                            command.Parameters.Add("@Date", SqlDbType.NVarChar);
                            command.Parameters.Add("@LatinStr", SqlDbType.NVarChar);
                            command.Parameters.Add("@RussianStr", SqlDbType.NVarChar);
                            command.Parameters.Add("@Number", SqlDbType.Int);
                            command.Parameters.Add("@NumberFl", SqlDbType.Decimal);

                            command.Parameters["@Date"].Value = arr[i];
                            command.Parameters["@LatinStr"].Value = arr[i + 1];
                            command.Parameters["@RussianStr"].Value = arr[i + 2];
                            command.Parameters["@Number"].Value = arr[i + 3];
                            command.Parameters["@NumberFl"].Value = arr[i + 4];

                            await command.ExecuteNonQueryAsync();

                            i += 5;
                            Console.WriteLine($"{j}/{50 - j}");
                            j++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Здесь идет подсчет суммы всех целых чисел через sql-запрос
        /// </summary>
        /// <returns></returns>
        public async Task GetSumm()
        {
            using (SqlConnection connection = new SqlConnection(connectionStringToDB))
            {
                await connection.OpenAsync();
                string sql = @"SELECT SUM(f.Number) FROM Files f";
                SqlCommand command = new SqlCommand(sql, connection);
                var summ = await command.ExecuteScalarAsync();
                Console.WriteLine($"Сумма всех целых чисел равна {summ}.");
            }
        }

        /// <summary>
        /// Здесь считается медиана дробных чисел через sql-запрос
        /// </summary>
        /// <returns></returns>
        public async Task GetMedian()
        {
            using (SqlConnection connection = new SqlConnection(connectionStringToDB))
            {
                await connection.OpenAsync();
                string sql = @"SELECT TOP (1) PERCENTILE_CONT (0.5)
                                           WITHIN GROUP(ORDER BY f.NumberFl)
                                           OVER()
                                FROM Files f";
                SqlCommand command = new SqlCommand(sql, connection);
                var med = await command.ExecuteScalarAsync();
                Console.WriteLine($"Медиана всех дробных чисел равна {med}.");
            }
        }

        private static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        private static string connectionStringToDB = "Server=(localdb)\\mssqllocaldb;Database=filesdb;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
