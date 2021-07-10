<div align="center">
	<img src="https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/Server/wwwroot/Modules/Oqtane.ChatHubs/wasmchatlogo.png" width="420" title="wasmchat">
</div>

## asp .net core blazor signalR entity framework chat hub template

nuget package not work so far:(

#### Getting Started For Oqtane ChatHub Module Dev

- [x] Get familiar with the [Oqtane Framework](https://github.com/oqtane/oqtane.framework).
- [x] Clone the Oqtane Github Repository in Visual Studio Team Explorer.
- [x] Clone the Oqtane ChatHub Module in VS Team Explorer and build in debug and release mode.
- [ ] get it work somehow good luck anyways

#### Optionally edit Default.razor under oqtane client themes
```HTML
new Resource { ResourceType = ResourceType.Stylesheet, Url = "https://wasmchat.com/Modules/Oqtane.ChatHubs/chat-hub-generated-bootstrap.min.css", CrossOrigin = "anonymous" },
```

#### Edit _Host.cshtml and paste at the end of body
```HTML
<script src="/Modules/Oqtane.ChatHubs/blazorStrap.js"></script>
<script src="/Modules/Oqtane.ChatHubs/blazorvideojsinterop.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/browserresizejsinterop.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/browserresizemap.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/blazorfileuploadjsinterop.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/blazordraggablelistjsinterop.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/blazormodaljsinterop.js" type="module"></script>
<script src="/Modules/Oqtane.ChatHubs/chat-hub-js-interop.js"></script>
```

#### To run locally and debug add this to oqtane.server.csproj
```C#  
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="5.0.4" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" Version="5.0.4" />
<PackageReference Include="BlazorStrap" Version="1.3.3" />
<PackageReference Include="Microsoft.CSharp" Version="4.7.0" />
<PackageReference Include="Microsoft.Composition" Version="1.0.31" />
```

#### To run locally and debug add this to oqtane.client.csproj
```C#  
<PackageReference Include="System.Drawing.Common" Version="5.0.2" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="5.0.4" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" Version="5.0.4" />
<PackageReference Include="BlazorStrap" Version="1.3.3" />
<PackageReference Include="Microsoft.CSharp" Version="4.7.0" />
<PackageReference Include="Microsoft.Composition" Version="1.0.31" />
```

#### Example Screenshots

<div class="float-left">
	<img src="https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/screenshot1.png" height="240">
</div>

#### Demo Site

Demo Website [Link](https://www.wasmchat.com/).
