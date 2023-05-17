using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      //policy.WithOrigins("http://127.0.0.1:5500/");
                                      policy.SetIsOriginAllowed(ori => new Uri(ori).Host == "localhost" || new Uri(ori).Host == "127.0.0.1")
                                      .AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            services.AddControllers(opt =>
            {
                opt.Filters.Add(typeof(FiltrosDeExepcion));
            }).AddJsonOptions(x=>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddNewtonsoftJson();

            services.AddDbContext<AplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("defaultConecction"));
            });

            services.AddTransient<MiFiltroDeAccion>();
            //services.AddHostedService<EscribirEnArchivo>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAutoMapper(typeof(Startup));  
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLogguearRespuestaHttp();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
