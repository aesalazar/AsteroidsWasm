using Microsoft.AspNetCore.Components.Builder;

namespace Asteroids.Blazor
{
    public class Startup
    {
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
