using System.Data;
using Microsoft.Data.SqlClient;

namespace FrontBlazor_AppiGenericaCsharp.Services
{
    public class SpService
    {
        private readonly IConfiguration _config;

        public SpService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<(bool, List<Dictionary<string, object?>>, string)> EjecutarSpAsync(
            string nombreSp,
            Dictionary<string, object?> parametros)
        {
            var resultados = new List<Dictionary<string, object?>>();

            try
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand(nombreSp, conn);

                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var p in parametros)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var fila = new Dictionary<string, object?>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        fila[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }

                    resultados.Add(fila);
                }

                return (true, resultados, "OK");
            }
            catch (Exception ex)
            {
                return (false, resultados, ex.Message);
            }
        }
    }
}