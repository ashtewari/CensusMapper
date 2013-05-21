using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.Models
{
    public class CountyFips
    {
        public Table table { get; set; }
    }

    public class Table
    {
        public List<string> columnNames { get; set; }
        public List<string> columnTypes { get; set; }
        public List<List<string>> rows { get; set; }
    }
}
