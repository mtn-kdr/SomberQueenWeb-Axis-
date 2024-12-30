using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;

namespace SomberQueenWeb_Axis_.Utilities
{
    public class DBHelper
    {
        private readonly string _connectionString;

        public DBHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Execute(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Execute(query, parameters);
            }
        }

        public IEnumerable<T> Query<T>(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<T>(query, parameters);
            }
        }

        public T QueryFirstOrDefault<T>(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(query, parameters);
            }
        }

        public void ExecuteProcedure(string procedureName, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public T ExecuteScalar<T>(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<T>(query, parameters);
            }
        }


        public bool TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open(); 
                    return true; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanı bağlantı hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
        }


        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(query, parameters);
            }
        }
    }
}
