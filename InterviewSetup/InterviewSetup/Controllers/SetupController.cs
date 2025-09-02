using InterviewSetup.Data;
using InterviewSetup.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterviewSetup.Controllers;

[Route("api/setup")]
public class SetupController(VehicleService vehicleService, SetupContext setupContext) : Controller
{
    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await vehicleService.GetWMIsForManufacturer("bmw");
        return Ok(vehicles);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await setupContext.Users.ToListAsync();
        return Ok(users);
    }
}