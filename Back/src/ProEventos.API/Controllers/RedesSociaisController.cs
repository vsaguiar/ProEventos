using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;

namespace ProEventos.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RedesSociaisController : ControllerBase
{
    private readonly IRedeSocialService _redeSocialService;
    private readonly IEventoService _eventoService;
    private readonly IPalestranteService _palestranteService;
    public RedesSociaisController(IRedeSocialService redeSocialService, IEventoService eventoService, IPalestranteService palestranteService)
    {
        _redeSocialService = redeSocialService;
        _eventoService = eventoService;
        _palestranteService = palestranteService;
    }

    [HttpGet("evento/{eventoId}")]
    public async Task<IActionResult> GetByEvento(int eventoId)
    {
        try
        {
            if (!await AutorEvento(eventoId)) return Unauthorized();

            var redeSocial = await _redeSocialService.GetAllByEventoIdAsync(eventoId);
            if (redeSocial is null) return NoContent();

            return Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar rede social por evento. Erro: {ex.Message}");
        }
    }


    [HttpGet("palestrante")]
    public async Task<IActionResult> GetByPalestrante()
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserIdExtensios());
            if (palestrante is null) return Unauthorized();

            var redeSocial = await _redeSocialService.GetAllByPalestranteIdAsync(palestrante.Id);
            if (redeSocial is null) return NoContent();

            return Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar rede social por palestrante. Erro: {ex.Message}");
        }
    }


    [HttpPut("evento/{eventoId}")]
    public async Task<IActionResult> SaveByEvento(int eventoId, RedeSocialDTO[] models)
    {
        try
        {
            if (!await AutorEvento(eventoId)) return Unauthorized();
            
            var redeSocial = await _redeSocialService.SaveByEventoAsync(eventoId, models);
            if (redeSocial is null) return NoContent();

            return Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar salvar rede social por evento. Erro: {ex.Message}");
        }
    }


    [HttpPut("palestrante")]
    public async Task<IActionResult> SaveByPalestrante(RedeSocialDTO[] models)
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserIdExtensios());
            if (palestrante is null) return Unauthorized();
            
            var redeSocial = await _redeSocialService.SaveByPalestranteAsync(palestrante.Id, models);
            if (redeSocial is null) return NoContent();

            return Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar salvar rede social por palestrante. Erro: {ex.Message}");
        }
    }


    [HttpDelete("evento/{eventoId}/{redeSocialId}")]
    public async Task<IActionResult> DeleteByEvento(int eventoId, int redeSocialId)
    {
        try
        {
            if (!await AutorEvento(eventoId)) return Unauthorized();

            var redeSocial = await _redeSocialService.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
            if (redeSocial is null) return NoContent();

            return await _redeSocialService.DeleteByEventoAsync(eventoId, redeSocialId)
                    ? Ok(new { message = "Rede social deletada!" })
                    : throw new Exception("Ocorreu um erro não específico ao tentar deletar rede social por evento.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar rede social por evento. Erro: {ex.Message}");
        }
    }


    [HttpDelete("palestrante/{redeSocialId}")]
    public async Task<IActionResult> DeleteByPalestrante(int redeSocialId)
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserIdExtensios());
            if (palestrante is null) return Unauthorized();

            var redeSocial = await _redeSocialService.GetRedeSocialPalestranteByIdsAsync(palestrante.Id, redeSocialId);
            if (redeSocial is null) return NoContent();

            return await _redeSocialService.DeleteByPalestranteAsync(palestrante.Id, redeSocialId)
                    ? Ok(new { message = "Rede social deletada!" })
                    : throw new Exception("Ocorreu um erro não específico ao tentar deletar rede social por palestrante.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar rede social por palestrante. Erro: {ex.Message}");
        }
    }


    [NonAction]
    private async Task<bool> AutorEvento(int eventoId)
    {
        var evento = await _eventoService.GetEventoByIdAsync(User.GetUserIdExtensios(), eventoId, false);
        if (evento is null) return false;

        return true;
    }
}
