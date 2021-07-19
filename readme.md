<div align="center">
	<img src="https://raw.githubusercontent.com/boredanyway/aspnetcore.chathub.template/master/Server/wwwroot/Modules/Oqtane.ChatHubs/wasmchatlogo.png" class="img-fluid" width="420" title="wasmchat">
</div>

## asp .net core blazor signalR entity framework video chat hub template

You can download the latest oqtane.chathubs package 4.0.2 <a href="https://www.nuget.org/packages/Oqtane.ChatHubs/">here</a>. Then follow the instructions below.

#### Getting Started For Oqtane ChatHub Module Dev

- [x] Check out the [Oqtane Blog Module](https://github.com/oqtane/oqtane.blogs).
- [x] Check out the [Oqtane Framework](https://github.com/oqtane/oqtane.framework).
- [x] Clone the Oqtane Github Repository in Visual Studio Team Explorer.
- [x] Clone the Oqtane ChatHubs Module in VS Team Explorer and build in debug and release mode.
- [ ] get it work somehow good luck anyways

#### Optional edit Default.razor under oqtane client oqtane theme and uncomment the bootstrap cyborg css or create your own bootstrap css
```HTML
new Resource { 
	ResourceType = ResourceType.Stylesheet, 
	Url = "/Modules/Oqtane.ChatHubs/chat-hub-generated-bootstrap.min.css", 
	CrossOrigin = "anonymous" },
```

#### Add this to oqtane.server.csproj
```C#  
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="5.0.4" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson" Version="5.0.4" />
<PackageReference Include="BlazorStrap" Version="1.3.3" />
<PackageReference Include="Microsoft.CSharp" Version="4.7.0" />
<PackageReference Include="Microsoft.Composition" Version="1.0.31" />
```

#### And add this to oqtane.client.csproj
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
