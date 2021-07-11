using Microsoft.AspNetCore.Components;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Oqtane.Services;
using System.Linq;
using System.Timers;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Modules;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using BlazorAlerts;
using System.Net;
using BlazorDraggableList;
using BlazorVideo;
using BlazorBrowserResize;
using System.Net.Http.Json;
using Oqtane.ChatHubs.Models;
using Oqtane.ChatHubs.Extensions;
using Oqtane.ChatHubs.Enums;

namespace Oqtane.ChatHubs.Services
{

    public class ChatHubService : ServiceBase, IService, IDisposable
    {

        public HttpClient HttpClient { get; set; }
        public NavigationManager NavigationManager { get; set; }
        public SiteState SiteState { get; set; }
        public IJSRuntime JSRuntime { get; set; }
        public ScrollService ScrollService { get; set; }
        public BlazorAlertsService BlazorAlertsService { get; set; }
        public BlazorDraggableListService BlazorDraggableListService { get; set; }
        public BlazorBrowserResizeService BrowserResizeService { get; set; }
        public BlazorVideoService BlazorVideoService { get; set; }

        public int ModuleId { get; set; }
        public HubConnection Connection { get; set; }
        public ChatHubUser ConnectedUser { get; set; }
        
        public Cookie IdentityCookie { get; set; }
        public string ContextRoomId { get; set; }
        

        public List<ChatHubRoom> Lobbies { get; set; } = new List<ChatHubRoom>();
        public List<ChatHubRoom> Rooms { get; set; } = new List<ChatHubRoom>();
        public List<ChatHubInvitation> Invitations { get; set; } = new List<ChatHubInvitation>();
        public List<ChatHubUser> IgnoredUsers { get; set; } = new List<ChatHubUser>();
        public List<ChatHubUser> IgnoredByUsers { get; set; } = new List<ChatHubUser>();

        public Timer GetLobbyRoomsTimer { get; set; } = new Timer();

        public event EventHandler OnUpdateUI;
        public event EventHandler<ChatHubUser> OnUpdateConnectedUserEvent;
        public event EventHandler<ChatHubRoom> OnAddChatHubRoomEvent;
        public event EventHandler<ChatHubRoom> OnRemoveChatHubRoomEvent;
        public event Action<ChatHubUser, string> OnAddChatHubUserEvent;
        public event Action<ChatHubUser, string> OnRemoveChatHubUserEvent;
        public event EventHandler<ChatHubMessage> OnAddChatHubMessageEvent;
        public event EventHandler<ChatHubInvitation> OnAddChatHubInvitationEvent;
        public event EventHandler<ChatHubInvitation> OnRemoveChatHubInvitationEvent;
        public event EventHandler<ChatHubWaitingRoomItem> OnAddChatHubWaitingRoomItemEvent;
        public event EventHandler<ChatHubWaitingRoomItem> OnRemovedChatHubWaitingRoomItemEvent;
        public event EventHandler<ChatHubUser> OnAddIgnoredUserEvent;
        public event EventHandler<ChatHubUser> OnRemoveIgnoredUserEvent;
        public event EventHandler<ChatHubUser> OnAddIgnoredByUserEvent;
        public event EventHandler<ChatHubUser> OnRemoveIgnoredByUserEvent;
        public event Action<ChatHubModerator, int> OnAddModeratorEvent;
        public event Action<ChatHubModerator, int> OnRemoveModeratorEvent;
        public event Action<ChatHubWhitelistUser, int> OnAddWhitelistUserEvent;
        public event Action<ChatHubWhitelistUser, int> OnRemoveWhitelistUserEvent;
        public event Action<ChatHubBlacklistUser, int> OnAddBlacklistUserEvent;
        public event Action<ChatHubBlacklistUser, int> OnRemoveBlacklistUserEvent;
        public event Action<ChatHubCam, int> OnAddChatHubCamEvent;
        public event Action<ChatHubCam, int> OnRemoveChatHubCamEvent;
        public event Action<string, string, string, ChatHubUser> OnDownloadBytesEvent;
        public event EventHandler<int> OnClearHistoryEvent;
        public event EventHandler<ChatHubUser> OnDisconnectEvent;

