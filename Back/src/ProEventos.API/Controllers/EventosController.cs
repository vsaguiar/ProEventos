using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;

namespace ProEventos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;
    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var eventos = await _eventoService.GetAllEventosAsync(true);
            if (eventos is null) return NotFound("Nenhum evento encontrado.");

            return Ok(eventos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var evento = await _eventoService.GetEventoByIdAsync(id);
            if (evento is null) return NotFound("Evento por Id não encontrado.");

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar um evento. Erro: {ex.Message}");
        }
    }


    [HttpGet("{tema}/tema")]
    public async Task<IActionResult> GetByTema(string tema)
    {
        try
        {
            var evento = await _eventoService.GetAllEventosByTemaAsync(tema);
            if (evento is null) return NotFound("Eventos por tema não encontrados.");

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> Post(EventoDTO model)
    {
        try
        {
            var evento = await _eventoService.AddEvento(model);
            if (evento is null) return BadRequest("Erro ao tentar incluir um evento.");

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar incluir um evento. Erro: {ex.Message}");
        }
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, EventoDTO model)
    {
        try
        {
            var evento = await _eventoService.UpdateEvento(id, model);
            if (evento is null) return BadRequest("Erro ao tentar atualizar um evento.");

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar um evento. Erro: {ex.Message}");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (await _eventoService.DeleteEvento(id))
                return Ok("Deletado.");
            else
                return BadRequest("Evento não deletado.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar um evento. Erro: {ex.Message}");
        }
    }
}
