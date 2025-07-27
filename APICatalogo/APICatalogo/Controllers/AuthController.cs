using APICatalogo.Configuracoes;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly JwtSection _jwtOptions;
    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IOptions<JwtSection> jwtOptions)
        => (_tokenService, _userManager, _roleManager, _configuration, _jwtOptions) =
           (tokenService, userManager, roleManager, configuration, jwtOptions.Value);

    ///<summary>
    ///Verifica as credenciais de um usuário
    ///</summary>
    ///<param name="model">Um objeto do tipo UsuarioDTO</param>
    ///<returns>Status 200 e um token para o cliente</returns>
    ///<remarks>Retorna o Status 200 e o token</remarks>
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.UserName!),
                new("id", user.UserName!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = _tokenService.GenerateAcessToken(authClaims);

            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_jwtOptions.RefreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }


    ///<summary>
    ///Registra um novo usuário
    ///</summary>
    ///<param name="model">Um objeto UsuarioDTO</param>
    ///<returns>Status 200</returns>
    ///<remarks>retorna o Status 200</remarks>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName!);

        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Usuário já existe na base!" });

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = $"A criação do usuário falhou! {string.Join(" , ", result.Errors.Select(e => e.Description))}" });

        return Ok(new Response { Status = "Success", Message = "A criação foi concluída com sucesso!" });
    }


    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel model)
    {
        if (model is null) return BadRequest("Requisição inválida");

        if (string.IsNullOrEmpty(model.AccessToken) || string.IsNullOrEmpty(model.RefreshToken))
            ArgumentException.ThrowIfNullOrEmpty(nameof(model));

        var principal = _tokenService.GetClaimsPrincipalFromExpiredToken(model.AccessToken!);

        if (principal is null) return BadRequest("Token ou RefreshToken inválido");

        string userName = principal.Identity?.Name ?? "";

        var user = await _userManager.FindByNameAsync(userName!);

        if (user is null ||
            user.RefreshToken == model.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Token ou RefreshToken inválido");

        var newAccessToken = _tokenService.GenerateAcessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new OkObjectResult(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
        });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{userName}")]
    [Authorize(Policy = "ExclusiveOnly")]
    public async Task<IActionResult> Revoke(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest("Nome do usuário inválido");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
        return NoContent();
    }

    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (roleExist) return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Função já existe" });

        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

        return roleResult.Succeeded ?
            StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Função {roleName} adicionada com sucesso!" }) :
            StatusCode(StatusCodes.Status200OK, new Response { Status = "Error", Message = $"Erro ao adicionar a nova função {roleName}" });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
       var user = await _userManager.FindByEmailAsync(email);

        if (user is null) return BadRequest(new { error = "Não foi possível encontrar o usuário" });
        
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded ?
          StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Usuário {user.Email} adicionado a função {roleName}" }) :
          StatusCode(StatusCodes.Status200OK, new Response { Status = "Error", Message = $"Erro ao adicionar a função {roleName} ao usuário {user.Email}" });

    }

}