        public ChatHubService(HttpClient httpClient, SiteState siteState, NavigationManager navigationManager, IJSRuntime JSRuntime, ScrollService scrollService, BlazorAlertsService blazorAlertsService, BlazorDraggableListService blazorDraggableListService, BlazorBrowserResizeService browserResizeService, BlazorVideoService blazorVideoService ) : base (httpClient)
        {
            this.HttpClient = httpClient;
            this.SiteState = siteState;
            this.NavigationManager = navigationManager;
            this.JSRuntime = JSRuntime;
            this.ScrollService = scrollService;
            this.BlazorAlertsService = blazorAlertsService;
            this.BlazorDraggableListService = blazorDraggableListService;
            this.BrowserResizeService = browserResizeService;
            this.BlazorVideoService = blazorVideoService;

            this.BlazorVideoService.StartVideoEvent += async (BlazorVideoModel model) => await this.StartCam(model);
            this.BlazorVideoService.StopVideoEvent += async (BlazorVideoModel model) => await this.StopCam(model);
            this.BlazorVideoService.BlazorVideoServiceExtension.OnDataAvailableEventHandler += async (string data, string id) => await OnDataAvailableEventHandlerExecute(data, id);

            this.BlazorAlertsService.OnAlertConfirmed += OnAlertConfirmedExecute;

            this.OnUpdateConnectedUserEvent += OnUpdateConnectedUserExecute;
            this.OnAddChatHubRoomEvent += OnAddChatHubRoomExecute;
            this.OnRemoveChatHubRoomEvent += OnRemoveChatHubRoomExecute;
            this.OnAddChatHubUserEvent += OnAddChatHubUserExecute;
            this.OnRemoveChatHubUserEvent += OnRemoveChatHubUserExecute;
            this.OnAddChatHubMessageEvent += OnAddChatHubMessageExecute;
            this.OnAddChatHubInvitationEvent += OnAddChatHubInvitationExecute;
            this.OnRemoveChatHubInvitationEvent += OnRemoveChatHubInvitationExecute;
            this.OnAddChatHubWaitingRoomItemEvent += OnAddChatHubWaitingRoomItemExecute;
            this.OnRemovedChatHubWaitingRoomItemEvent += OnRemovedChathubWaitingRoomItemExecute;
            this.OnAddIgnoredUserEvent += OnAddIngoredUserExexute;
            this.OnRemoveIgnoredUserEvent += OnRemoveIgnoredUserExecute;
            this.OnAddIgnoredByUserEvent += OnAddIgnoredByUserExecute;
            this.OnAddModeratorEvent += OnAddModeratorExecute;
            this.OnRemoveModeratorEvent += OnRemoveModeratorExecute;
            this.OnAddWhitelistUserEvent += OnAddWhitelistUserExecute;
            this.OnRemoveWhitelistUserEvent += OnRemoveWhitelistUserExecute;
            this.OnAddBlacklistUserEvent += OnAddBlacklistUserExecute;
            this.OnRemoveBlacklistUserEvent += OnRemoveBlacklistUserExecute;
            this.OnAddChatHubCamEvent += OnAddChatHubCamExecute;
            this.OnRemoveChatHubCamEvent += OnRemoveChatHubCamExecute;
            this.OnDownloadBytesEvent += async (string dataURI, string id, string connectionId, ChatHubUser creator) => await OnDownloadBytesExecuteAsync(dataURI, id, connectionId, creator);
            this.OnRemoveIgnoredByUserEvent += OnRemoveIgnoredByUserExecute;
            this.OnClearHistoryEvent += OnClearHistoryExecute;
            this.OnDisconnectEvent += OnDisconnectExecute;

            GetLobbyRoomsTimer.Elapsed += new ElapsedEventHandler(OnGetLobbyRoomsTimerElapsed);
            GetLobbyRoomsTimer.Interval = 10000;
            GetLobbyRoomsTimer.Enabled = true;
        }

