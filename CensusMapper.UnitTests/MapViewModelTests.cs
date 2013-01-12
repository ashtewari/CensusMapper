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
    public class MapViewModelTests
    {
        private MapViewModel vm;

        [TestInitialize]
        public void Create_Target_VM()
        {
            var keys = new MockApiKeyProvider();
            vm = new MapViewModel(new MockBingMapsApi(keys), new MockCensusApi(keys));
        }

        [TestMethod]
        public void Validate_Initial_State()
        {
            Assert.AreEqual(0, vm.Items.Count);
            Assert.AreEqual(5.0, vm.CurrentZoomLevel);
            Assert.IsInstanceOfType(vm.CenterOfUs, typeof(Coordinates));

            var center = vm.CenterOfUs as Coordinates;
            Assert.AreEqual(39.833333, center.Latitude);
            Assert.AreEqual(-98.583333, center.Longitude);
        }

        [TestMethod]
        public void Population_For_ZipCode_Selected_Location()
        {
            Assert.AreEqual(0, vm.Items.Count);
            vm.SelectLocation(vm.CenterOfUs as Coordinates).Wait();
            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual(55555, vm.Items[0].Population);
        }
    }
}
