using Microsoft.AspNetCore.Builder.Extensions;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Tests
{
    [TestClass]
    public class AdminTester
    {
        AdminManager adminManager;

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
            Assert.IsNotNull(map.Image);
            Assert.AreEqual(map.Image.Thumbnail, image.Thumbnail);
            Assert.AreEqual(map.Image.Url, image.Url);

            await adminManager.DeleteMapAsync(id);

            var exception = await Assert.ThrowsExceptionAsync<MuseumMapException>(async () => await adminManager.GetMapAsync(id));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.ErrorCode == MuseumMapErrorCode.InvalidMapError);
        }

        [TestMethod]
        public void MapModificationTest()
        {

        }

        [TestMethod]
        public async Task GetMapsTest()
        { 
            
        }

        [TestMethod]
        public void ActiveMapTest()
        {

        }

        [TestMethod]
        public void PoiCRUDTest()
        {

        }

        [TestMethod]
        public void PoiModificationTest()
        {

        }
    }
}
