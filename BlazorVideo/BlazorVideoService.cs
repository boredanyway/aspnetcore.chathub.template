using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlazorVideo
{

    public class BlazorVideoService : IAsyncDisposable
    {

        public IDictionary<Guid, BlazorVideoModel> BlazorVideoMaps { get; set; } = new Dictionary<Guid, BlazorVideoModel>();
        public IJSObjectReference Module { get; set; }
        public IJSRuntime JsRuntime { get; set; }
        
        public DotNetObjectReference<BlazorVideoServiceExtension> DotNetObjectRef;
        public BlazorVideoServiceExtension BlazorVideoServiceExtension;

        public event Action RunUpdateUI;

        public Dictionary<Guid, dynamic> LocalStreamTasks { get; set; } = new Dictionary<Guid, dynamic>();
        public Dictionary<Guid, dynamic> RemoteStreamTasks { get; set; } = new Dictionary<Guid, dynamic>();

        public BlazorVideoService(IJSRuntime jsRuntime)
        {
            this.JsRuntime = jsRuntime;
            this.BlazorVideoServiceExtension = new BlazorVideoServiceExtension(this);
            this.DotNetObjectRef = DotNetObjectReference.Create(this.BlazorVideoServiceExtension);
        }
        public async Task InitBlazorVideo()
        {
            if(this.Module == null)
            {
                this.Module = await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorVideo/blazorvideojsinterop.js");
            }
        }
        public async Task InitBlazorVideoMap(string id, BlazorVideoType type)
        {
            IJSObjectReference jsobjref = await this.Module.InvokeAsync<IJSObjectReference>("initblazorvideo", this.DotNetObjectRef, id, type.ToString().ToLower());
            this.AddBlazorVideoMap(id, type, jsobjref);
        }
        
        public void AddBlazorVideoMap(string id, BlazorVideoType type, IJSObjectReference jsobjref)
        {
            if (!this.BlazorVideoMaps.Any(item => item.Value.Id == id))
            {
                this.BlazorVideoMaps.Add(new KeyValuePair<Guid, BlazorVideoModel>(Guid.NewGuid(), new BlazorVideoModel() { Id = id, Type = type, JsObjRef = jsobjref, VideoOverlay = true }));
            }
        }
        public void RemoveBlazorVideoMap(Guid guid)
        {
            if (this.BlazorVideoMaps.Any(item => item.Key == guid))
            {
                this.BlazorVideoMaps.Remove(guid);
            }
        }

        public KeyValuePair<Guid, BlazorVideoModel> GetBlazorVideoMap(string roomId)
        {
            return this.BlazorVideoMaps.FirstOrDefault(item => item.Value.Id == roomId);
        }

        public async Task InitLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("initlocallivestream", connectionId);
        }
        public async Task InitDevicesLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("initdeviceslocallivestream", connectionId);
        }
        public async Task StartBroadcastingLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("startbroadcastinglocallivestream", connectionId);
        }
        public async Task StartSequenceLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("startsequencelocallivestream", connectionId);
        }
        public async Task StopSequenceLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("stopsequencelocallivestream", connectionId); 
        }
        public async Task ContinueLocalLivestreamAsync(string id, string connectionId)
        {
            List<KeyValuePair<Guid, dynamic>> localList = this.LocalStreamTasks.Where(item => item.Value.roomId == id && item.Value.connectionId == connectionId).ToList();
            List<KeyValuePair<Guid, dynamic>> remoteList = this.RemoteStreamTasks.Where(item => item.Value.roomId == id && item.Value.connectionId == connectionId).ToList();

            if (localList.Any() || remoteList.Any())
            {
                await this.StartVideoChat(id, connectionId);
            }
        }
        public async Task CloseLocalLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("closelocallivestream", connectionId);
        }

        public async Task InitRemoteLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("initremotelivestream", connectionId);
        }
        public async Task AppendBufferRemoteLivestream(string dataURI, string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("appendbufferremotelivestream", dataURI, connectionId);
        }
        public async Task CloseRemoteLivestream(string id, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(id);
            await keyvaluepair.Value.JsObjRef.InvokeVoidAsync("closeremotelivestream", connectionId);
        }

        public async Task StartVideoChat(string roomId, string connectionId)
        {
            try
            {
                var keyvaluepair = this.GetBlazorVideoMap(roomId);
                await this.StopVideoChat(keyvaluepair.Value.Id, connectionId);

                if (keyvaluepair.Value.Type == BlazorVideoType.LocalLivestream)
                {
                    await this.InitLocalLivestream(keyvaluepair.Value.Id, connectionId);
                    await this.InitDevicesLocalLivestream(keyvaluepair.Value.Id, connectionId);
                    await this.StartBroadcastingLocalLivestream(keyvaluepair.Value.Id, connectionId);

                    this.BlazorVideoMaps[keyvaluepair.Key] = new BlazorVideoModel() { Id = keyvaluepair.Value.Id, JsObjRef = keyvaluepair.Value.JsObjRef, Type = keyvaluepair.Value.Type, VideoOverlay = false };
                    this.RunUpdateUI.Invoke();

                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellationToken token = tokenSource.Token;
                    Task task = new Task(async () => await this.StreamTaskImplementation(roomId, connectionId, token), token);
                    this.AddLocalStreamTask(keyvaluepair.Value.Id, connectionId, task, tokenSource);
                    task.Start();
                }
                else if(keyvaluepair.Value.Type == BlazorVideoType.RemoteLivestream)
                {
                    await this.InitRemoteLivestream(keyvaluepair.Value.Id, connectionId);
                    this.AddRemoteStreamTask(roomId, connectionId);

                    this.BlazorVideoMaps[keyvaluepair.Key] = new BlazorVideoModel() { Id = keyvaluepair.Value.Id, JsObjRef = keyvaluepair.Value.JsObjRef, Type = keyvaluepair.Value.Type, VideoOverlay = false };
                    this.RunUpdateUI.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task StopVideoChat(string roomId, string connectionId)
        {
            var keyvaluepair = this.GetBlazorVideoMap(roomId);
            if (keyvaluepair.Value.Type == BlazorVideoType.LocalLivestream)
            {
                this.BlazorVideoMaps[keyvaluepair.Key] = new BlazorVideoModel() { Id = keyvaluepair.Value.Id, JsObjRef = keyvaluepair.Value.JsObjRef, Type = keyvaluepair.Value.Type, VideoOverlay = true };
                this.RunUpdateUI.Invoke();

                this.RemoveLocalStreamTask(roomId, connectionId);
                await this.CloseLocalLivestream(roomId, connectionId);
            }
            else if (keyvaluepair.Value.Type == BlazorVideoType.RemoteLivestream)
            {
                this.BlazorVideoMaps[keyvaluepair.Key] = new BlazorVideoModel() { Id = keyvaluepair.Value.Id, JsObjRef = keyvaluepair.Value.JsObjRef, Type = keyvaluepair.Value.Type, VideoOverlay = true };
                this.RunUpdateUI.Invoke();

                this.RemoveRemoteStreamTask(roomId, connectionId);
                await this.CloseRemoteLivestream(roomId, connectionId);
            }
        }
        public async Task RestartStreamTaskIfExists(string roomId, string connectionId)
        {
            if (this.LocalStreamTasks.Any(item => item.Value.roomId == roomId && item.Value.connectionId == connectionId) || this.RemoteStreamTasks.Any(item => item.Value.roomId == roomId && item.Value.connectionId == connectionId))
            {
                await this.StartVideoChat(roomId, connectionId);
            }

            this.RunUpdateUI.Invoke();
        }
        public void AddLocalStreamTask(string roomId, string connectionId, Task task, CancellationTokenSource tokenSource)
        {
            this.RemoveLocalStreamTask(roomId, connectionId);
            dynamic obj = new { roomId = roomId, connectionId = connectionId, task = task, tokenSource = tokenSource };
            this.LocalStreamTasks.Add(Guid.NewGuid(), obj);

            this.RunUpdateUI.Invoke();
        }
        public void RemoveLocalStreamTask(string roomId, string connectionId)
        {
            List<KeyValuePair<Guid, dynamic>> list = this.LocalStreamTasks.Where(item => item.Value.roomId == roomId && item.Value.connectionId == connectionId).ToList();
            if (list.Any())
            {
                KeyValuePair<Guid, dynamic> keyValuePair = list.FirstOrDefault();
                dynamic obj = keyValuePair.Value;

                obj.tokenSource?.Cancel();
                obj.task?.Dispose();

                this.LocalStreamTasks.Remove(keyValuePair.Key);
            }
        }
        public void AddRemoteStreamTask(string roomId, string connectionId)
        {
            var items = this.RemoteStreamTasks.Where(item => item.Value.roomId == roomId && item.Value.connectionId == connectionId);
            if (!items.Any())
            {
                this.RemoteStreamTasks.Add(Guid.NewGuid(), new { roomId = roomId, connectionId = connectionId });
                this.RunUpdateUI.Invoke();
            }
        }
        public void RemoveRemoteStreamTask(string roomId, string connectionId)
        {
            var items = this.RemoteStreamTasks.Where(item => item.Value.roomId == roomId && item.Value.connectionId == connectionId);
            if (items.Any())
            {
                this.RemoteStreamTasks.Remove(items.FirstOrDefault().Key);
            }
        }
        public async Task StreamTaskImplementation(string roomId, string connectionId, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await this.StopSequenceLocalLivestream(roomId, connectionId);
                    await this.StartSequenceLocalLivestream(roomId, connectionId);

                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public async Task DisposeStreamTasksAsync()
        {
            foreach (var task in LocalStreamTasks)
            {
                await this.StopVideoChat(task.Value.roomId, task.Value.connectionId);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeStreamTasksAsync();

            foreach (var keyvaluepair in this.BlazorVideoMaps)
            {
                await keyvaluepair.Value.JsObjRef.DisposeAsync();
            }

            await this.Module.DisposeAsync();
        }

    }

    public class BlazorVideoServiceExtension
    {

        public BlazorVideoService BlazorVideoService { get; set; }
        public event Action<string ,string> OnDataAvailableEventHandler;

        public BlazorVideoServiceExtension(BlazorVideoService blazorVideoService)
        {
            this.BlazorVideoService = blazorVideoService;
        }

        [JSInvokable("OnDataAvailable")]
        public void OnDataAvailable(string dataURI, string id)
        {
            if(!string.IsNullOrEmpty(dataURI) && !string.IsNullOrEmpty(id))
            {
                this.OnDataAvailableEventHandler.Invoke(dataURI, id);
            }
        }

        [JSInvokable("PauseLivestreamTask")]
        public void PauseLivestreamTask(string id)
        {
            List<KeyValuePair<Guid, dynamic>> list = this.BlazorVideoService.LocalStreamTasks.Where(item => item.Value.roomId == id && item.Value.connectionId).ToList();
            if (list.Any())
            {
                KeyValuePair<Guid, dynamic> keyValuePair = list.FirstOrDefault();
                dynamic obj = keyValuePair.Value;
                obj.tokenSource.Cancel();
                obj.task.Dispose();
            }
        }

    }

}
