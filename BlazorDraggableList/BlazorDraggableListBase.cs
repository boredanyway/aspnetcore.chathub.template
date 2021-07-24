using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorDraggableList
{
    public partial class BlazorDraggableListBase<TItemGeneric> : ComponentBase, IDisposable
    {

        [Inject] BlazorDraggableListService BlazorDraggableListService { get; set; }

        [Parameter] public IList<TItemGeneric> Items { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public BlazorDraggableListOrientationType Orientation { get; set; }
        [Parameter] public RenderFragment<TItemGeneric> BlazorDraggableListItem { get; set; }

        protected override async Task OnInitializedAsync()
        {            
            this.BlazorDraggableListService.BlazorDraggableListServiceExtension.OnDropEvent += OnDropEventExecute;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await this.BlazorDraggableListService.InitDraggableList(this.Id);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void OnDropEventExecute(object sender, BlazorDraggableListEvent e)
        {
            if(e.DraggableContainerElementId == this.Id)
            {
                this.Items = this.Items.Swap(e.DraggableItemOldIndex, e.DraggableItemNewIndex);
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            this.BlazorDraggableListService.BlazorDraggableListServiceExtension.OnDropEvent -= OnDropEventExecute;
        }
    }

    public static class BlazorDraggableListExtension
    {
        public static IList<TItemGeneric> Swap<TItemGeneric>(this IList<TItemGeneric> list, int x, int y)
        {
            TItemGeneric temp = list[x];
            list[x] = list[y];
            list[y] = temp;
            return list;
        }
    }
}
