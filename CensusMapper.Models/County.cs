using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.Models
{
    public class SbaData
    {
        public object county_name { get; set; }
        public object description { get; set; }
        public string feat_class { get; set; }
        public string feature_id { get; set; }
        public string fips_class { get; set; }
        public string fips_county_cd { get; set; }
        public object full_county_name { get; set; }
        public object link_title { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string primary_latitude { get; set; }
        public string primary_longitude { get; set; }
        public string state_abbreviation { get; set; }
        public string state_name { get; set; }
    }
}
