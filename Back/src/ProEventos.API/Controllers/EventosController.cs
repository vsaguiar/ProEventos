using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Context;
using ProEventos.API.Models;

namespace ProEventos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventosController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public IEnumerable<Evento> Get()
    {
        return _context.Eventos;
    }


    [HttpGet("{id}")]
    public Evento GetById(int id)
    {
        return _context.Eventos
        .Where(evento => evento.EventoId == id)
        .First();
    }
}
