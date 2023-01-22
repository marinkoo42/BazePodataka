/*using BazePodatakaProjekat.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Neo4jClient;
using StackExchange.Redis;



using BazePodatakaProjekat.ChatHub;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;

using Neo4j.Driver;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

public IConfiguration Configuration { get; }
// Add services to the container.

//neo4j 

*//*builder.Services.AddScoped<IGraphClient, GraphClient>(provider =>
{
   // var options = provider.GetService<IOptions<Neo4jDbSettings>>();
    //var client = new GraphClient(new Uri(options.Value.uri),
      //  username: options.Value.username, password: options.Value.password);
    var client = new GraphClient(new Uri("http://localhost:7687"), "neo4j", "edukacija");
    client.ConnectAsync().Wait();

    return client;
});*//*

//var cl = new GraphClient(new Uri("bolt://localhost:7687"));
builder.Services.AddSingleton(s => GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "edukacija")));
builder.Services.AddScoped(s => s.GetService<IDriver>().AsyncSession());


var client = new BoltGraphClient(new Uri("bolt://localhost:7687"), "neo4j", "edukacija");
await client.ConnectAsync();
builder.Services.AddSingleton<IGraphClient>(client);


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = Configuration["JWT:ValidAudience"],
        ValidIssuer = Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
    };
});


builder.Services.AddIdentity<AppUser, Neo4jIdentityRole>()
            .AddNeo4jDataStores()
            .AddDefaultTokenProviders();

*//*
builder.Services.AddScoped<IUserStore<Neo4jIdentityUser>, Neo4jUserStore<Neo4jIdentityUser, Neo4jIdentityRole>>();
builder.Services.AddScoped<IRoleStore<Neo4jIdentityRole>, Neo4jRoleStore<Neo4jIdentityRole, Neo4jIdentityRoleClaim>>();
builder.Services.AddScoped<UserManager<Neo4jIdentityUser>>();
*//*

//redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                  ConnectionMultiplexer.Connect("localhost:6379"));


*//*builder.Services.AddNeo4jAnnotations<ApplicationContext>();*/

/*builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //var dataProtectionPath = Path.Combine(HostingEnvironment.WebRootPath, "identity-artifacts");
    //options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
    //options.Cookies.ApplicationCookie.DataProtectionProvider = DataProtectionProvider.Create(dataProtectionPath);

    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddUserStore<UserStore<AppUser>>()
.AddRoleStore<RoleStore<IdentityRole>>()
;
*//*



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSignalRSwaggerGen();
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(p =>
{
    p.AddPolicy("CORS",
          builder =>
          {
              builder.AllowAnyOrigin();
              builder.AllowAnyMethod();
              builder.AllowAnyHeader();
              builder.AllowCredentials();
          });
});

builder.Services.AddSignalR();
var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseNeo4jIdentity();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
   
});



app.UseCors("CORS");
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BazePodatakaProjekat;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BazePodatakaProjekat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
