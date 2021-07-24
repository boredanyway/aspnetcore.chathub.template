using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Oqtane.ChatHubs.Services;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BlazorAlerts;
using BlazorWindows;
using System.Net;
using BlazorDraggableList;
using BlazorFileUpload;
using BlazorBrowserResize;
using BlazorVideo;
using Oqtane.ChatHubs;
using Oqtane.ChatHubs.Models;
using Oqtane.ChatHubs.Enums;
using Oqtane.ChatHubs.Extensions;
using BlazorModal;
using Oqtane.Models;

namespace Oqtane.ChatHubs
{
    public class IndexBase : ModuleBase, IDisposable
    {
        
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected HttpClient HttpClient { get; set; }
        [Inject] protected SiteState SiteState { get; set; }
        [Inject] protected ISettingService SettingService { get; set; }
        [Inject] protected BlazorAlertsService BlazorAlertsService { get; set; }
        [Inject] protected ChatHubService ChatHubService { get; set; }
        [Inject] protected BlazorBrowserResizeService BrowserResizeService { get; set; }
        [Inject] protected ScrollService ScrollService { get; set; }
        [Inject] protected CookieService CookieService { get; set; }
        [Inject] protected BlazorDraggableListService BlazorDraggableListService { get; set; }
        [Inject] protected BlazorFileUploadService BlazorFileUploadService { get; set; }
        [Inject] protected BlazorVideoService BlazorVideoService { get; set; }
        [Inject] protected BlazorModalService BlazorModalService { get; set; }

        public int MessageWindowHeight { get; set; }
        public int UserlistWindowHeight { get; set; }
        
        public string GuestUsername { get; set; } = string.Empty;
        public ChatHubRoom contextRoom { get; set; }

        public int maxUserNameCharacters { get; set; }
        public int framerate { get; set; }
        public int videoBitsPerSecond { get; set; }
        public int audioBitsPerSecond { get; set; }
        public int videoSegmentsLength { get; set; }

        public int InnerHeight = 0;
        public int InnerWidth = 0;

        public static string ChatWindowDatePattern = @"HH:mm:ss";

        public Dictionary<string, string> settings { get; set; }

        public ImageModal ImageModalRef;
        public SettingsModal SettingsModalRef;
        public EditRoomModal EditRoomModalRef;

        protected readonly string DraggableLivestreamsContainerElementId = "DraggableLivestreamsContainer";
        protected readonly string FileUploadDropzoneContainerElementId = "FileUploadDropzoneContainer";
        protected readonly string FileUploadInputFileElementId = "FileUploadInputFileContainer";

        protected override async Task OnInitializedAsync()
        {
            this.ChatHubService.ModuleId = ModuleState.ModuleId;

            Dictionary<string, string> settings = await this.SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            this.maxUserNameCharacters = Int32.Parse(this.SettingService.GetSetting(settings, "MaxUserNameCharacters", "20"));
            this.framerate = Int32.Parse(this.SettingService.GetSetting(settings, "Framerate", "30"));
            this.videoBitsPerSecond = Int32.Parse(this.SettingService.GetSetting(settings, "VideoBitsPerSecond", "240000"));
            this.audioBitsPerSecond = Int32.Parse(this.SettingService.GetSetting(settings, "AudioBitsPerSecond", "100000"));
            this.videoSegmentsLength = Int32.Parse(this.SettingService.GetSetting(settings, "VideoSegmentsLength", "420"));

            this.BrowserResizeService.BrowserResizeServiceExtension.OnResize += BrowserHasResized;
            this.BlazorDraggableListService.BlazorDraggableListServiceExtension.OnDropEvent += OnDraggableListDropEventExecute;
            this.ChatHubService.OnUpdateUI += (object sender, EventArgs e) => UpdateUI();
                        
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (PageState.QueryString.ContainsKey("moduleid") && PageState.QueryString.ContainsKey("roomid") && int.Parse(PageState.QueryString["moduleid"]) == ModuleState.ModuleId)
                {
                    this.contextRoom = await this.ChatHubService.GetRoom(int.Parse(PageState.QueryString["roomid"]), ModuleState.ModuleId);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Rooms {Error}", ex.Message);
                ModuleInstance.AddModuleMessage("Error Loading Rooms", MessageType.Error);
            }

            await base.OnParametersSetAsync();
        }        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.ChatHubService.InitChatHubService();
                await this.CookieService.InitCookieService();
                await this.ScrollService.InitScrollService();
                await this.BrowserResizeService.InitBrowserResizeService();
                await this.BlazorModalService.InitBlazorModal();

