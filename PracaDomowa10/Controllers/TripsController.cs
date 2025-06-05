using Microsoft.AspNetCore.Mvc;
using PracaDomowa10.DbService;
using PracaDomowa10.DTOs;

namespace PracaDomowa10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly IClientService _clientService;


    public TripsController(ITripService tripService, IClientService clientService)
    {
        _tripService = tripService;
        _clientService = clientService;
    }
        
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _tripService.GetSortedTrips(page, pageSize);
        return Ok(result);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> RegisterClientOnTripAsync([FromBody] ClientTripDTO request, CancellationToken cancellationToken)
    {
        try
        {
            await _clientService.AddClientToTripAsync(request, cancellationToken);
            return Ok("Client assigned to trip successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    } 
        
}