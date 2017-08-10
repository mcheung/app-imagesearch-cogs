using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageSearch.Shared.Services.GoogleSearch
{
    public class Image
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("htmlTitle")]
        public string HtmlTitle { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("displayLink")]
        public string DisplayLink { get; set; }
        [JsonProperty("snippet")]
        public string Snippet { get; set; }
        [JsonProperty("htmlSnippet")]
        public string HtmlSnippet { get; set; }
        [JsonProperty("mime")]
        public string Mime { get; set; }
    }
}
