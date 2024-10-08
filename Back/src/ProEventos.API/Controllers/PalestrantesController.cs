using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Persistence.Models;

namespace ProEventos.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PalestrantesController : ControllerBase
{
    private readonly IPalestranteService _palestranteService;
    private readonly IWebHostEnvironment _environment;
    private readonly IAccountService _accountService;
    public PalestrantesController(IPalestranteService palestranteService, IWebHostEnvironment environment, IAccountService accountService)
    {
        _palestranteService = palestranteService;
        _environment = environment;
        _accountService = accountService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery]PageParams pageParams)
    {
        try
        {
            var palestrantes = await _palestranteService.GetAllPalestrantesAsync(pageParams, true);
            if (palestrantes is null) return NoContent();

            Response.AddPagination(palestrantes.CurrentPage, palestrantes.PageSize, palestrantes.TotalCount, palestrantes.TotalPages);

            return Ok(palestrantes);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar palestrantes. Erro: {ex.Message}");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetPalestrantes()
    {
        try
        {
            var palestrantes = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserIdExtensios(), true);
            if (palestrantes is null) return NoContent();

            return Ok(palestrantes);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar um palestrantes. Erro: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> Post(PalestranteAddDTO model)
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserIdExtensios(), false);
            if (palestrante is null)
                palestrante = await _palestranteService.AddPalestrantes(User.GetUserIdExtensios(), model);

            return Ok(palestrante);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar incluir um palestrante. Erro: {ex.Message}");
        }
    }


    [HttpPut]
    public async Task<IActionResult> Put(int id, PalestranteUpdateDTO model)
    {
        try
        {
            var palestrante = await _palestranteService.UpdatePalestrante(User.GetUserIdExtensios(), model);
            if (palestrante is null) return NoContent();

            return Ok(palestrante);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar um palestrante. Erro: {ex.Message}");
        }
    }
}
