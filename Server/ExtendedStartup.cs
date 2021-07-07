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
using Oqtane.ChatHubs.Hubs;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using System;

namespace Oqtane
{
    public class ExtendedStartup : IServerStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BlazorAlertsService, BlazorAlertsService>();
            services.AddScoped<BlazorDraggableListService, BlazorDraggableListService>();
            services.AddScoped<BlazorFileUploadService, BlazorFileUploadService>();
            services.AddScoped<BlazorColorPickerService, BlazorColorPickerService>();
            services.AddScoped<BlazorVideoService, BlazorVideoService>();
            services.AddScoped<BlazorBrowserResizeService, BlazorBrowserResizeService>();
            services.AddScoped<BlazorModalService, BlazorModalService>();

            services.AddServerSideBlazor()
                .AddHubOptions(options => options.MaximumReceiveMessageSize = 512 * 1024);

            services.AddMvc()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddSignalR()
                .AddHubOptions<ChatHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    options.MaximumReceiveMessageSize = Int64.MaxValue;
                    options.StreamBufferCapacity = Int32.MaxValue;
                })
                .AddMessagePackProtocol()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
