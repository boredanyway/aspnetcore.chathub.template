using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Oqtane.ChatHubs.Services
{
    public class ScrollService : ServiceBase, IService
    {

        private readonly IJSRuntime JSRuntime;
        private IJSObjectReference scrollScriptJsObjRef { get; set; }
        private IJSObjectReference scrollScriptMap { get; set; }

        public ScrollService(HttpClient httpClient, IJSRuntime jsRuntime) : base(httpClient)
        {
            this.JSRuntime = jsRuntime;
        }

        public async Task InitScrollService()
        {
            this.scrollScriptJsObjRef = await this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Modules/Oqtane.ChatHubs/scrollservicejsinterop.js");
            this.scrollScriptMap = await this.scrollScriptJsObjRef.InvokeAsync<IJSObjectReference>("initscrollservice");
        }

        public async Task ScrollToBottom(string element)
        {
            await this.scrollScriptMap.InvokeAsync<object>("scrollToBottom", element);
        }

    }
}