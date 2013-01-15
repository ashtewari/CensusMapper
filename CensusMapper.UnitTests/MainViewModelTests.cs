using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CensusMapper.Services;
using CensusMapper.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace CensusMapper.UnitTests
{
    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel vm;

        [TestInitialize]
        public void Create_Target_VM()
        {
            var keys = new MockApiKeyProvider();
            vm = new MainViewModel(new MockBingMapsApi(keys), new MockCensusApi(keys));            
        }

        [TestMethod]
        public void MainVM_Creates_MapVM()
        {
            Assert.IsNotNull(vm.MapViewModel);
        }

        [TestMethod]
        public void MainVM_Loads_State_Data()
        {
            Assert.AreEqual(0, vm.MapViewModel.Items.Count);

            vm.LoadData.Execute(null);

            Assert.AreEqual(3, vm.MapViewModel.Items.Count);
            Assert.AreEqual(9535483, vm.MapViewModel.Items[0].Population.Value);
        }
    }
}
