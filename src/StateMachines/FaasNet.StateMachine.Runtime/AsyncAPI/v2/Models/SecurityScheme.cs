namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models
{
    public class SecurityScheme
    {
        /// <summary>
        /// The type of the security scheme.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// A short description for security scheme.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The name of the header, query or cookie parameter to be used.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The location of the API key. Valid values are "user" and "password" for "apiKey" and "query".
        /// "header" or "cookie" for "httpApiKey".
        /// </summary>
        public string In { get; set; }
        /// <summary>
        /// The name of the HTTP Authorization scheme to be used.
        /// </summary>
        public string Scheme { get; set; }
    }
}
