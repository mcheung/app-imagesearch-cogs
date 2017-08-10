using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageSearch.Shared.Services.GoogleSearch
{
    public class SearchResult
    {
        [JsonProperty("items")]
        public List<Image> Images { get; set; }
    }
}
