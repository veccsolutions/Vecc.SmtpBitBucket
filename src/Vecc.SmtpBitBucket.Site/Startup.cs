using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vecc.SmtpBitBucket.Site.Api.V1.Services;
using Vecc.SmtpBitBucket.Site.Api.V1.Services.Internal;
using Vecc.SmtpBitBucket.Site.Services;

namespace Vecc.SmtpBitBucket.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddCors((options) =>
            {
                options.AddDefaultPolicy((policyBuilder) => {
                    policyBuilder.AllowAnyOrigin();
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyHeader();
                });
            });

            var serverOptions = new SmtpServerOptions();
            this.Configuration.GetSection("SmtpServer").Bind(serverOptions);

            switch (serverOptions.DataSource.ToLowerInvariant())
            {
                case "Postgres":
                    services.AddPostgresStores(this.Configuration.GetSection("PostgresConnection"));
                    break;
                default:
                    services.AddInMemoryStores();
                    break;
            }

            services.AddCoreBitBucket();
            services.Configure<SmtpServerOptions>(this.Configuration.GetSection("SmtpServer"));
            services.AddSingleton<IModelConverter, ModelConverter>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
