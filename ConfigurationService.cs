using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using WebApiAdvance.DAL.EfCore;
using WebApiAdvance.DAL.UnitOfWork.Abstracts;
using WebApiAdvance.DAL.UnitOfWork.Cocretes;
using WebApiAdvance.Entities.Auth;

namespace WebApiAdvance
{
	public static class ConfigurationService
	{
		public static IServiceCollection AddApiConfiguration(this IServiceCollection services,IConfiguration configuration)
		{
			TokenOption tokenOption = configuration.GetSection("TokenOptions").Get<TokenOption>();
			services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(configuration["ConnectionStrings:Default"]);
			});
			services.AddAutoMapper(Assembly.GetExecutingAssembly());


			// Register and Login istifade edilende yazilmalidir
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
			services.AddAuthentication(opt =>
			{
				//bearer kitabxanasini yuklemek lazimdir.
				opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = true,
					ValidateIssuer = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = tokenOption.Issuer,
					ValidAudience = tokenOption.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey))


				};
			});
			//unit of workdan istifade edilir 
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
