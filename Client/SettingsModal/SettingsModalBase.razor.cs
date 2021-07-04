using Microsoft.AspNetCore.Components;
using Oqtane.ChatHubs.Shared.Models;
using Oqtane.ChatHubs.Services;
using Oqtane.Modules;

namespace Oqtane.ChatHubs
{
    public class SettingsModalBase : ModuleBase
    {

        [Inject] public ChatHubService ChatHubService { get; set; }

        public bool DialogIsOpen { get; set; }

        public void OpenDialog()
        {
            this.DialogIsOpen = true;
            StateHasChanged();
        }

        public void CloseDialog()
        {
            this.DialogIsOpen = false;
            StateHasChanged();
        }

    }
}
