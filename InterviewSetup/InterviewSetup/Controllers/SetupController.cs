using InterviewSetup.Data;
using InterviewSetup.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterviewSetup.Controllers
{
    [Route("api/setup")]
    public class SetupController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly SetupContext _setupContext;

        public SetupController(VehicleService vehicleService, SetupContext setupContext)
        {
            _vehicleService = vehicleService;
            _setupContext = setupContext;
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var vehicles = await _vehicleService.GetWMIsForManufacturer("bmw");
            return Ok(vehicles);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _setupContext.Users.ToListAsync();
            return Ok(users);
        }
    }
}
