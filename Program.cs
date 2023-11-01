using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using WebApiAdvance.DAL.EfCore;
using WebApiAdvance.DAL.Repositories.Abstracts;
using WebApiAdvance.DAL.Repositories.Concretes.EfCore;
using WebApiAdvance.Entities.Auth;

namespace WebApiAdvance
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			TokenOption tokenOption = builder.Configuration.GetSection("TokenOptions").Get<TokenOption>();


			//Validationslar tetbiq edilende yazilmlidir
			builder.Services.AddControllers().AddFluentValidation(opt =>
			{
				opt.ImplicitlyValidateChildProperties = true;
				opt.ImplicitlyValidateRootCollectionElements= true;
				opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			});

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
			});
			builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


			// Register and Login istifade edilende yazilmalidir
			builder.Services.AddIdentity<AppUser,IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
			builder.Services.AddAuthentication(opt =>
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
					ValidateLifetime= true,
					ValidateIssuerSigningKey = true,
					ValidIssuer=tokenOption.Issuer,
					ValidAudience=tokenOption.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey))


			};
			});

			builder.Services.AddScoped<IProductRepository,EfProductRepository>();
			var app = builder.Build();


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}