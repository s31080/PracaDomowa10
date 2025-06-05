using Microsoft.AspNetCore.Mvc;
using PracaDomowa10.DbService;

namespace PracaDomowa10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }
        
    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClientAsync(int idClient, CancellationToken cancellationToken)
    {
        try
        {
            await _clientService.DeleteClientAync(idClient, cancellationToken);
            return Ok("Client deleted");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}