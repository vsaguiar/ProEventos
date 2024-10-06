using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application;

public class RedeSocialService : IRedeSocialService
{
    private readonly IRedeSocialPersist _redeSocialPersist;
    private readonly IMapper _mapper;
    public RedeSocialService(IRedeSocialPersist redeSocialPersist, IMapper mapper)
    {
        _redeSocialPersist = redeSocialPersist;
        _mapper = mapper;
    }

    public async Task AddRedeSocial(int Id, RedeSocialDTO model, bool isEvento)
    {
        try
        {
            var RedeSocial = _mapper.Map<RedeSocial>(model);
            if (isEvento)
            {
                RedeSocial.EventoId = Id;
                RedeSocial.PalestranteId = null;
            }
            else
            {
                RedeSocial.EventoId = null;
                RedeSocial.PalestranteId = Id;
            }

            _redeSocialPersist.Add<RedeSocial>(RedeSocial);
            await _redeSocialPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO[]> SaveByEventoAsync(int eventoId, RedeSocialDTO[] models)
    {
        try
        {
            var redeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
            if (redeSocials is null) return null;

            foreach (var model in models)
            {
                if (model.Id == 0)
                    await AddRedeSocial(eventoId, model, true);
                else
                {
                    var RedeSocial = redeSocials.FirstOrDefault(RedeSocial => RedeSocial.Id == model.Id);
                    model.EventoId = eventoId;

                    _mapper.Map(model, RedeSocial);

                    _redeSocialPersist.Update<RedeSocial>(RedeSocial);

                    await _redeSocialPersist.SaveChangesAsync();
                }
            }
            var redeSocialRetorno = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);

            return _mapper.Map<RedeSocialDTO[]>(redeSocialRetorno);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO[]> SaveByPalestranteAsync(int palestranteId, RedeSocialDTO[] models)
    {
        try
        {
            var redeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
            if (redeSocials is null) return null;

            foreach (var model in models)
            {
                if (model.Id == 0)
                    await AddRedeSocial(palestranteId, model, false);
                else
                {
                    var RedeSocial = redeSocials.FirstOrDefault(RedeSocial => RedeSocial.Id == model.Id);
                    model.PalestranteId = palestranteId;

                    _mapper.Map(model, RedeSocial);
                    _redeSocialPersist.Update<RedeSocial>(RedeSocial);
                    await _redeSocialPersist.SaveChangesAsync();
                }
            }
            var redeSocialRetorno = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);

            return _mapper.Map<RedeSocialDTO[]>(redeSocialRetorno);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteByEventoAsync(int eventoId, int redeSocialId)
    {
        try
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
            if (redeSocial is null) throw new Exception("Rede Social por Evento para delete não encontrado.");

            _redeSocialPersist.Delete<RedeSocial>(redeSocial);
            return await _redeSocialPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteByPalestranteAsync(int palestranteId, int redeSocialId)
    {
        try
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
            if (redeSocial is null) throw new Exception("Rede Social por Palestrante para delete não encontrado.");

            _redeSocialPersist.Delete<RedeSocial>(redeSocial);
            return await _redeSocialPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO[]> GetAllByEventoIdAsync(int eventoId)
    {
        try
        {
            var redeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
            if (redeSocials is null) return null;

            var resultado = _mapper.Map<RedeSocialDTO[]>(redeSocials);
            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO[]> GetAllByPalestranteIdAsync(int palestranteId)
    {
        try
        {
            var redeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
            if (redeSocials is null) return null;

            var resultado = _mapper.Map<RedeSocialDTO[]>(redeSocials);
            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId)
    {
        try
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
            if (redeSocial is null) return null;

            var resultado = _mapper.Map<RedeSocialDTO>(redeSocial);
            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<RedeSocialDTO> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId)
    {
        try
        {
            var redeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId);
            if (redeSocial is null) return null;

            var resultado = _mapper.Map<RedeSocialDTO>(redeSocial);
            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}