using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.API.Helpers;
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
    private readonly IUtil _util;
    private readonly string destino = "Perfil";

    public AccountController(IAccountService accountService, ITokenService tokenService, IUtil util)
    {
        _accountService = accountService;
        _tokenService = tokenService;
        _util = util;
    }


    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var userName = User.GetUserNameExtensios();

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
            if (user is not null)
                return Ok(new
                {
                    userName = user.UserName,
                    PrimeiroNome = user.PrimeiroNome,
                    token = _tokenService.CreateTokenAsync(user).Result
                });

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

            return Ok(new
            {
                userName = user.UserName,
                PrimeiroNome = user.PrimeiroNome,
                token = _tokenService.CreateTokenAsync(user).Result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar realizar login. Erro: {ex.Message}");
        }
    }


    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser(UserUpdateDTO userUpdateDTO)
    {
        try
        {
            if (userUpdateDTO.UserName != User.GetUserNameExtensios()) return Unauthorized("Usuário Inválido");

            var user = await _accountService.GetUserByUserNameAsync(User.GetUserNameExtensios());
            if (user is null) return Unauthorized("Usuário inválido.");

            var userReturn = await _accountService.UpdateAccountAsync(userUpdateDTO);
            if (userReturn is null) return NoContent();

            return Ok(new
            {
                userName = userReturn.UserName,
                PrimeiroNome = userReturn.PrimeiroNome,
                token = _tokenService.CreateTokenAsync(userReturn).Result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar usuário. Erro: {ex.Message}");
        }
    }


    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage()
    {
        try
        {
            var user = await _accountService.GetUserByUserNameAsync(User.GetUserNameExtensios());
            if (user is null) return NoContent();

            var file = Request.Form.Files[0];
            if (file.Length > 0)
            {
                _util.DeleteImage(user.ImagemURL, destino);
                user.ImagemURL = await _util.SaveImage(file, destino);
            }
            var userRetorno = await _accountService.UpdateAccountAsync(user);

            return Ok(userRetorno);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar realizar upload de foto do usuário. Erro: {ex.Message}");
        }
    }
}
