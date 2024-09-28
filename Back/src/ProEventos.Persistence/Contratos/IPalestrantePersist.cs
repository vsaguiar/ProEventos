using ProEventos.Domain;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence.Contratos;

public interface IPalestrantePersist : IGeralPersist
{
    Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false);
    Task<Palestrante> GetPalestranteByIdAsync(int userId, bool includeEventos = false);
}
