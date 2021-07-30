using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorVideo
{
    public class BlazorVideoComponentBase : ComponentBase, IDisposable
    {

        [Inject] public BlazorVideoService BlazorVideoService { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string ConnectionId { get; set; }
        [Parameter] public string Name { get; set; }
        [Parameter] public string BackgroundColor { get; set; }
        [Parameter] public BlazorVideoType Type { get; set; }
        [Parameter] public BlazorVideoStatusType Status { get; set; }
        [Parameter] public int Viewers { get; set; }
        [Parameter] public int Framerate { get; set; }
        [Parameter] public int VideoBitsPerSecond { get; set; }
        [Parameter] public int AudioBitsPerSecond { get; set; }
        [Parameter] public int VideoSegmentsLength { get; set; }

        public bool IsVideoOverlay { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            this.BlazorVideoService.RunUpdateUI += UpdateUIStateHasChanged;

            await this.BlazorVideoService.InitBlazorVideo();
            await this.BlazorVideoService.InitBlazorVideoMap(this.Id, this.ConnectionId, this.Type, this.Framerate, this.VideoBitsPerSecond, this.AudioBitsPerSecond, this.VideoSegmentsLength);

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
            var maps = BlazorVideoService.BlazorVideoMaps.Where(item => item.Value.Id == Id && item.Value.ConnectionId == ConnectionId).ToList();
            if(maps.Any())
            {
                var keyvaluepair = maps.FirstOrDefault();
                this.BlazorVideoService.BlazorVideoMaps[keyvaluepair.Key] = new BlazorVideoModel() { Id = keyvaluepair.Value.Id, ConnectionId = keyvaluepair.Value.ConnectionId, JsObjRef = keyvaluepair.Value.JsObjRef, Type = keyvaluepair.Value.Type, VideoOverlay = true };
            }

            this.BlazorVideoService.RunUpdateUI -= UpdateUIStateHasChanged;
        }

    }
}
