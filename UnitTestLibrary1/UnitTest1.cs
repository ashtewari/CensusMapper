using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CensusMapper;
using CensusMapper.Services;
using CensusMapper.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTestLibrary1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IApiKeyProvider keys = new MockApiKeyProvider();
            var vm = new MainViewModel(new MockBingMapsApi(keys), new MockCensusApi(keys));
            Assert.IsNotNull(vm);
        }
    }
}
