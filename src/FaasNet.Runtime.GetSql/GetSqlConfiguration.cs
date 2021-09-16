using FaasNet.Runtime.Attributes;

namespace FaasNet.Runtime.GetSql
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