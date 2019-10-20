using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SpaceApp.Game.AppServices.Communication;
using SpaceApp.Game.AppServices.DataServices;
using SpaceApp.Game.AppServices.MovementServices;
using SpaceApp.Game.Communication;
using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using System.Linq;

namespace SpaceApp.Game
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private GameEngine GetInitialGameEngine()
        {
            var gameEngine = new GameEngine();
            var roomName = PreProcessService.GenerateRoomName();
            var gameRoom = new GameRoom(roomName);

            var spaceScrap = new NasaDataService().GetSpaceScrap();

            gameRoom.Game.Load(spaceScrap.Select(scrap => 
                GameObject.CreateScrap(new Position(scrap.X, scrap.Y),scrap.SizeCoeficient, scrap.Name)).ToList());

            gameEngine.AddNewRoom(gameRoom);

            return gameEngine;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var gameEngine = GetInitialGameEngine();

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR()
                .AddJsonProtocol(o =>
                {
                    o.PayloadSerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSingleton(gameEngine);
            services.AddScoped<NasaDataService>();
            services.AddScoped<GameChangerService>();
            services.AddScoped<CommunicationService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Game API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());

            app.UseSignalR(x => x.MapHub<GameHub>("/gamehub"));
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
        }
    }
}