                string hostname = new Uri(NavigationManager.BaseUri).Host;
                var cookievalue = await this.CookieService.GetCookieAsync(".AspNetCore.Identity.Application");
                this.ChatHubService.IdentityCookie = new Cookie(".AspNetCore.Identity.Application", cookievalue, "/", hostname);

                await this.ConnectToChat();
                await this.ChatHubService.chatHubMap.InvokeVoidAsync("showchathubscontainer");
                await this.ChatHubService.GetLobbyRooms();

                await this.BrowserResizeService.RegisterWindowResizeCallback();
                await BrowserHasResized();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnDraggableListDropEventExecute(object sender, BlazorDraggableListEvent e)
        {
            try
            {
                if (this.DraggableLivestreamsContainerElementId == e.DraggableContainerElementId)
                {
                    var items = this.ChatHubService.Rooms.Swap(e.DraggableItemOldIndex, e.DraggableItemNewIndex);
                    this.ChatHubService.Rooms = items.ToList<ChatHubRoom>();

                    List<ChatHubRoom> rooms = new List<ChatHubRoom>();
                    rooms.Add(this.ChatHubService.Rooms[e.DraggableItemOldIndex]);
                    rooms.Add(this.ChatHubService.Rooms[e.DraggableItemNewIndex]);

                    foreach(var room in rooms)
                    {
                        if (ChatHubService.ConnectedUser?.UserId == room.CreatorId)
                        {
                            await this.BlazorVideoService.RestartStreamTaskIfExists(room.Id.ToString(), ChatHubService.Connection.ConnectionId);
                        }
                        else
                        {
                            if (room.Creator != null)
                            {
                                foreach (var connection in room.Creator?.Connections)
                                {
                                    await this.BlazorVideoService.RestartStreamTaskIfExists(room.Id.ToString(), connection.ConnectionId);
                                }
                            }
                        }
                    }

                    this.UpdateUI();
                }
            }
            catch (Exception ex)
            {
                this.ChatHubService.HandleException(ex);
            }
        }

        public async Task EnableArchiveRoom(ChatHubRoom room)
        {
            try
            {
                if(room.Status == ChatHubRoomStatus.Archived.ToString())
                {
                    room.Status = ChatHubRoomStatus.Enabled.ToString();
                }
                else if(room.Status == ChatHubRoomStatus.Enabled.ToString())
                {
                    room.Status = ChatHubRoomStatus.Archived.ToString();
                }

                await ChatHubService.UpdateRoom(room);
                await logger.LogInformation("Room Archived {ChatHubRoom}", room);
                NavigationManager.NavigateTo(NavigateUrl());
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Archiving Room {ChatHubRoom} {Error}", room, ex.Message);
                ModuleInstance.AddModuleMessage("Error Archiving Room", MessageType.Error);
            }
        }

        public async Task DeleteRoom(int id)
        {
            try
            {
                await ChatHubService.DeleteRoom(id);
                await logger.LogInformation("Room Deleted {ChatHubRoomId}", id);
                NavigationManager.NavigateTo(NavigateUrl());
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Deleting Room {ChatHubRoomId} {Error}", id, ex.Message);
                ModuleInstance.AddModuleMessage("Error Deleting Room", MessageType.Error);
            }
        }

        public async Task ConnectToChat()
        {
            try
            {
                if (this.ChatHubService.Connection?.State == HubConnectionState.Connected
                 || this.ChatHubService.Connection?.State == HubConnectionState.Connecting
                 || this.ChatHubService.Connection?.State == HubConnectionState.Reconnecting)
                {
                    this.BlazorAlertsService.NewBlazorAlert($"The client is already connected. Trying establish new connection with guest name { this.GuestUsername }.", "Javascript Application", PositionType.Fixed);
                }

                this.ChatHubService.BuildHubConnection(GuestUsername, ModuleState.ModuleId);
                this.ChatHubService.RegisterHubConnectionHandlers();
                await this.ChatHubService.ConnectAsync();
                this.GuestUsername = string.Empty;
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Connecting To ChatHub: {Error}", ex.Message);
                ModuleInstance.AddModuleMessage("Error Connecting To ChatHub", MessageType.Error);
            }
        }
        public async Task EnterRoom_Clicked(int roomId, int moduleid)
        {
            if(!this.ChatHubService.Rooms.Any(item => item.Id == roomId) && ChatHubService.Connection?.State == HubConnectionState.Connected)
            {
                await this.ChatHubService.EnterChatRoom(roomId);
            }
        }
        public async Task LeaveRoom_Clicked(int roomId, int moduleId)
        {
            await this.ChatHubService.LeaveChatRoom(roomId);
        }

