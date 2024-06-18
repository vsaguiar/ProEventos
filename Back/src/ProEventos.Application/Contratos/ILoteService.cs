using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos;

public interface ILoteService
{
    Task<LoteDTO[]> SaveLote(int eventoId, LoteDTO[] models);
    Task<bool> DeleteLote(int eventoId, int loteId);
    Task<LoteDTO[]> GetLotesByEventoIdAsync(int eventoId);
    Task<LoteDTO> GetLoteByIdsAsync(int eventoId, int loteId);
}
