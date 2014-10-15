// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CensusMapper
{

    public class Point
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public IList<double> Coordinates { get; set; }
    }
}
