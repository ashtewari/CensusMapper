using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace CensusMapper.UnitTests
{
    using System.Diagnostics;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SbaApi api = new SbaApi();
            var data = api.GetCountyData("nc");
            Debug.WriteLine(data);
        }
    }
}
