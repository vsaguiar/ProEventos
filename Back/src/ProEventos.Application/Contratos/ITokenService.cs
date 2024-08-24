using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos;

public interface ITokenService
{
    Task<string> CreateTokenAsync(UserUpdateDTO userUpdateDTO);
}
