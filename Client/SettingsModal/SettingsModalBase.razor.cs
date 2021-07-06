using Microsoft.AspNetCore.Components;
using Oqtane.ChatHubs.Services;
using Oqtane.Modules;
using BlazorModal;
using System.Threading.Tasks;

namespace Oqtane.ChatHubs
{
    public class SettingsModalBase : ModuleBase
    {

        [Inject] public ChatHubService ChatHubService { get; set; }
        [Inject] public BlazorModalService BlazorModalService { get; set; }

        public const string SettingsModalElementId = "SettingsModalElementId";

        public bool DialogIsOpen { get; set; }

        public async void OpenDialogAsync()
        {
            this.DialogIsOpen = true;
            await this.BlazorModalService.ShowModal(SettingsModalElementId);
            StateHasChanged();
        }

        public async void CloseDialogAsync()
        {
            this.DialogIsOpen = false;
            await this.BlazorModalService.HideModal(SettingsModalElementId);
            StateHasChanged();
        }

    }
}
