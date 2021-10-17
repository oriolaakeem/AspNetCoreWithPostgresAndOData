using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Students.Api.Brokers;
using Students.Api.Models;

namespace Students.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(mvcOptions => mvcOptions.EnableEndpointRouting = false);
            InitializeStorage(services);
            services.AddControllers()
            .AddOData(o =>
            {
                o.AddRouteComponents("odata", GetEdmModel());
                o.Select();
                o.Filter();
                o.Expand();
                o.Filter();
                o.OrderBy();
                o.Count();
                o.SetMaxTop(100);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMvc(routeBuilder =>
            //{
            //    routeBuilder.Select().Expand().Filter().Count();
            //    routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IEdmModel GetEdmModel()
        {
            var edmModelBuilder = new ODataConventionModelBuilder();
            edmModelBuilder.EntitySet<Student>("Students");

            return edmModelBuilder.GetEdmModel();
        }

        private void InitializeStorage(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<StorageBroker>(context => context.UseNpgsql(connectionString));
        }
    }
}
