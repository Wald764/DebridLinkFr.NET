using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Models
{
    public class UserSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether HTTPS protocol is enabled.
        /// </summary>
        [JsonProperty("https")]
        public bool Https { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dark theme is enabled.
        /// </summary>
        [JsonProperty("themeDark")]
        public bool ThemeDark { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether old links are hidden.
        /// </summary>
        [JsonProperty("hideOldLinks")]
        public bool HideOldLinks { get; set; }

        /// <summary>
        /// Gets or sets the CDN used.
        /// </summary>
        [JsonProperty("cdn")]
        public string? Cdn { get; set; }
    }
}
