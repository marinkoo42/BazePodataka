using AspNetCore.Identity.Neo4j;
using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using Neo4jClient;
using StackExchange.Redis;
using System.Text;


namespace BazePodatakaProjekat
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddSingleton(s => GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "edukacija")));
            services.AddScoped(s => s.GetService<IDriver>().AsyncSession());


            var client = new BoltGraphClient(new Uri("bolt://localhost:7687"), "neo4j", "edukacija");
            client.ConnectAsync();
            services.AddSingleton<IGraphClient>(client);


            services.AddEndpointsApiExplorer();


            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


            })
            .AddJwtBearer(options =>
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


            services.AddIdentity<AppUser, Neo4jIdentityRole>()
                        .AddNeo4jDataStores()
                        .AddDefaultTokenProviders();


            //redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                              ConnectionMultiplexer.Connect("localhost:6379"));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSignalRSwaggerGen();
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors(p =>
            {
                p.AddPolicy("CORS",
                      builder =>
                      {
                          builder.AllowAnyOrigin();
                          builder.AllowAnyMethod();
                          builder.AllowAnyHeader();

                      });
            });

            services.AddSignalR();

            services.AddTransient<ChatHub.ChatHub>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NaissusEvents v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthentication();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub.ChatHub>("/chat");

            });
        }




    }
}
