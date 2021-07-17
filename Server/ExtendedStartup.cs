using BlazorAlerts;
using BlazorBrowserResize;
using BlazorColorPicker;
using BlazorDraggableList;
using BlazorFileUpload;
using BlazorModal;
using BlazorVideo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Oqtane.ChatHubs.Caching;
using Oqtane.ChatHubs.Hubs;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using System;

namespace Oqtane
{
    public class ExtendedStartup : IServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddMemoryCache();
            services.AddSingleton<ChatHubCachingService, ChatHubCachingService>();

            services.AddScoped<BlazorAlertsService, BlazorAlertsService>();
            services.AddScoped<BlazorDraggableListService, BlazorDraggableListService>();
            services.AddScoped<BlazorFileUploadService, BlazorFileUploadService>();
            services.AddScoped<BlazorColorPickerService, BlazorColorPickerService>();
            services.AddScoped<BlazorVideoService, BlazorVideoService>();
            services.AddScoped<BlazorBrowserResizeService, BlazorBrowserResizeService>();
            services.AddScoped<BlazorModalService, BlazorModalService>();

            services.AddServerSideBlazor()
                .AddHubOptions(options => options.MaximumReceiveMessageSize = 512 * 1024);

            services.AddSignalR()
                .AddHubOptions<ChatHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    options.MaximumReceiveMessageSize = Int64.MaxValue;
                    options.StreamBufferCapacity = Int32.MaxValue;
                })
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseTenantResolution();
            app.UseBlazorFrameworkFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                    options.ApplicationMaxBufferSize = Int64.MaxValue;
                    options.TransportMaxBufferSize = Int64.MaxValue;
                    options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(10);
                    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
                });
            });
        }
        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {

        }
    }
}
