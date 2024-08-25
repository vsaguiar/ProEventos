using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;

namespace ProEventos.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;
    private readonly IWebHostEnvironment _environment;
    private readonly IAccountService _accountService;
    public EventosController(IEventoService eventoService, IWebHostEnvironment environment, IAccountService accountService)
    {
        _eventoService = eventoService;
        _environment = environment;
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var eventos = await _eventoService.GetAllEventosAsync(User.GetUserIdExtensios(), true);
            if (eventos == null) return NoContent();

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


    [HttpGet("{tema}/tema")]
    public async Task<IActionResult> GetByTema(string tema)
    {
        try
        {
            var evento = await _eventoService.GetAllEventosByTemaAsync(User.GetUserIdExtensios(), tema, true);
            if (evento is null) return NoContent();

            return Ok(evento);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
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
                DeleteImage(evento.ImagemURL);
                evento.ImagemURL = await SaveImage(file);
            }
            var eventoRetorno = await _eventoService.UpdateEvento(User.GetUserIdExtensios(), eventoId, evento);

            return Ok(eventoRetorno);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar a imagem. Erro: {ex.Message}");
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
                DeleteImage(evento.ImagemURL);
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


    [NonAction]
    public async Task<string> SaveImage(IFormFile imageFile)
    {
        string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName)
            .Take(10)
            .ToArray()
        ).Replace(' ', '-');

        imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmssfff")}{Path.GetExtension(imageFile.FileName)}";

        var imagePath = Path.Combine(_environment.ContentRootPath, @"Resources/Images", imageName);

        using (var fileStream = new FileStream(imagePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return imageName;
    }

    [NonAction]
    public void DeleteImage(string imageName)
    {
        var imagePath = Path.Combine(_environment.ContentRootPath, @"Resources/Images", imageName);
        if (System.IO.File.Exists(imagePath))
            System.IO.File.Delete(imagePath);
    }
}
