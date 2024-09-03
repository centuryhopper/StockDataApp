
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Contexts;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public class AccountRepository(StockDataDbContext stockDataDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration) : IAccountRepository
{
    public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
    {
        if (loginDTO is null)
        {
            return new LoginResponse(false, null!, "Login container is empty");
        }

        var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
        if (getUser is null)
        {
            return new LoginResponse(false, null!, "User not found");
        }

        bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
        if (!checkUserPasswords)
        {
            return new LoginResponse(false, null!, "Invalid email/password");
        }

        var getUserRole = await userManager.GetRolesAsync(getUser);
        var userSession = new UserSession(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());
        string token = GenerateToken(userSession);

        // add user to stock user table if they arent in it already
        if (await stockDataDbContext.Stockusers.FirstOrDefaultAsync(u => u.UmsUserid == userSession.Id) is null)
        {
            try
            {
                await stockDataDbContext.Stockusers.AddAsync(new Stockuser {
                    UmsUserid = userSession.Id!
                    ,Email = userSession.Email!
                    ,DateCreated = DateTime.Now
                    ,DateLastLogin = DateTime.Now
                    ,DateRetired = null
                });
            }
            catch (System.Exception ex)
            {
                return new LoginResponse(false, null!, ex.Message);
            }
        }


        return new LoginResponse(true, token!, "Login completed");
    }

    private string GenerateToken(UserSession user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

