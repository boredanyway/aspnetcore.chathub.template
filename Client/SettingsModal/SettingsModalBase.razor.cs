using Microsoft.AspNetCore.Components;
using Oqtane.ChatHubs.Services;
using Oqtane.Modules;
using BlazorModal;

namespace Oqtane.ChatHubs
{
    public class SettingsModalBase : ModuleBase
    {

        [Inject] public ChatHubService ChatHubService { get; set; }
        [Inject] public BlazorModalService BlazorModalService { get; set; }

        public const string SettingsModalElementId = "SettingsModalElementId";

        public async void OpenDialogAsync()
        {
            await this.BlazorModalService.ShowModal(SettingsModalElementId);
            StateHasChanged();
        }

        public async void CloseDialogAsync()
        {
            await this.BlazorModalService.HideModal(SettingsModalElementId);
            StateHasChanged();
        }

    }
}
