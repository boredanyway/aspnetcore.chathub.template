using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWindows
{
    public partial class WindowItemBase : ComponentBase, IDisposable, IWindowItem
    {

        [CascadingParameter] public WindowContainer WindowContainer { get; set; }

        [Parameter] public RenderFragment WindowTitle { get; set; }

        [Parameter] public RenderFragment WindowContent { get; set; }

        [Parameter] public RenderFragment WindowLivestream { get; set; }

        [Parameter] public int Id { get; set; }

        [Parameter] public bool IsActiveWindow { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.WindowContainer.AddWindowItem(this);
            await base.OnInitializedAsync();
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (this.IsActiveWindow)
            {
                this.WindowContainer.ActiveWindow = this;
            }

            base.OnAfterRender(firstRender);
        }

        public string TitleCssClass => this.WindowContainer.ActiveWindow == this ? "active" : null;

        public void ActivateWindow()
        {
            this.WindowContainer.ActiveWindow = this;
        }

        public void UpdateWindowContent()
        {            
            if(this.WindowContainer.ActiveWindow == this)
            {
                InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            this.WindowContainer.RemoveWindowItem(this.Id);
        }

    }
}
