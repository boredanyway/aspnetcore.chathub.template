using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorVideo
{
    public class BlazorVideoComponentBase : ComponentBase, IDisposable
    {

        [Inject] public BlazorVideoService BlazorVideoService { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string ConnectionId { get; set; }
        [Parameter] public string Name { get; set; }
        [Parameter] public BlazorVideoType Type { get; set; }
        [Parameter] public string BackgroundColor { get; set; }
        [Parameter] public BlazorVideoStatusType Status { get; set; }
        [Parameter] public int Framerate { get; set; }
        [Parameter] public int VideoBitsPerSecond { get; set; }
        [Parameter] public int AudioBitsPerSecond { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.BlazorVideoService.RunUpdateUI += UpdateUIStateHasChanged;

            await this.BlazorVideoService.InitBlazorVideo();
            await this.BlazorVideoService.InitBlazorVideoMap(this.Id, this.ConnectionId, this.Type, this.Framerate, this.VideoBitsPerSecond, this.AudioBitsPerSecond);

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.BlazorVideoService.ContinueLocalLivestreamAsync(this.Id, this.ConnectionId);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void UpdateUIStateHasChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            this.BlazorVideoService.RunUpdateUI -= UpdateUIStateHasChanged;
        }

    }
}
