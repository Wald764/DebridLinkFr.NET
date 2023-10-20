using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Models
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets a value indicating the success of the response.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the value of the response.
        /// </summary>
        [JsonProperty("value")]
        public T? Value { get; set; }

        /// <summary>
        /// Gets or sets the error of the response.
        /// </summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the pagination information.
        /// </summary>
        [JsonProperty("pagination")]
        public Pagination? Pagination { get; set; }
    }
}
