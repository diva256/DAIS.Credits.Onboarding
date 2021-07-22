using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace DAIS.Credits.Onboarding.WebAPI.Definition
{
    public class Startup
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                    {
                        NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy()
                    };
                    o.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddSwaggerDocument(document => document.DocumentName = "v1");
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
