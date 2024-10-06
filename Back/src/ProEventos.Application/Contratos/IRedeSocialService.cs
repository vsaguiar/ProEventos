using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos;

public interface IRedeSocialService
{
    Task<RedeSocialDTO[]> SaveByEventoAsync(int eventoId, RedeSocialDTO[] models);
    Task<bool> DeleteByEventoAsync(int eventoId, int redeSocialId);
    Task<RedeSocialDTO[]> SaveByPalestranteAsync(int palestranteId, RedeSocialDTO[] models);
    Task<bool> DeleteByPalestranteAsync(int palestranteId, int redeSocialId);
    Task<RedeSocialDTO[]> GetAllByEventoIdAsync(int eventoId);
    Task<RedeSocialDTO[]> GetAllByPalestranteIdAsync(int palestranteId);
    Task<RedeSocialDTO> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId);
    Task<RedeSocialDTO> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId);
}
