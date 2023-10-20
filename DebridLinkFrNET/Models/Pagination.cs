using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Models
{
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; set; }

        /// <summary>
        /// Gets or sets the number of the next page.
        /// </summary>
        [JsonProperty("next")]
        public int Next { get; set; }

        /// <summary>
        /// Gets or sets the number of the previous page.
        /// </summary>
        [JsonProperty("previous")]
        public int Previous { get; set; }
    }
}
