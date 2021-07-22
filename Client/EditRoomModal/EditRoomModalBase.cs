using BlazorAlerts;
using BlazorColorPicker;
using BlazorModal;
using BlazorSelect;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.ChatHubs.Services;
using Oqtane.ChatHubs.Enums;
using Oqtane.ChatHubs.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Oqtane.ChatHubs
{
    public class EditRoomModalBase : ModuleBase, IDisposable
    {

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }
        [Inject] public SiteState SiteState { get; set; }
        [Inject] public ChatHubService ChatHubService { get; set; }
        [Inject] public BlazorAlertsService BlazorAlertsService { get; set; }
        [Inject] public BlazorColorPickerService BlazorColorPickerService { get; set; }
        [Inject] public BlazorModalService BlazorModalService { get; set; }

        public const string EditRoomModalElementId = "EditRoomModalElementId";

        protected readonly string FileUploadDropzoneContainerElementId = "EditComponentFileUploadDropzoneContainer";
        protected readonly string FileUploadInputFileElementId = "EditComponentFileUploadInputFileContainer";

        public HashSet<string> SelectionItems { get; set; } = new HashSet<string>();
        public HashSet<string> BlazorColorPickerSelectionItems { get; set; } = new HashSet<string>();

        public BlazorColorPickerType BlazorColorPickerActiveType { get; set; }

        public int roomId = -1;
        public string title;
        public string content;
        public string backgroundcolor;
        public string type;
        public string imageUrl;
        public string createdby;
        public string modifiedby;
        public DateTime createdon;
        public DateTime modifiedon;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.BlazorColorPickerSelectionItems.Add(BlazorColorPickerType.HTML5ColorPicker.ToString());
                this.BlazorColorPickerSelectionItems.Add(BlazorColorPickerType.CustomColorPicker.ToString());
                this.BlazorColorPickerActiveType = BlazorColorPickerType.CustomColorPicker;

                this.SelectionItems.Add(ChatHubRoomType.Public.ToString());
                this.SelectionItems.Add(ChatHubRoomType.Protected.ToString());
                this.SelectionItems.Add(ChatHubRoomType.Private.ToString());

                this.type = ChatHubRoomType.Public.ToString();

                this.BlazorColorPickerService.OnBlazorColorPickerContextColorChangedEvent += OnBlazorColorPickerContextColorChangedExecute;
                this.ChatHubService.OnUpdateUI += (object sender, EventArgs e) => UpdateUIStateHasChanged();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Room {ChatHubRoomId} {Error}", roomId, ex.Message);
                ModuleInstance.AddModuleMessage("Error Loading Room", MessageType.Error);
            }
        }

        public void InitCreateRoom()
        {
            this.roomId = -1;
            this.title = string.Empty;
            this.content = string.Empty;
            this.backgroundcolor = string.Empty;
            this.type = ChatHubRoomType.Public.ToString();
            this.imageUrl = string.Empty;
            this.createdby = string.Empty;
            this.createdon = DateTime.Now;
            this.modifiedby = string.Empty;
            this.modifiedon = DateTime.Now;
        }
        public async Task CreateRoom()
        {
            ChatHubRoom room = new ChatHubRoom()
            {
                ModuleId = ModuleState.ModuleId,
                Title = this.title,
                Content = this.content,
                BackgroundColor = this.backgroundcolor,
                Type = this.type,
                Status = ChatHubRoomStatus.Enabled.ToString(),
                ImageUrl = string.Empty,
                OneVsOneId = string.Empty,
                CreatorId = ChatHubService.ConnectedUser.UserId,
            };

            room = await this.ChatHubService.CreateRoom(room);
            await this.CloseModal();
            this.BlazorAlertsService.NewBlazorAlert("Successfully created room.", "[Javascript Application]", PositionType.Fixed);
            StateHasChanged();
        }
        public async Task OpenCreateRoomModal()
        {
            this.InitCreateRoom();
            await this.BlazorModalService.ShowModal(EditRoomModalElementId);
            StateHasChanged();
        }

        public async Task InitEditRoom(int roomId)
        {
            try
            {
                this.roomId = roomId;
                ChatHubRoom room = await this.ChatHubService.GetRoom(roomId, ModuleState.ModuleId);
                if (room != null)
                {
                    this.title = room.Title;
                    this.content = room.Content;
                    this.backgroundcolor = room.BackgroundColor;
                    this.type = room.Type;
                    this.imageUrl = room.ImageUrl;
                    this.createdby = room.CreatedBy;
                    this.createdon = room.CreatedOn;
                    this.modifiedby = room.ModifiedBy;
                    this.modifiedon = room.ModifiedOn;
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Room {ChatHubRoomId} {Error}", roomId, ex.Message);
                ModuleInstance.AddModuleMessage("Error Loading Room", MessageType.Error);
            }
        }
        public async Task EditRoom()
        {
            try
            {
                ChatHubRoom room = await this.ChatHubService.GetRoom(roomId, ModuleState.ModuleId);
                if (room != null)
                {
                    room.Title = this.title;
                    room.Content = this.content;
                    room.BackgroundColor = this.backgroundcolor;
                    room.Type = this.type;

                    await this.ChatHubService.UpdateRoom(room);
                    await this.CloseModal();
                    this.BlazorAlertsService.NewBlazorAlert("Successfully edited room.", "[Javascript Application]", PositionType.Fixed);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Saving Room {ChatHubRoomId} {Error}", roomId, ex.Message);
                ModuleInstance.AddModuleMessage("Error Saving Room", MessageType.Error);
            }
        }
        public async Task OpenEditRoomModal(int roomId)
        {
            await this.InitEditRoom(roomId);
            await this.BlazorModalService.ShowModal(EditRoomModalElementId);
            StateHasChanged();
        }

        public async Task CloseModal()
        {
            await this.BlazorModalService.HideModal(EditRoomModalElementId);
        }

        public void OnSelect(BlazorSelectEvent e)
        {
            this.type = e.SelectedItem;
            this.UpdateUIStateHasChanged();
        }
        public void BlazorColorPicker_OnSelect(BlazorSelectEvent e)
        {
            this.BlazorColorPickerActiveType = (BlazorColorPickerType)Enum.Parse(typeof(BlazorColorPickerType), e.SelectedItem, true);
            this.UpdateUIStateHasChanged();
        }
        private void OnBlazorColorPickerContextColorChangedExecute(BlazorColorPickerEvent obj)
        {
            this.backgroundcolor = obj.ContextColor;
            this.UpdateUIStateHasChanged();
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
            this.BlazorColorPickerService.OnBlazorColorPickerContextColorChangedEvent -= OnBlazorColorPickerContextColorChangedExecute;
            this.ChatHubService.OnUpdateUI -= (object sender, EventArgs e) => UpdateUIStateHasChanged();
        }

    }
}
