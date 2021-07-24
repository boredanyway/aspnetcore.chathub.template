using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorModal
{
    public class BlazorModalBase : ComponentBase
    {

        [Inject] public BlazorModalService BlazorModalService { get; set; }
        [Parameter] public RenderFragment BlazorModalHeader { get; set; }
        [Parameter] public RenderFragment BlazorModalBody { get; set; }
        [Parameter] public RenderFragment BlazorModalFooter { get; set; }
        [Parameter] public string ElementId { get; set; }

    }
}
