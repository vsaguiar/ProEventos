using Microsoft.AspNetCore.Identity;
using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos;
public interface IAccountService
{
    Task<bool> UserExistsAsync(string username);
    Task<UserUpdateDTO> GetUserByUserNameAsync(string username);
    Task<SignInResult> CheckUserPasswordAsync(UserUpdateDTO userUpdateDTO, string password);
    Task<UserUpdateDTO> CreateAccountAsync(UserDTO userDTO);
    Task<UserUpdateDTO> UpdateAccountAsync(UserUpdateDTO userUpdateDTO);
}
