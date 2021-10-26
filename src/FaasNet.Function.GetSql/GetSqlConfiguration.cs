using FaasNet.Function.Attributes;

namespace FaasNet.Function.GetSql
{
    [FuncInfo("GetSql", "v1")]
    public class GetSqlConfiguration
    {
        [Translation("fr", "Chaîne de connection")]
        [Translation("en", "ConnectionString")]
        public string ConnectionString { get; set; }
        [Translation("fr", "Requête SQL")]
        [Translation("en", "SQL Query")]
        public string SqlQuery { get; set; }
    }
}