        public void BuildGuestConnection(string username, int moduleId)
        {
            StringBuilder urlBuilder = new StringBuilder();
            var chatHubConnection = this.NavigationManager.BaseUri + "chathub";

            urlBuilder.Append(chatHubConnection);
            urlBuilder.Append("?guestname=" + username);

            var url = urlBuilder.ToString();
            Connection = new HubConnectionBuilder().WithUrl(url, options =>
            {
                options.Cookies.Add(this.IdentityCookie);
                options.Headers.Add("moduleid", moduleId.ToString());
                options.Headers.Add("platform", "Oqtane");
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            })
            .AddNewtonsoftJsonProtocol(options => {
                options.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .Build();
        }
        public void RegisterHubConnectionHandlers()
        {
            Connection.Reconnecting += (ex) =>
            {
                if (ex != null)
                {
                    this.HandleException(new Exception(ex.Message, ex));
                }

                return Task.CompletedTask;
            };
            Connection.Reconnected += (msg) =>
            {
                if (msg != null)
                {
                    this.HandleException(new Exception(msg));
                }

                return Task.CompletedTask;
            };
            Connection.Closed += (ex) =>
            {
                if (ex != null)
                {
                    this.HandleException(new Exception(ex.Message, ex));
                }

                this.Rooms.Clear();
                this.RunUpdateUI();
                return Task.CompletedTask;
            };

            this.Connection.On("OnUpdateConnectedUser", (ChatHubUser user) => OnUpdateConnectedUserEvent(this, user));
            this.Connection.On("AddRoom", (ChatHubRoom room) => OnAddChatHubRoomEvent(this, room));
            this.Connection.On("RemoveRoom", (ChatHubRoom room) => OnRemoveChatHubRoomEvent(this, room));
            this.Connection.On("AddUser", (ChatHubUser user, string roomId) => OnAddChatHubUserEvent(user, roomId));
            this.Connection.On("RemoveUser", (ChatHubUser user, string roomId) => OnRemoveChatHubUserEvent(user, roomId));
            this.Connection.On("AddMessage", (ChatHubMessage message) => OnAddChatHubMessageEvent(this, message));
            this.Connection.On("AddInvitation", (ChatHubInvitation invitation) => OnAddChatHubInvitationEvent(this, invitation));
            this.Connection.On("RemoveInvitation", (ChatHubInvitation invitation) => OnRemoveChatHubInvitationEvent(this, invitation));
            this.Connection.On("AddWaitingRoomItem", (ChatHubWaitingRoomItem waitingRoomItem) => OnAddChatHubWaitingRoomItemEvent(this, waitingRoomItem));
            this.Connection.On("RemovedWaitingRoomItem", (ChatHubWaitingRoomItem waitingRoomItem) => OnRemovedChatHubWaitingRoomItemEvent(this, waitingRoomItem));
            this.Connection.On("AddIgnoredUser", (ChatHubUser ignoredUser) => OnAddIgnoredUserEvent(this, ignoredUser));
            this.Connection.On("RemoveIgnoredUser", (ChatHubUser ignoredUser) => OnRemoveIgnoredUserEvent(this, ignoredUser));
            this.Connection.On("AddIgnoredByUser", (ChatHubUser ignoredUser) => OnAddIgnoredByUserEvent(this, ignoredUser));
            this.Connection.On("RemoveIgnoredByUser", (ChatHubUser ignoredUser) => OnRemoveIgnoredByUserEvent(this, ignoredUser));
            this.Connection.On("DownloadBytes", (string dataURI, string id, string connectionId, ChatHubUser creator) => OnDownloadBytesEvent(dataURI, id, connectionId, creator));
            this.Connection.On("AddModerator", (ChatHubModerator moderator, int roomId) => OnAddModeratorEvent(moderator, roomId));
            this.Connection.On("RemoveModerator", (ChatHubModerator moderator, int roomId) => OnRemoveModeratorEvent(moderator, roomId));
            this.Connection.On("AddWhitelistUser", (ChatHubWhitelistUser whitelistUser, int roomId) => OnAddWhitelistUserEvent(whitelistUser,roomId));
            this.Connection.On("RemoveWhitelistUser", (ChatHubWhitelistUser whitelistUser, int roomId) => OnRemoveWhitelistUserEvent(whitelistUser, roomId));
            this.Connection.On("AddBlacklistUser", (ChatHubBlacklistUser blacklistUser, int roomId) => OnAddBlacklistUserEvent(blacklistUser, roomId));
            this.Connection.On("RemoveBlacklistUser", (ChatHubBlacklistUser blacklistUser, int roomId) => OnRemoveBlacklistUserEvent(blacklistUser, roomId));
            this.Connection.On("AddCam", (ChatHubCam cam, int roomId) => OnAddChatHubCamEvent(cam, roomId));
            this.Connection.On("RemoveCam", (ChatHubCam cam, int roomId) => OnRemoveChatHubCamEvent(cam, roomId));
            this.Connection.On("ClearHistory", (int roomId) => OnClearHistoryEvent(this, roomId));
            this.Connection.On("Disconnect", (ChatHubUser user) => OnDisconnectEvent(this, user));
        }
        public async Task ConnectAsync()
        {
            await this.Connection.StartAsync().ContinueWith(async task =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);

                    await this.Connection.SendAsync("Init").ContinueWith((task) =>
                    {
                        if (task.IsCompleted)
                        {
                            this.HandleException(task);
                        }
                    });
                }
            });
        }
        
