using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Contexto;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence;

public class EventoPersist : IEventoPersist
{
    private readonly ProEventosContext _context;
    public EventoPersist(ProEventosContext context)
    {
        _context = context;
    }

    public async Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
    {
        IQueryable<Evento> query = _context.Eventos.AsNoTracking()
            .Include(e => e.Lotes)
            .Include(e => e.RedesSociais);

        if (includePalestrantes)
        {
            query = query
                .Include(e => e.PalestrantesEventos)
                .ThenInclude(p => p.Palestrante);
        }

        query = query.AsNoTracking()
                    .Where(e => (e.Tema.ToLower().Contains(pageParams.Term.ToLower()) ||
                                e.Local.ToLower().Contains(pageParams.Term.ToLower())) && 
                                e.UserId == userId)
                     .OrderBy(e => e.Id);

        return await PageList<Evento>.CreateAsync(query, pageParams.PageNumber, pageParams.pageSize);
    }

    public async Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
    {
        IQueryable<Evento> query = _context.Eventos.AsNoTracking()
            .Include(e => e.Lotes)
            .Include(e => e.RedesSociais);

        if (includePalestrantes)
        {
            query = query
                .Include(e => e.PalestrantesEventos)
                .ThenInclude(p => p.Palestrante);
        }

        query = query
            .OrderBy(e => e.Id)
            .AsNoTracking()
            .Where(e => e.Id == eventoId && e.UserId == userId);

        return await query.FirstOrDefaultAsync();
    }
}
