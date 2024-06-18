using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application;

public class LoteService : ILoteService
{
    private readonly IGeralPersist _geralPersist;
    private readonly ILotePersist _lotePersist;
    private readonly IMapper _mapper;
    public LoteService(IGeralPersist geralPersist, ILotePersist lotePersist, IMapper mapper)
    {
        _geralPersist = geralPersist;
        _lotePersist = lotePersist;
        _mapper = mapper;
    }

    public async Task AddLote(int eventoId, LoteDTO model)
    {
        try
        {
            var lote = _mapper.Map<Lote>(model);
            lote.EventoId = eventoId;

            _geralPersist.Add<Lote>(lote);

            await _geralPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<LoteDTO[]> SaveLote(int eventoId, LoteDTO[] models)
    {
        try
        {
            var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
            if (lotes is null) return null;

            foreach (var model in models)
            {
                if (model.Id == 0) await AddLote(eventoId, model);
                else
                {
                    var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);
                    model.EventoId = eventoId;

                    _mapper.Map(model, eventoId);

                    _geralPersist.Update<Lote>(lote);

                    await _geralPersist.SaveChangesAsync();
                }
            }

            var loteRetorno = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
            
            return _mapper.Map<LoteDTO[]>(loteRetorno);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteLote(int eventoId, int loteId)
    {
        try
        {
            var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote is null) throw new Exception("Lote para deletar não foi encontrado.");

            _geralPersist.Delete<Lote>(lote);
            return await _geralPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<LoteDTO[]> GetLotesByEventoIdAsync(int eventoId)
    {
        try
        {
            var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
            if (lotes is null) return null;

            var result = _mapper.Map<LoteDTO[]>(lotes);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<LoteDTO> GetLoteByIdsAsync(int eventoId, int loteId)
    {
        try
        {
            var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote is null) return null;

            var result = _mapper.Map<LoteDTO>(lote);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}