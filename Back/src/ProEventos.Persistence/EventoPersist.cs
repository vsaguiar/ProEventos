using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Contexto;

namespace ProEventos.Persistence;

public class EventoPersist : IEventoPersist
{
    private readonly ProEventosContext _context;
    public EventoPersist(ProEventosContext context)
    {
        _context = context;
    }

    public async Task<Evento[]> GetAllEventosAsync(int userId, bool includePalestrantes = false)
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

        query = query.Where(e => e.UserId == userId)
                     .OrderBy(e => e.Id)
                     .AsNoTracking();

        return await query.ToArrayAsync();
    }

    public async Task<Evento[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
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
            .Where(e => e.Tema.ToLower().Contains(tema.ToLower()) && 
                        e.UserId == userId);

        return await query.ToArrayAsync();
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
