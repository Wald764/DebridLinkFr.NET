using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Models
{
    /// <summary>
    /// Represents a hosted file with information about its attributes.
    /// </summary>
    public class HostedFile : BaseFile
    {
        /// <summary>
        /// Gets or sets the unique identifier of the hosted file.
        /// </summary>
        [JsonProperty("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hosted file has expired.
        /// </summary>
        [JsonProperty("expired")]
        public bool Expired { get; set; }

        /// <summary>
        /// Gets or sets the chunk number of the hosted file.
        /// </summary>
        [JsonProperty("chunk")]
        public int Chunk { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the hosted file was created.
        /// </summary>
        [JsonProperty("created")]
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets an optional description of the hosted file (optional).
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets an optional category for the hosted file (optional).
        /// </summary>
        [JsonProperty("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the URL of the hosted file (optional).
        /// </summary>
        [JsonProperty("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the download URL of the hosted file (optional).
        /// </summary>
        [JsonProperty("downloadUrl")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the host of the hosted file (optional).
        /// </summary>
        [JsonProperty("host")]

        public string? Host { get; set; }
        /// <summary>
        /// Gets or sets other links associated with the hosted file.
        /// </summary>
        [JsonProperty("otherLinks")]
        public List<string>? OtherLinks { get; set; }
    }

}
