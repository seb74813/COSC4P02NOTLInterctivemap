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
            /*
            //ensure map is not null
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);
            Assert.AreEqual(id, map.Id);
            //ensure that the map image isn't null
            Assert.IsNotNull(map.Image);

            //add here more tessting stuff if needed

            await adminManager.DeleteMapAsync(id);
            */
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
            /*
            //ensure map create function works
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);

            //ensure poi creation works
            Guid POIid = Guid.NewGuid();
            var poi = await adminManager.CreatePOIAsync(POIid, id, 0, 0, Core.Entities.POIType.Exhibit);
            Assert.IsNotNull(poi);

            //ensure that pois retrieve func works
            var getPOI = await adminManager.GetPOIsAsync(POIid);
            Assert.IsNotNull(getPOI);

            await adminManager.DeleteMapAsync(id);
            await adminManager.DeletePOIAsync(POIid);
            */
        }
    }
}