using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Asteroids.Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Blazor.Extensions.Storage, both SessionStorage and LocalStorage are registered
            services.AddStorage();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
