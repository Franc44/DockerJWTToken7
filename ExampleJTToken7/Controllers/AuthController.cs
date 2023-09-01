using ExampleJTToken7.Models;
using ExampleJTToken7.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExampleJTToken7.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase 
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly UsersContext _usersContext;
    private readonly TokenService _tokenService;
    public AuthController(UserManager<IdentityUser> userManager, UsersContext usersContext, TokenService tokenService)
    {
        _userManager = userManager;
        _usersContext = usersContext;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userManager.CreateAsync(
            new IdentityUser { UserName = request.Username, Email = request.Email },
            request.Password
        );

        if(result.Succeeded)
        {
            request.Password = "";
            return CreatedAtAction(nameof(Register), new { request.Email}, request);
        }

        foreach(var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);           
        }
        return BadRequest(ModelState);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody]AuthRequest request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var managedUser = await _userManager.FindByEmailAsync(request.Email);

        if(managedUser == null)
        {
            return BadRequest("User or password are wrong! Please, try again");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
        if(!isPasswordValid)
        {
            return BadRequest("User or password are wrong! Please, try again");
        }

        var userInDB = _usersContext.Users.FirstOrDefault(x => x.Email == request.Email);
        if(userInDB == null)
        {
            return Unauthorized();
        }

        var accessToken = _tokenService.CreateToken(userInDB);

        return Ok(new AuthResponse
            {
                UserName = userInDB.UserName,
                Email = userInDB.Email,
                Token = accessToken
            }
        );
    }
}