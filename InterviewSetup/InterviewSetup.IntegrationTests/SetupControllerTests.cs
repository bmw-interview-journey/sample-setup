using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using InterviewSetup.IntegrationTests.Setup;
using Newtonsoft.Json;
using System.Collections.Generic;
using InterviewSetup.Model;
using InterviewSetup.Data.Entities;

namespace InterviewSetup.IntegrationTests
{
    [TestFixture]
    public class SetupControllerTests : IntegrationTest
    {
        [Test]
        public async Task GetVehicles_Successfully()
        {
            var response = await TestClient.GetAsync("api/setup/vehicles");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var vehicles = JsonConvert.DeserializeObject<List<Wmi>>(content);

            Assert.AreEqual(5, vehicles.Count);
        }

        [Test]
        public async Task GetUsers_Successfully()
        {
            var response = await TestClient.GetAsync("api/setup/users");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(content);

            Assert.AreEqual(4, users.Count);
        }
    }
}
