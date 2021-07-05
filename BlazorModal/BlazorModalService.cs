using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorModal
{

    public class BlazorModalService : IAsyncDisposable
    {

        public IJSObjectReference Module { get; set; }
        public IJSRuntime JsRuntime { get; set; }
        public IJSObjectReference jsobjref { get; set; }

        public DotNetObjectReference<BlazorModalServiceExtension> DotNetObjectRef;
        public BlazorModalServiceExtension BlazorModalServiceExtension;

        public event Action RunUpdateUI;

        public BlazorModalService(IJSRuntime jsRuntime)
        {
            this.JsRuntime = jsRuntime;
            this.BlazorModalServiceExtension = new BlazorModalServiceExtension(this);
            this.DotNetObjectRef = DotNetObjectReference.Create(this.BlazorModalServiceExtension);
        }
        public async Task InitBlazorModal()
        {
            if(this.Module == null)
            {
                this.Module = await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorModal/blazormodaljsinterop.js");
            }
        }
        public async Task InitBlazorModalMap()
        {
            if(this.jsobjref == null)
            {
                this.jsobjref = await this.Module.InvokeAsync<IJSObjectReference>("initmodal", this.DotNetObjectRef);
            }
        }

        public async Task ShowModal(string id)
        {
            await this.jsobjref.InvokeVoidAsync("showmodal", id);
            this.RunUpdateUI?.Invoke();
        }

        public async Task HideModal(string id)
        {
            await this.jsobjref.InvokeVoidAsync("hidemodal", id);
            this.RunUpdateUI?.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            await this.Module.DisposeAsync();
        }
    }

    public class BlazorModalServiceExtension
    {

        public BlazorModalService BlazorModalService { get; set; }
        public event Action<string> OnModalShown;
        public event Action<string> OnModalHidden;

        public BlazorModalServiceExtension(BlazorModalService blazorModalService)
        {
            this.BlazorModalService = blazorModalService;
        }

        [JSInvokable("ModalShown")]
        public void ModalShown(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                this.OnModalShown.Invoke(id);
            }
        }

        [JSInvokable("ModalHidden")]
        public void ModalHidden(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                this.OnModalHidden.Invoke(id);
            }
        }

    }

}
