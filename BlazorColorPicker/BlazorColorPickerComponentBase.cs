using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorColorPicker
{
    public class BlazorColorPickerComponentBase : ComponentBase
    {

        [Inject] public BlazorColorPickerService BlazorColorPickerService { get; set; }

        [Parameter] public string ContextColor { get; set; }
        [Parameter] public BlazorColorPickerType ColorPickerType { get; set; }

        [Parameter]
        public Dictionary<string, dynamic> ColorSet { get; set; } = new Dictionary<string, dynamic>()
        {
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FAEBD7" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#F0F8FF" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFEBCD" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFE4C4" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#F8F8FF" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#DCDCDC" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#F0FFF0" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFF0F5" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFFACD" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#E0FFFF" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FAFAD2" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFB6C1" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#87CEFA" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFB6C1" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#E6E6FA" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FAF0E6" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFE4E1" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFEFD5" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFDAB9" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#B0E0E6" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#D8BFD8" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFF5EE" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#F5DEB3" } },
            { Guid.NewGuid().ToString(), new { itemchecked = false, itemcolor = "#FFFFE0" } },
        };

        protected override Task OnInitializedAsync()
        {
            if(ColorPickerType == BlazorColorPickerType.CustomColorPicker && !string.IsNullOrEmpty(this.ColorSet.FirstOrDefault().Value?.itemcolor))
            {
                this.ContextColor = this.ColorSet.FirstOrDefault().Value.itemcolor;
            }

            return base.OnInitializedAsync();
        }

        public void ContextColor_OnChangeAsync(string color)
        {
            this.ContextColor = color;
            this.BlazorColorPickerService.InvokeColorPickerEvent(color);
        }

        public void ColorSetItem_OnClicked(KeyValuePair<string, dynamic> clickedkvpair)
        {
            foreach(var checkedkvpair in this.ColorSet.Where(item => item.Value.itemchecked == true))
            {
                this.ColorSet[checkedkvpair.Key] = new { itemchecked = false, itemcolor = checkedkvpair.Value.itemcolor };
            }

            this.ColorSet[clickedkvpair.Key] = new { itemchecked = true, itemcolor = clickedkvpair.Value.itemcolor };
            this.ContextColor = clickedkvpair.Value.itemcolor;
            this.BlazorColorPickerService.InvokeColorPickerEvent(clickedkvpair.Value.itemcolor);
            this.StateHasChanged();
        }

        public bool ShowCustomColorPicker { get; set; }
        public void ToggleCustomColorPicker()
        {
            this.ShowCustomColorPicker = !this.ShowCustomColorPicker;
        }

    }
}
