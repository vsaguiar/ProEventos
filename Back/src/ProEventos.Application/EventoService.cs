using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application;

public class EventoService : IEventoService
{
    private readonly IGeralPersist _geralPersist;
    private readonly IEventoPersist _eventoPersist;
    private readonly IMapper _mapper;
    public EventoService(IGeralPersist geralPersist, IEventoPersist eventoPersist, IMapper mapper)
    {
        _geralPersist = geralPersist;
        _eventoPersist = eventoPersist;
        _mapper = mapper;
    }

    public async Task<EventoDTO> AddEvento(EventoDTO model)
    {
        try
        {
            var evento = _mapper.Map<Evento>(model);

            _geralPersist.Add<Evento>(evento);

            if (await _geralPersist.SaveChangesAsync())
            {
                var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false);
                return _mapper.Map<EventoDTO>(eventoRetorno);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDTO> UpdateEvento(int eventoId, EventoDTO model)
    {
        try
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false);
            if (evento is null) return null;

            model.Id = eventoId;

            _mapper.Map(model, evento);

            _geralPersist.Update<Evento>(evento);

            if (await _geralPersist.SaveChangesAsync())
            {
                var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false);
                return _mapper.Map<EventoDTO>(eventoRetorno);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteEvento(int eventoId)
    {
        try
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false);
            if (evento is null) throw new Exception("Evento para deletar não foi encontrado.");

            _geralPersist.Delete(evento);
            return await _geralPersist.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDTO[]> GetAllEventosAsync(bool includePalestrantes = false)
    {
        try
        {
            var eventos = await _eventoPersist.GetAllEventosAsync(includePalestrantes);
            if (eventos is null) return null;

            var result = _mapper.Map<EventoDTO[]>(eventos);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDTO[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
    {
        try
        {
            var eventos = await _eventoPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
            if (eventos is null) return null;

            var result = _mapper.Map<EventoDTO[]>(eventos);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDTO> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
    {
        try
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
            if (evento is null) return null;

            var result = _mapper.Map<EventoDTO>(evento);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}