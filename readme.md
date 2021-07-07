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

#### Edit _Host.cshtml and paste before the blazor.server.js
```HTML
<script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
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

#### Add to oqtane.server.csproj
```C#  
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="5.0.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.MessagePack" Version="5.0.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" Version="5.0.0" />
<PackageReference Include="BlazorStrap" Version="1.3.3" />
<PackageReference Include="Microsoft.CSharp" Version="4.7.0" />
<PackageReference Include="Microsoft.Composition" Version="1.0.31" />
```

#### Add to oqtane.client.csproj
```C#  
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="5.0.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.MessagePack" Version="5.0.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" Version="5.0.0" />
<PackageReference Include="BlazorStrap" Version="1.3.3" />
<PackageReference Include="Microsoft.CSharp" Version="4.7.0" />
<PackageReference Include="Microsoft.Composition" Version="1.0.31" />
```

#### Example Screenshots

<div class="float-left">
	<img src="https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/screenshot1.png" height="133">
</div>

#### Demo Site

Demo Website [Link](https://www.wasmchat.com/).
