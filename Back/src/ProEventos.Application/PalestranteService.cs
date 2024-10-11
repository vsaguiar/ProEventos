using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;

namespace ProEventos.Application;

public class PalestranteService : IPalestranteService
{
    private readonly IPalestrantePersist _palestrantePersist;
    private readonly IMapper _mapper;
    public PalestranteService(IPalestrantePersist palestrantePersist, IMapper mapper)
    {
        _palestrantePersist = palestrantePersist;
        _mapper = mapper;
    }

    public async Task<PalestranteDTO> AddPalestrantes(int userId, PalestranteAddDTO model)
    {
        try
        {
            var palestrante = _mapper.Map<Palestrante>(model);
            palestrante.UserId = userId;

            _palestrantePersist.Add<Palestrante>(palestrante);

            if (await _palestrantePersist.SaveChangesAsync())
            {
                var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
                return _mapper.Map<PalestranteDTO>(palestranteRetorno);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PalestranteDTO> UpdatePalestrante(int userId, PalestranteUpdateDTO model)
    {
        try
        {
            var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
            if (palestrante is null) return null;

            model.Id = palestrante.Id;
            model.UserId = userId;

            _mapper.Map(model, palestrante);

            _palestrantePersist.Update<Palestrante>(palestrante);

            if (await _palestrantePersist.SaveChangesAsync())
            {
                var palestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
                return _mapper.Map<PalestranteDTO>(palestranteRetorno);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PageList<PalestranteDTO>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
    {
        try
        {
            var palestrantes = await _palestrantePersist.GetAllPalestrantesAsync(pageParams, includeEventos);
            if (palestrantes is null) return null;

            var result = _mapper.Map<PageList<PalestranteDTO>>(palestrantes);

            result.CurrentPage = palestrantes.CurrentPage;
            result.TotalPages = palestrantes.TotalPages;
            result.PageSize = palestrantes.PageSize;
            result.TotalCount = palestrantes.TotalCount;

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PalestranteDTO> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
    {
        try
        {
            var palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, includeEventos);
            if (palestrante is null) return null;

            var result = _mapper.Map<PalestranteDTO>(palestrante);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
}