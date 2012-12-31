using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace CensusMapper.UnitTests
{
    using System.Diagnostics;
    using Newtonsoft.Json;
    using CensusMapper;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
             // CallSbaApi();            
        }

        private static async System.Threading.Tasks.Task CallSbaApi()
        {
            SbaApi api = new SbaApi();
            var data = await api.GetCountyData("nc");
            Debug.WriteLine(data);
        }
    }
}
