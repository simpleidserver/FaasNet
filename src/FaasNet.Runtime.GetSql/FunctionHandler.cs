using FaasNet.Runtime.Parameters;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FaasNet.Runtime.GetSql
{
    public class FunctionHandler
    {
        public async Task<JObject> Handle(FunctionParameter<GetSqlConfiguration> parameter)
        {
            var jArr = new JArray();
            using (var connection = new SqlConnection(parameter.Configuration.ConnectionString))
            {
                var command = new SqlCommand(parameter.Configuration.SqlQuery, connection);
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                var columns = reader.GetColumnSchema();
                while(await reader.ReadAsync())
                {
                    var record = new JObject();
                    for(int i = 0; i < columns.Count; i++)
                    {
                        var column = columns[i];
                        record.Add(column.ColumnName, reader[i].ToString());
                    }

                    jArr.Add(record);
                }
            }

            var result = new JObject();
            result.Add("content", jArr);
            return result;
        }
    }
}
