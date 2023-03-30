using Microsoft.AspNetCore.Builder.Extensions;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Notl.MuseumMap.Tests
{
    [TestClass]
    public class AdminTester
    {
        AdminManager adminManager;

        public AdminTester() 
        {
            adminManager = new AdminManager(TestHelper.Db, new StorageManager(TestHelper.Options));
        }

        [TestMethod]
        public async Task MapCRUDTest()
        {
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);

            Assert.IsNotNull(map);

            var test = await adminManager.GetMapAsync(id);
            Assert.IsNotNull(test);
            Assert.AreEqual(map.Id, test.Id);
            Assert.IsNull(map.Image);

            ImageReference image = new ImageReference();
            image.Id = id;
            image.Url = "welp";
            image.Thumbnail = "wlp";

            test = await adminManager.UpdateMapAsync(id, image);
            Assert.IsNotNull(test);
            Assert.AreEqual(map.Id, test.Id);
            Assert.IsNotNull(test.Image);
            Assert.AreEqual(test.Image.Thumbnail, image.Thumbnail);
            Assert.AreEqual(test.Image.Url, image.Url);

            await adminManager.DeleteMapAsync(id);

            var exception = await Assert.ThrowsExceptionAsync<MuseumMapException>(async () => await adminManager.GetMapAsync(id));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.ErrorCode == MuseumMapErrorCode.InvalidMapError);
        }

        [TestMethod]
        public async Task MapModificationTest()
        {
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            var test = await adminManager.GetMapAsync(id);

            ImageReference image1 = new ImageReference();
            image1.Id = id;
            image1.Url = "welp";
            image1.Thumbnail = "wlp";

            test = await adminManager.UpdateMapAsync(id, image1);
            Assert.IsNotNull(test);
            Assert.AreEqual(map.Id, test.Id);
            Assert.IsNotNull(test.Image);
            Assert.AreEqual(test.Image.Thumbnail, image1.Thumbnail);
            Assert.AreEqual(test.Image.Url, image1.Url);

            ImageReference image2 = new ImageReference();
            image2.Id = id;
            image2.Url = "abcd";
            image2.Thumbnail = "abc";

            test = await adminManager.UpdateMapAsync(id, image2);
            Assert.IsNotNull(test);
            Assert.AreEqual(map.Id, test.Id);
            Assert.IsNotNull(test.Image);
            Assert.AreEqual(test.Image.Thumbnail, image2.Thumbnail);
            Assert.AreEqual(test.Image.Url, image2.Url);

            await adminManager.DeleteMapAsync(id);
        }

        [TestMethod]
        public async Task GetMapsTest()
        {
            Guid id1 = Guid.NewGuid();
            var map1 = await adminManager.CreateMapAsync(id1);

            Guid id2 = Guid.NewGuid();
            var map2 = await adminManager.CreateMapAsync(id2);

            var maps = await adminManager.GetMapsAsync();
            Assert.IsNotNull(maps);
            Assert.IsTrue(maps.Count != 0);

            var test1 = maps.Where((m) => m.Id == id1);
            Assert.IsNotNull(test1);

            var test2 = maps.Where((m) => m.Id == id2);
            Assert.IsNotNull(test2);

            await adminManager.DeleteMapAsync(id1);
            await adminManager.DeleteMapAsync(id2);
        }

        [TestMethod]
        public async Task ActiveMapTest()
        {
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);
            Assert.AreEqual(map.Id, id);

            await adminManager.SetActiveMapAsync(id);

            var test = await adminManager.GetActiveMapAsync();
            Assert.IsNotNull(test);
            Assert.AreEqual(test.Id, map.Id);
        }

        [TestMethod]
        public async Task PoiCRUDTest()
        {
            Guid poiID = Guid.NewGuid();
            Guid poiID_2 = Guid.NewGuid();
            Guid mapID = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(mapID);
            Assert.IsNotNull(map);

            //Ensure the CreatePOIAsync function creates the POI properly
            var poi = await adminManager.CreatePOIAsync(poiID,mapID,0,0, POIType.Exhibit);
            var poi_2 = await adminManager.CreatePOIAsync(poiID_2, mapID, 1, 1, POIType.Bathroom);
            Assert.IsNotNull(poi);
            Assert.IsNotNull(poi_2);

            //Ensure the GetPOI function returns the POI
            var poiGet = await adminManager.GetPOIAsync(poiID);
            Assert.IsNotNull(poiGet);
            Assert.AreEqual(poi.Id, poiGet.Id);

            //Ensure the GetPOIs function properly returns all POIs that are assigned to a map
            var poiGetAll = await adminManager.GetPOIsAsync(mapID);
            Assert.IsNotNull(poiGetAll);

            var test = poiGetAll.Where((p) => p.Id == poiID || p.Id == poiID_2);
            Assert.IsTrue(test.Count() == 2);

            //Ensure the Delete POI function deletes a POI
            await adminManager.DeletePOIAsync(poiID);
            await adminManager.DeletePOIAsync(poiID_2);

            var exception = await Assert.ThrowsExceptionAsync<MuseumMapException>( async() => await adminManager.GetPOIAsync(poiID));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.ErrorCode == MuseumMapErrorCode.InvalidPOIError);

            exception = await Assert.ThrowsExceptionAsync<MuseumMapException>(async () => await adminManager.GetPOIAsync(poiID_2));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.ErrorCode == MuseumMapErrorCode.InvalidPOIError);

            //Delete test map from DB
            await adminManager.DeleteMapAsync(mapID);
        }

        [TestMethod]
        public async Task PoiModificationTest()
        {
            Guid id = Guid.NewGuid();
            var map = await adminManager.CreateMapAsync(id);
            Assert.IsNotNull(map);

            Guid POIid = Guid.NewGuid();
            var poi = await adminManager.CreatePOIAsync(POIid, id, 0, 0, Core.Entities.POIType.Exhibit);
            Assert.IsNotNull(poi);

            var test = await adminManager.UpdatePOIAsync(POIid, id, 1, 1);
            Assert.IsNotNull(test);
            Assert.AreNotEqual(test.x, poi.x);
            Assert.AreNotEqual(test.y, poi.y);
            Assert.AreNotEqual(test.POIType, poi.POIType);

            poi = await adminManager.UpdatePOIContentAsync(POIid, POIType.Item, "title", "description", new ImageReference { Thumbnail = "welp", Url = "Welp"});
            Assert.IsNotNull(poi);
            Assert.IsNotNull(poi.Title);
            Assert.IsNotNull(poi.Description);
            Assert.IsNotNull(poi.Image);
            Assert.IsNotNull(poi.Image.Url);
            Assert.IsTrue(poi.Title.Equals("title"));
            Assert.IsTrue(poi.Description.Equals("description"));
            Assert.IsTrue(poi.Image.Url.Equals("Welp"));
            Assert.IsTrue(poi.POIType == POIType.Item);

            await adminManager.DeleteMapAsync(id);
            await adminManager.DeletePOIAsync(POIid);
        }
    }
}
