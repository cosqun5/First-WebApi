﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAdvance.Entities.Auth;
using WebApiAdvance.Entities.Dtos.Auth;

namespace WebApiAdvance.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly TokenOption _tokenOption;

		public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_configuration = configuration;
			_tokenOption = configuration.GetSection("TokenOptions").Get<TokenOption>();
		}
		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			AppUser appUser = _mapper.Map<AppUser>(registerDto);
			var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
			if (!createUserResult.Succeeded)
			{
				return BadRequest(new
				{
					statusCode =400,
					errors = createUserResult.Errors
				});
			}
			await _roleManager.CreateAsync(new IdentityRole { Name = "user" });
		 var addRoleResult = await	_userManager.AddToRoleAsync(appUser,"user");
			if (!addRoleResult.Succeeded)
			{
				return BadRequest(new
				{
					statusCode = 400,
					errors = createUserResult.Errors
				});
			}
			return Ok(new
			{
				message = "User Created"
			});
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			AppUser appUser = await _userManager.FindByNameAsync(loginDto.UserName);
			if (appUser is null) return NotFound();
			if (!await _userManager.CheckPasswordAsync(appUser,loginDto.Password))
			{
				return Unauthorized();
			}
			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.SecurityKey));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256Signature);
			JwtHeader header = new JwtHeader(signingCredentials);
			List<Claim> claims = new List<Claim>() 
			{
			 new Claim(ClaimTypes.NameIdentifier,appUser.Id),
			 new Claim(ClaimTypes.Name,appUser.UserName),
			 //new Claim(ClaimTypes.Expiration,DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration).Ticks.ToString()),
			 new Claim("FullName",appUser.FullName)
			};
			IList<string> roles = await _userManager.GetRolesAsync(appUser);
			foreach (string role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role,role));	
			}
			JwtPayload payload = new JwtPayload(
				issuer: _tokenOption.Issuer,
				audience: _tokenOption.Audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration));
			JwtSecurityToken securityToken = new JwtSecurityToken(header,payload);
			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			string token = handler.WriteToken(securityToken);
			return Ok(new
			{
				token = token,
				expires = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration)
			});

		}
	}
}
