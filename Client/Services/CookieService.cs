using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Oqtane.ChatHubs
{
    public class CookieService : ServiceBase, IService
    {

        private IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference cookieScriptJsObjRef { get; set; }
        private IJSObjectReference cookieScriptMap { get; set; }

        public CookieService(HttpClient httpClient, IJSRuntime JSRuntime) : base(httpClient)
        {
            this.JSRuntime = JSRuntime;
        }

        public async Task InitCookieService()
        {
            this.cookieScriptJsObjRef = await this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Modules/Oqtane.ChatHubs/cookiejsinterop.js");
            this.cookieScriptMap = await this.cookieScriptJsObjRef.InvokeAsync<IJSObjectReference>("initcookie");
        }

        public async Task<string> GetCookieAsync(string cookieName)
        {
            return await this.cookieScriptMap.InvokeAsync<string>("getCookie", cookieName);
        }

        public async Task SetCookie(string cookieName, string cookieValue, int expirationDays)
        {
            await this.cookieScriptJsObjRef.InvokeVoidAsync("setCookie", cookieName);
        }

    }
}
