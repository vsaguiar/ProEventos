using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;

namespace ProEventos.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;

    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
    }


    [HttpGet("GetUser/{userName}")]
    public async Task<IActionResult> GetUser(string userName)
    {
        try
        {
            var user = await _accountService.GetUserByUserNameAsync(userName);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
        }
    }


    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserDTO userDTO)
    {
        try
        {
            if (await _accountService.UserExistsAsync(userDTO.UserName)) 
                return BadRequest("Usuário já existe.");

            var user = await _accountService.CreateAccountAsync(userDTO);
            if (user is not null) return Ok(user);
            
            return BadRequest("Usuário não criado! Tente novamente mais tarde.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar registrar um novo usuário. Erro: {ex.Message}");
        }
    }


    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
    {
        try
        {
            var user = await _accountService.GetUserByUserNameAsync(userLoginDTO.Username);
            if (user is null) return Unauthorized("Usuário ou Senha inválidos.");

            var result = await _accountService.CheckUserPasswordAsync(user, userLoginDTO.Password);
            if (!result.Succeeded) return Unauthorized();

            return Ok( new {
                userName = user.UserName,
                PrimeiroNome = user.PrimeiroNome,
                token = _tokenService.CreateTokenAsync(user).Result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar retornar usuário. Erro: {ex.Message}");
        }
    }
}
