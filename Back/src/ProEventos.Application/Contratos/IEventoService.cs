using ProEventos.Application.DTOs;
using ProEventos.Persistence.Models;

namespace ProEventos.Application.Contratos;

public interface IEventoService
{
    Task<EventoDTO> AddEvento(int userId, EventoDTO model);
    Task<EventoDTO> UpdateEvento(int userId, int eventoId, EventoDTO model);
    Task<bool> DeleteEvento(int userId, int eventoId);

    Task<PageList<EventoDTO>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false);
    Task<EventoDTO> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);
}
