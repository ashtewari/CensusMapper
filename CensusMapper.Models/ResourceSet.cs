// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CensusMapper
{

    public class ResourceSet
    {

        [JsonProperty("estimatedTotal")]
        public int EstimatedTotal { get; set; }

        [JsonProperty("resources")]
        public IList<Resource> Resources { get; set; }
    }
}
