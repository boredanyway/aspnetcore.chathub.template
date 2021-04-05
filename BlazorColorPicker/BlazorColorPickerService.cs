using System;
using System.Threading.Tasks;

namespace BlazorColorPicker
{
    public class BlazorColorPickerService
    {

        public event Action<BlazorColorPickerEvent> OnBlazorColorPickerContextColorChangedEvent;

        public void InvokeColorPickerEvent(string color)
        {
            this.OnBlazorColorPickerContextColorChangedEvent.Invoke(new BlazorColorPickerEvent() { ContextColor = color });
        }

    }
}
