using Newtonsoft.Json;

namespace FaasNet.Runtime.AsyncAPI.v2.Models
{
    public class Info
    {
        /// <summary>
        /// The title of the application.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Provides the version of the application API (not to be confused with the specification version).
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
        /// <summary>
        /// A short description of the application. CommonMark syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// A URL to the Terms of Service for the API. MUST be in the format of a URL.
        /// </summary>
        public string TermsOfService { get; set; }
    }
}
