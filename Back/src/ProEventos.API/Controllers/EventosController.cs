using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.API.Helpers;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Persistence.Models;

namespace ProEventos.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;
    private readonly IUtil _util;
    private readonly IAccountService _accountService;
    private readonly string destino = "Images";
    public EventosController(IEventoService eventoService, IUtil util, IAccountService accountService)
    {
        _eventoService = eventoService;
        _util = util;
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
    {
        try
        {
            var eventos = await _eventoService.GetAllEventosAsync(User.GetUserIdExtensios(), pageParams, true);
            if (eventos == null) return NoContent();

            Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);

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
            var evento = await _eventoService.GetEventoByIdAsync(User.GetUserIdExtensios(), id, true);
            if (evento is null) return NoContent();

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar um evento. Erro: {ex.Message}");
        }
    }


    [HttpPost("upload-image/{eventoId}")]
    public async Task<IActionResult> UploadImage(int eventoId)
    {
        try
        {
            var evento = await _eventoService.GetEventoByIdAsync(User.GetUserIdExtensios(), eventoId, true);
            if (evento is null) return NoContent();

            var file = Request.Form.Files[0];
            if (file.Length > 0)
            {
                _util.DeleteImage(evento.ImagemURL, destino);
                evento.ImagemURL = await _util.SaveImage(file, destino);
            }
            var eventoRetorno = await _eventoService.UpdateEvento(User.GetUserIdExtensios(), eventoId, evento);

            return Ok(eventoRetorno);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar a imagem do evento. Erro: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> Post(EventoDTO model)
    {
        try
        {
            var evento = await _eventoService.AddEvento(User.GetUserIdExtensios(), model);
            if (evento is null) return NoContent();

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
            var evento = await _eventoService.UpdateEvento(User.GetUserIdExtensios(), id, model);
            if (evento is null) return NoContent();

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
            var evento = await _eventoService.GetEventoByIdAsync(User.GetUserIdExtensios(), id, true);
            if (evento is null) return NoContent();

            if (await _eventoService.DeleteEvento(User.GetUserIdExtensios(), id))
            {
                _util.DeleteImage(evento.ImagemURL, destino);
                return Ok(new { message = "Deletado" });
            }
            else
                throw new Exception("Ocorreu um erro ao tentar deletar o evento.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar um evento. Erro: {ex.Message}");
        }
    }
}