        public async Task FollowInvitation_Clicked(Guid invitationGuid, int roomId)
        {
            if (ChatHubService.Connection?.State == HubConnectionState.Connected)
            {
                await this.ChatHubService.EnterChatRoom(roomId);
                this.ChatHubService.Invitations.RemoveInvitation(invitationGuid);
            }
        }

        public void RemoveInvitation_Clicked(Guid guid)
        {
            this.ChatHubService.Invitations.RemoveInvitation(guid);
        }

        private int _cachedMsgInputCounter { get; set; } = 1;
        private List<string> _cachedMsgInputList { get; set; } = new List<string>();
        public async Task KeyDown(KeyboardEventArgs e, ChatHubRoom room)
        {
            if (!e.ShiftKey && e.Key == "Enter")
            {
                await this.SendMessage_Clicked(room.MessageInput, room);
            }
            else if (e.ShiftKey && e.Key == "Enter")
            {
                if (!string.IsNullOrEmpty(room.MessageInput))
                {
                    room.MessageInput.Trim();

                    if (this._cachedMsgInputCounter == 1)
                    {
                        this._cachedMsgInputList.Add(room.MessageInput);
                    }

                    if (this._cachedMsgInputList.Contains(room.MessageInput) == false)
                    {
                        this._cachedMsgInputList.Clear();
                        this._cachedMsgInputList.Add(room.MessageInput);
                        this._cachedMsgInputCounter = 1;
                    }

                    string newMessageInput = this.ChatHubService.AutocompleteUsername(this._cachedMsgInputList.FirstOrDefault(), room.Id, this._cachedMsgInputCounter, e.Key);
                    this._cachedMsgInputList.Add(newMessageInput);
                    room.MessageInput = newMessageInput;

                    if (this._cachedMsgInputList.Contains(room.MessageInput) == true)
                    {
                        this._cachedMsgInputCounter++;
                    }

                    this.UpdateUI();
                }
            }
        }

        public async Task SendMessage_Clicked(string messageInput, ChatHubRoom room)
        {
            await this.ChatHubService.SendMessage(messageInput, room.Id);
            room.MessageInput = string.Empty;
            this.StateHasChanged();
        }
        
        private async Task BrowserHasResized()
        {
            try
            {
                await InvokeAsync(async () =>
                {
                    this.InnerHeight = await this.BrowserResizeService.GetInnerHeight();
                    this.InnerWidth = await this.BrowserResizeService.GetInnerWidth();

                    this.MessageWindowHeight = 520;
                    this.UserlistWindowHeight = 570;

                    StateHasChanged();
                });
            }
            catch(Exception ex)
            {
                await logger.LogError(ex, "Error On Browser Resize {Error}", ex.Message);
                ModuleInstance.AddModuleMessage("Error On Browser Resize", MessageType.Error);
            }
        }

        public void UserlistItem_Clicked(MouseEventArgs e, ChatHubRoom room, ChatHubUser user)
        {
            InvokeAsync(() =>
            {
                if (user.UserlistItemCollapsed)
                {
                    user.UserlistItemCollapsed = false;
                }
                else
                {
                    foreach (var chatUser in room.Users.Where(x => x.UserlistItemCollapsed == true))
                    {
                        chatUser.UserlistItemCollapsed = false;
                    }
                    user.UserlistItemCollapsed = true;
                }

                StateHasChanged();
            });
        }

        public void OpenProfile_Clicked(int userId, int roomId)
        {
            this.SettingsModalRef.OpenDialogAsync();
        }

        public async Task FixCorruptConnections_ClickedAsync()
        {
            try
            {
                await this.ChatHubService.FixCorruptConnections(ModuleState.ModuleId);
            }
            catch
            {
                throw;
            }
        }

