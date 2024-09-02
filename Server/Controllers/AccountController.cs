using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountRepository accountRepository) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        var response = await accountRepository.LoginAccount(loginDTO);
        return Ok(response);
    }
}
