![Module](https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/Server/wwwroot/Modules/Oqtane.ChatHubs/images/wasmchatlogo.png "wasm chat logo")

## asp .net core blazor signalR entity framework chat hub template

The oqtane chathubs module allows developers to code and run realtime chat. Post issues anytime you want it is okay and appreciated i will reply as soon as im ready and fixed it anyhow. You can even debug with two seperated solutions like the chathub module solution and the oqtane solution. First build in debug and release mode. Then run the oqtane project from command line in release mode and open the url. After this go to the visual studio chathub module solution and debug -> attach to process (oqtane server). This should work actually.

#### Getting Started For Oqtane ChatHub Module Dev

- [x] Get familiar with the [Oqtane Framework](https://github.com/oqtane/oqtane.framework).
- [x] Clone the Oqtane Github Repository in Visual Studio Team Explorer.
- [x] Clone the Oqtane ChatHub Module in VS Team Explorer and build in debug and release mode.
- [ ] get it work somehow good luck anyways

#### Edit _Host.cshtml end of head tag
```HTML
<link href="_content/BlazorAlerts/css/blazoralerts.min.css" rel="stylesheet" />
<link href="modules/oqtane.chathubs/chat-hub-stylesheets.css" rel="stylesheet" />
```

#### Edit Default.razor under oqtane client themes
```HTML
new Resource { ResourceType = ResourceType.Stylesheet, Url = "https://wasmchat.com/modules/oqtane.chathubs/chat-hub-generated-bootstrap.min.css", CrossOrigin = "anonymous" },
```

#### Edit _Host.cshtml end of body tag
```HTML
<script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>

<script async defer src="https://buttons.github.io/buttons.js"></script>
<script src="_content/BlazorStrap/blazorStrap.js"></script>
<script src="_content/BlazorVideo/blazorvideojsinterop.js" type="module"></script>
<script src="_content/BlazorBrowserResize/browserresizejsinterop.js" type="module"></script>
<script src="_content/BlazorBrowserResize/browserresizemap.js" type="module"></script>
<script src="_content/BlazorFileUpload/blazorfileuploadjsinterop.js" type="module"></script>
<script src="_content/BlazorDraggableList/blazordraggablelistjsinterop.js" type="module"></script>
<script src="_content/BlazorModal/blazormodaljsinterop.js" type="module"></script>
<script src="modules/oqtane.chathubs/chat-hub-js-interop.js"></script>
```

#### Edit startup.cs configure services methode
```C#
services.AddScoped<BlazorAlertsService, BlazorAlertsService>();
services.AddScoped<BlazorDraggableListService, BlazorDraggableListService>();
services.AddScoped<BlazorFileUploadService, BlazorFileUploadService>();
services.AddScoped<BlazorColorPickerService, BlazorColorPickerService>();
services.AddScoped<BlazorVideoService, BlazorVideoService>();
services.AddScoped<BlazorBrowserResizeService, BlazorBrowserResizeService>();
services.AddScoped<BlazorModalService, BlazorModalService>();

services.AddServerSideBlazor()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 512 * 1024);

services.AddMvc()
    .AddNewtonsoftJson(options => 
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

services.AddSignalR()
    .AddHubOptions<ChatHub>(options =>
    {
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
        options.MaximumReceiveMessageSize = Int64.MaxValue;
        options.StreamBufferCapacity = Int32.MaxValue;
    })
    .AddMessagePackProtocol()
    .AddNewtonsoftJsonProtocol(options =>
    {
        options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
```

#### Edit startup.cs configure runtime pipeline
```C#	
endpoints.MapHub<ChatHub>("/chathub", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
    options.ApplicationMaxBufferSize = Int64.MaxValue;
    options.TransportMaxBufferSize = Int64.MaxValue;
    options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(10);
    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
});
```

#### Edit TenantResolver.cs
```C#
if (segments.Length > 1 && (segments[1] == "api" || segments[1] == "pages") && segments[0] != "~")
{
	aliasId = int.Parse(segments[0]);
}
else if (segments[0] == "chathub")
{
	aliasId = 1;
}
```

#### Module Dependencies so far
```C#
<dependency id="Oqtane.Framework" version="2.0.0" />      
<dependency id="System.Drawing.Common" version="5.0.0" />
<dependency id="Microsoft.CSharp" version="4.7.0" />
<dependency id="BlazorStrap" version="1.3.3" />
<dependency id="Microsoft.AspNetCore.SignalR.Client" version="5.0.0" />
<dependency id="Microsoft.AspNetCore.SignalR.Protocols.MessagePack" version="5.0.0" />
<dependency id="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" version="5.0.0" />
<dependency id="Microsoft.Composition" version="1.0.31" />
```

#### Example Screenshots

<div class="float-left">
	<img src="https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/screenshot1.png" height="133">
</div>

#### Demo Site

Demo Website [Link](https://www.wasmchat.com/).