        public string ReplaceYoutubeLinksAsync(string message)
        {
            try
            {
                //var youtubeRegex = @"(?:http?s?:\/\/)?(?:www.)?(?:m.)?(?:music.)?youtu(?:\.?be)(?:\.com)?(?:(?:\w*.?:\/\/)?\w*.?\w*-?.?\w*\/(?:embed|e|v|watch|.*\/)?\??(?:feature=\w*\.?\w*)?&?(?:v=)?\/?)([\w\d_-]{11})(?:\S+)?";
                List<string> regularExpressions = this.SettingService.GetSetting(this.settings, "RegularExpression", "").Split(";delimiter;", StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var regularExpression in regularExpressions)
                {
                    string pattern = regularExpression;
                    string replacement = string.Format("<a href=\"{0}\" target=\"_blank\" title=\"{0}\">{0}</a>", "$0");
                    message = Regex.Replace(message, pattern, replacement);
                }
            }
            catch (Exception ex)
            {
                ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error);
            }

            return message;
        }

        public string HighlightOwnUsername(string message, string username)
        {
            try
            {
                string pattern = username;
                string replacement = string.Format("<strong>{0}</strong>", "$0");
                message = Regex.Replace(message, pattern, replacement);
            }
            catch (Exception ex)
            {
                ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error);
            }

            return message;
        }

        private void UpdateUI()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public void ShowWindow(WindowEvent e)
        {
            this.ChatHubService.ContextRoomId = e.ActivatedItem.Id.ToString();
            var room = this.ChatHubService.Rooms.FirstOrDefault(item => item.Id.ToString() == this.ChatHubService.ContextRoomId);
            if (room != null)
            {
                room.UnreadMessages = 0;
            }
        }
        public void HideWindow(WindowEvent e)
        {
            var room = this.ChatHubService.Rooms.FirstOrDefault(item => item.Id == e.DeactivatedItem.Id);
            if (room != null)
            {
                //this.ChatHubService.StopVideoChat(e.DeactivatedItem.Id);
            }
        }
        public void ShownWindow(WindowEvent e)
        {
        }
        public void HiddenWindow(WindowEvent e)
        {
        }
        public void AddedWindow(WindowEvent e)
        {

        }
        public async void RemovedWindow(WindowEvent e)
        {
            foreach (var room in this.ChatHubService.Rooms)
            {
                if (ChatHubService.ConnectedUser?.UserId == room.CreatorId)
                {
                    await this.BlazorVideoService.RestartStreamTaskIfExists(room.Id.ToString(), ChatHubService.Connection.ConnectionId);
                }
                else
                {
                    if (room.Creator != null)
                    {
                        foreach (var connection in room.Creator?.Connections)
                        {
                            await this.BlazorVideoService.RestartStreamTaskIfExists(room.Id.ToString(), connection.ConnectionId);
                        }
                    }
                }
            }
        }

        public override List<Resource> Resources => new List<Resource>()
        {
            new Resource { ResourceType = ResourceType.Script, Bundle = "jQuery", Url = "https://code.jquery.com/jquery-3.2.1.slim.min.js", Integrity = "sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN", CrossOrigin = "anonymous", Location = ResourceLocation.Body, Declaration = ResourceDeclaration.Local },
            new Resource { ResourceType = ResourceType.Script, Bundle = "IoButtons", Url = "https://buttons.github.io/buttons.js", CrossOrigin = "anonymous", Location = ResourceLocation.Body, Declaration = ResourceDeclaration.Local },
        };

        public void Dispose()
        {
            this.BlazorDraggableListService.BlazorDraggableListServiceExtension.OnDropEvent -= OnDraggableListDropEventExecute;
            this.BrowserResizeService.BrowserResizeServiceExtension.OnResize -= BrowserHasResized;
            this.ChatHubService.OnUpdateUI -= (object sender, EventArgs e) => UpdateUI();

            if(ChatHubService.Connection != null)
            {
                this.ChatHubService.Connection.StopAsync();
                this.ChatHubService.Connection.DisposeAsync();
            }            
        }

    }

    public static class IndexBaseExtensionMethods
    {
        public static IList<ChatHubRoom> Swap<TItemGeneric>(this IList<ChatHubRoom> list, int x, int y)
        {
            ChatHubRoom temp = list[x];
            list[x] = list[y];
            list[y] = temp;
            return list;
        }
    }

}