        public async Task OnDataAvailableEventHandlerExecute(string dataUri, string roomId)
        {
            try
            {
                if (this.Connection?.State == HubConnectionState.Connected)
                {
                    await this.Connection.SendAsync("UploadDataUri", dataUri, roomId).ContinueWith((task) =>
                    {
                        if (task.IsCompleted)
                        {
                            this.HandleException(task);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }        
        public async Task OnDownloadBytesExecuteAsync(string dataURI, string id, string connectionId, ChatHubUser creator)
        {
            try
            {
                var room = this.Rooms.FirstOrDefault(item => item.Id.ToString() == id);
                if(room != null)
                {
                    room.Creator = creator;
                    this.RunUpdateUI();
                }

                await this.BlazorVideoService.AppendBufferRemoteLivestream(dataURI, id, connectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<ChatHubRoom> CreateRoom(ChatHubRoom room)
        {
            ChatHubRoom createdRoom = null;
            await this.Connection.InvokeAsync<ChatHubRoom>("CreateRoom", room).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                    createdRoom = task.Result;
                }
            });

            return createdRoom;
        }
        public async Task<ChatHubRoom> UpdateRoom(ChatHubRoom room)
        {
            ChatHubRoom updatedRoom = null;
            await this.Connection.InvokeAsync<ChatHubRoom>("UpdateRoom", room).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                    updatedRoom = task.Result;
                }
            });

            return updatedRoom;
        }
        public async Task DeleteRoom(int roomId)
        {
            await this.Connection.InvokeAsync("DeleteRoom", roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public async Task EnterChatRoom(int roomId)
        {
            await this.Connection.InvokeAsync("EnterChatRoom", roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public async Task LeaveChatRoom(int roomId)
        {
            await this.Connection.InvokeAsync("LeaveChatRoom", roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public async Task StartCam(BlazorVideoModel model)
        {
            await this.Connection.InvokeAsync("StartCam", Convert.ToInt32(model.Id)).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });            
        }
        public async Task StopCam(BlazorVideoModel model)
        {
            await this.Connection.InvokeAsync("StopCam", Convert.ToInt32(model.Id)).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public async Task GetLobbyRooms(int moduleId)
        {
            try
            {
                this.Lobbies = await this.GetChatHubRoomsByModuleIdAsync(moduleId);
                if(this.Lobbies != null && this.Lobbies.Any())
                {
                    this.SortLobbyRooms();
                    this.RunUpdateUI();
                }
            }
            catch (Exception ex)
            {
                //this.HandleException(ex);
            }
        }
        public async Task GetIgnoredUsers()
        {
            await this.Connection.InvokeAsync<List<ChatHubUser>>("GetIgnoredUsers").ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);

                    var ignoredUsers = task.Result;
                    if (ignoredUsers != null)
                    {
                        foreach (var user in ignoredUsers)
                        {
                            this.IgnoredUsers.AddIgnoredUser(user);
                        }
                    }
                }
            });
        }
        public async Task GetIgnoredByUsers()
        {
            await this.Connection.InvokeAsync<List<ChatHubUser>>("GetIgnoredByUsers").ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);

                    var ignoredByUsers = task.Result;
                    if (ignoredByUsers != null)
                    {
                        foreach (var user in ignoredByUsers)
                        {
                            this.IgnoredByUsers.AddIgnoredByUser(user);
                        }
                    }
                }
            });
        }

        public void SortLobbyRooms()
        {
            if (this.Lobbies != null && this.Lobbies.Any())
            {
                this.Lobbies = this.Lobbies.OrderByDescending(item => item.Users?.Count()).ThenByDescending(item => item.CreatedOn).OrderBy(item => (int)Enum.Parse(typeof(ChatHubRoomStatus), item.Status)).Take(1000).ToList();
            }
        }

        public async Task SendMessage(string content, int roomId)
        {
            await this.Connection.InvokeAsync("SendMessage", content, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void IgnoreUser_Clicked(int userId, int roomId, string username)
        {
            this.Connection.InvokeAsync("IgnoreUser", username).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void UnignoreUser_Clicked(string username)
        {
            this.Connection.InvokeAsync("UnignoreUser", username).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void AddModerator_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("AddModerator", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void RemoveModerator_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("RemoveModerator", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void AddWhitelistUser_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("AddWhitelistUser", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void RemoveWhitelistUser_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("RemoveWhitelistUser", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void AddBlacklistUser_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("AddBlacklistUser", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void RemoveBlacklistUser_Clicked(int userId, int roomId)
        {
            this.Connection.InvokeAsync("RemoveBlacklistUser", userId, roomId).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });
        }
        public void RemoveWaitingRoomItem_Clicked(ChatHubWaitingRoomItem waitingRoomItem)
        {
            this.AddWhitelistUser_Clicked(waitingRoomItem.UserId, waitingRoomItem.RoomId);

            this.Connection.InvokeAsync("RemoveWaitingRoomItem", waitingRoomItem).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    this.HandleException(task);
                }
            });

            this.Rooms.FirstOrDefault(item => item.Id == waitingRoomItem.RoomId).WaitingRoomItems.Remove(waitingRoomItem);
            this.RunUpdateUI();
        }
        public async Task DisconnectAsync()
        {
            if (Connection.State != HubConnectionState.Disconnected)
            {
                await Connection.StopAsync();
            }
        }

        private void OnAddChatHubRoomExecute(object sender, ChatHubRoom room)
        {
            this.Rooms.AddRoom(room);
            this.RunUpdateUI();
        }
        private void OnRemoveChatHubRoomExecute(object sender, ChatHubRoom room)
        {
            this.Rooms.RemoveRoom(room);
            this.RunUpdateUI();
        }
        private void OnAddChatHubUserExecute(ChatHubUser user, string roomId)
        {
            this.Rooms.AddUser(user, roomId);
            this.RunUpdateUI();
        }
        private void OnRemoveChatHubUserExecute(ChatHubUser user, string roomId)
        {
            this.Rooms.RemoveUser(user, roomId);
            this.RunUpdateUI();
        }
        public async void OnAddChatHubMessageExecute(object sender, ChatHubMessage message)
        {
            ChatHubRoom room = this.Rooms.FirstOrDefault(item => item.Id == message.ChatHubRoomId);
            room.AddMessage(message);

            if (message.ChatHubRoomId.ToString() != this.ContextRoomId)
            {
                this.Rooms.FirstOrDefault(room => room.Id == message.ChatHubRoomId).UnreadMessages++;
            }

            this.RunUpdateUI();

            string elementId = string.Concat("#message-window-", this.ModuleId.ToString(), "-", message.ChatHubRoomId.ToString());
            await this.ScrollService.ScrollToBottom(elementId);
        }
        private void OnAddChatHubInvitationExecute(object sender, ChatHubInvitation item)
        {
            this.Invitations.AddInvitation(item);
        }
        private void OnRemoveChatHubInvitationExecute(object sender, ChatHubInvitation item)
        {
            this.Invitations.RemoveInvitation(item.Guid);
        }
        private void OnAddChatHubWaitingRoomItemExecute(object sender, ChatHubWaitingRoomItem e)
        {
            this.Rooms.FirstOrDefault(item => item.Id == e.RoomId).WaitingRoomItems.Add(e);
        }
        private async void OnRemovedChathubWaitingRoomItemExecute(object sender, ChatHubWaitingRoomItem e)
        {
            var room = await this.GetChatHubRoomAsync(e.RoomId, this.ModuleId);
            this.BlazorAlertsService.NewBlazorAlert($"You have been granted access to the room named {room.Title}. Do you like to enter??", "Javascript Application", PositionType.Fixed, true, e.RoomId.ToString());
        }
        private void OnAddIngoredUserExexute(object sender, ChatHubUser user)
        {
            this.IgnoredUsers.AddIgnoredUser(user);
            this.RunUpdateUI();
        }
        private void OnRemoveIgnoredUserExecute(object sender, ChatHubUser user)
        {
            this.IgnoredUsers.RemoveIgnoredUser(user);
            this.RunUpdateUI();
        }
        private void OnAddIgnoredByUserExecute(object sender, ChatHubUser user)
        {
            this.IgnoredByUsers.AddIgnoredByUser(user);
            this.RunUpdateUI();
        }
        private void OnRemoveIgnoredByUserExecute(object sender, ChatHubUser user)
        {
            this.IgnoredByUsers.RemoveIgnoredByUser(user);
            this.RunUpdateUI();
        }
        private void OnAddModeratorExecute(ChatHubModerator moderator, int roomId)
        {
            this.Rooms.AddModerator(moderator, roomId);
            this.RunUpdateUI();
        }
        private void OnRemoveModeratorExecute(ChatHubModerator moderator, int roomId)
        {
            this.Rooms.RemoveModerator(moderator, roomId);
            this.RunUpdateUI();
        }
        private void OnAddWhitelistUserExecute(ChatHubWhitelistUser whitelistUser, int roomId)
        {
            this.Rooms.AddWhitelistUser(whitelistUser, roomId);
            this.RunUpdateUI();
        }
        private void OnRemoveWhitelistUserExecute(ChatHubWhitelistUser whitelistUser, int roomId)
        {
            this.Rooms.RemoveWhitelistUser(whitelistUser, roomId);
            this.RunUpdateUI();
        }
        private void OnAddBlacklistUserExecute(ChatHubBlacklistUser blacklistUser, int roomId)
        {
            this.Rooms.AddBlacklistUser(blacklistUser, roomId);
            this.RunUpdateUI();
        }
        private void OnRemoveBlacklistUserExecute(ChatHubBlacklistUser blacklistUser, int roomId)
        {
            this.Rooms.RemoveBlacklistUser(blacklistUser, roomId);
            this.RunUpdateUI();
        }
        private void OnAddChatHubCamExecute(ChatHubCam cam, int roomId)
        {
            this.Rooms.AddCam(cam, roomId);
            this.RunUpdateUI();
        }
        private void OnRemoveChatHubCamExecute(ChatHubCam cam, int roomId)
        {
            this.Rooms.RemoveCam(cam, roomId);
            this.RunUpdateUI();
        }
        private void OnClearHistoryExecute(object sender, int roomId)
        {
            this.ClearHistory(roomId);
        }
        public void OnUpdateConnectedUserExecute(object sender, ChatHubUser user)
        {
            this.ConnectedUser = user;
            this.RunUpdateUI();
        }
        private async void OnDisconnectExecute(object sender, ChatHubUser user)
        {
            await this.DisconnectAsync();
        }

        private async void OnGetLobbyRoomsTimerElapsed(object source, ElapsedEventArgs e)
        {
            await this.GetLobbyRooms(this.ModuleId);
        }

        public async void OnAlertConfirmedExecute(object sender, dynamic obj)
        {
            bool confirmed = (bool)obj.confirmed;
            BlazorAlertsModel model = (BlazorAlertsModel)obj.model;

            if (confirmed)
            {
                await this.EnterChatRoom(Convert.ToInt32(model.Id));
            }
        }

        public void ClearHistory(int roomId)
        {
            var room = this.Rooms.FirstOrDefault(x => x.Id == roomId);
            room.Messages.Clear();
            this.RunUpdateUI();
        }
        public void ToggleUserlist(ChatHubRoom room)
        {
            room.ShowUserlist = !room.ShowUserlist;
        }
        public string AutocompleteUsername(string msgInput, int roomId, int autocompleteCounter, string pressedKey)
        {
            List<string> words = msgInput.Trim().Split(' ').ToList();
            string lastWord = words.Last();

            var room = this.Rooms.FirstOrDefault(item => item.Id == roomId);
            var users = room.Users.Where(x => x.DisplayName.StartsWith(lastWord));

            if (users.Any())
            {
                autocompleteCounter = autocompleteCounter % users.Count();

                words.Remove(lastWord);
                if (pressedKey == "Enter")
                    words.Add(users.ToArray()[autocompleteCounter].DisplayName);

                msgInput = string.Join(' ', words);
            }

            return msgInput;
        }

        public void HandleException(Task task)
        {
            if (task.Exception != null)
            {
                this.HandleException(task.Exception);
            }
        }
        public void HandleException(Exception exception)
        {
            string message = string.Empty;
            if (exception.InnerException != null && exception.InnerException is HubException)
            {
                int startIndex = exception.Message.IndexOf("HubException:");
                int endIndex = exception.Message.Length - startIndex - 1;

                message = exception.Message.Substring(startIndex, endIndex);
            }
            else
            {
                message = exception.ToString();
            }

            BlazorAlertsService.NewBlazorAlert(message, "Javascript Application", PositionType.Fixed);
            this.RunUpdateUI();
        }

        public void Dispose()
        {
            this.BlazorVideoService.StartVideoEvent -= async (BlazorVideoModel model) => await this.StartCam(model);
            this.BlazorVideoService.StopVideoEvent -= async (BlazorVideoModel model) => await this.StopCam(model);
            this.BlazorVideoService.BlazorVideoServiceExtension.OnDataAvailableEventHandler -= async (string data, string id) => await OnDataAvailableEventHandlerExecute(data, id);

            this.BlazorAlertsService.OnAlertConfirmed -= OnAlertConfirmedExecute;

            this.OnUpdateConnectedUserEvent -= OnUpdateConnectedUserExecute;
            this.OnAddChatHubRoomEvent -= OnAddChatHubRoomExecute;
            this.OnRemoveChatHubRoomEvent -= OnRemoveChatHubRoomExecute;
            this.OnAddChatHubUserEvent -= OnAddChatHubUserExecute;
            this.OnRemoveChatHubUserEvent -= OnRemoveChatHubUserExecute;
            this.OnAddChatHubMessageEvent -= OnAddChatHubMessageExecute;
            this.OnAddChatHubInvitationEvent -= OnAddChatHubInvitationExecute;
            this.OnRemoveChatHubInvitationEvent -= OnRemoveChatHubInvitationExecute;
            this.OnAddChatHubWaitingRoomItemEvent -= OnAddChatHubWaitingRoomItemExecute;
            this.OnRemovedChatHubWaitingRoomItemEvent -= OnRemovedChathubWaitingRoomItemExecute;
            this.OnAddIgnoredUserEvent -= OnAddIngoredUserExexute;
            this.OnRemoveIgnoredUserEvent -= OnRemoveIgnoredUserExecute;
            this.OnAddIgnoredByUserEvent -= OnAddIgnoredByUserExecute;
            this.OnAddModeratorEvent -= OnAddModeratorExecute;
            this.OnRemoveModeratorEvent -= OnRemoveModeratorExecute;
            this.OnAddWhitelistUserEvent -= OnAddWhitelistUserExecute;
            this.OnRemoveWhitelistUserEvent -= OnRemoveWhitelistUserExecute;
            this.OnAddBlacklistUserEvent -= OnAddBlacklistUserExecute;
            this.OnRemoveBlacklistUserEvent -= OnRemoveBlacklistUserExecute;
            this.OnAddChatHubCamEvent += OnAddChatHubCamExecute;
            this.OnRemoveChatHubCamEvent += OnRemoveChatHubCamExecute;
            this.OnDownloadBytesEvent -= async (string dataURI, string id, string connectionId, ChatHubUser creator) => await OnDownloadBytesExecuteAsync(dataURI, id, connectionId, creator);
            this.OnRemoveIgnoredByUserEvent -= OnRemoveIgnoredByUserExecute;
            this.OnClearHistoryEvent -= OnClearHistoryExecute;
            this.OnDisconnectEvent -= OnDisconnectExecute;

            GetLobbyRoomsTimer.Elapsed -= new ElapsedEventHandler(OnGetLobbyRoomsTimerElapsed);            
        }

        public void RunUpdateUI()
        {
            this.OnUpdateUI.Invoke(this, EventArgs.Empty);
        }

        public string apiurl
        {
            //get { return NavigationManager.BaseUri + "api/ChatHub"; }
            get { return CreateApiUrl(SiteState.Alias, "ChatHub"); }
        }
        public async Task<List<ChatHubRoom>> GetChatHubRoomsByModuleIdAsync(int ModuleId)
        {
            return await HttpClient.GetFromJsonAsync<List<ChatHubRoom>>(apiurl + "/getchathubroomsbymoduleid?entityid=" + ModuleId);
        }
        public async Task<ChatHubRoom> GetChatHubRoomAsync(int ChatHubRoomId, int ModuleId)
        {
            return await HttpClient.GetFromJsonAsync<ChatHubRoom>(apiurl + "/getchathubroom/" + ChatHubRoomId + "?entityid=" + ModuleId);
        }
        public async Task<ChatHubRoom> AddChatHubRoomAsync(ChatHubRoom ChatHubRoom)
        {
            var response = await HttpClient.PostAsJsonAsync(apiurl + "/addchathubroom" + "?entityid=" + ChatHubRoom.ModuleId, ChatHubRoom);
            return await response.Content.ReadFromJsonAsync<ChatHubRoom>();
        }
        public async Task UpdateChatHubRoomAsync(ChatHubRoom ChatHubRoom)
        {
            await HttpClient.PutAsJsonAsync(apiurl + "/updatechathubroom/" + ChatHubRoom.Id + "?entityid=" + ChatHubRoom.ModuleId, ChatHubRoom);
        }
        public async Task DeleteChatHubRoomAsync(int ChatHubRoomId, int ModuleId)
        {
            await HttpClient.DeleteAsync(apiurl + "/deletechathubroom/" + ChatHubRoomId + "?entityid=" + ModuleId);
        }
        public async Task DeleteRoomImageAsync(int ChatHubRoomId, int ModuleId)
        {
            await HttpClient.DeleteAsync(apiurl + "/deleteroomimage/" + ChatHubRoomId + "?entityid=" + ModuleId);
        }
        public async Task FixCorruptConnections(int ModuleId)
        {
            await HttpClient.DeleteAsync(apiurl + "/fixcorruptconnections" + "?entityid=" + ModuleId);
        }
        
    }
}
