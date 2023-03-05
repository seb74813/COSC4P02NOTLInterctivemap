using Notl.MuseumMap.Core.Managers;

namespace Notl.MuseumMap.Tests
{
    [TestClass]
    public class MapTester
    {
        MapManager mapManager;
        AdminManager adminManager;

        [TestMethod]
        public async void GetMapTest()
        {
            //ensure map is not null
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);
            Assert.AreEqual(id, map.Id);
            Assert.IsNotNull(map.Image);

            //more tessting stuff if needed

            await adminManager.DeleteMapAsync(id);

        }

        [TestMethod]
        public async Task GetPoiTest()
        {

            /*
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);

            Guid POIid = Guid.NewGuid();
            var poi = await adminManager.CreatePOIAsync(POIid, id, 0, 0, Core.Entities.POIType.Exhibit);
            Assert.IsNotNull(poi);

            var getPOI = await adminManager.GetPOIsAsync(POIid);
            Assert.IsNotNull(getPOI);

            //additonal testing ?

            await adminManager.DeleteMapAsync(id);
            await adminManager.DeletePOIAsync(POIid);
            */

        }

        [TestMethod]
        public async Task GetMapPoisTest()
        {
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);

            Guid POIid = Guid.NewGuid();
            var poi = await adminManager.CreatePOIAsync(POIid, id, 0, 0, Core.Entities.POIType.Exhibit);
            Assert.IsNotNull(poi);

            var getPOI = await adminManager.GetPOIsAsync(POIid);
            Assert.IsNotNull(getPOI);

            await adminManager.DeleteMapAsync(id);
            await adminManager.DeletePOIAsync(POIid);
        }
    }
}