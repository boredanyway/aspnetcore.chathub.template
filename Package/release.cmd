"..\..\oqtane.framework\oqtane.package\nuget.exe" pack Oqtane.ChatHubs.nuspec 
XCOPY "*.nupkg" "..\..\oqtane.framework\Oqtane.Server\Packages\" /Y
