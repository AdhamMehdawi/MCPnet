## MCPnet NuGet Packages

This directory contains the NuGet packages for the MCPnet library:

1. adham.mehdawi.MCPnet.Core.1.0.0.nupkg - Core interfaces and models
2. adham.mehdawi.MCPnet.Client.1.0.0.nupkg - Client implementation
3. adham.mehdawi.MCPnet.1.0.0.nupkg - Metapackage that references both

### Instructions for Manual Publishing

1. Go to https://www.nuget.org/packages/manage/upload
2. Log in with your adham.mehdawi account
3. Click "Upload" and select the packages in this order:
   a. First upload adham.mehdawi.MCPnet.Core.1.0.0.nupkg
   b. Then upload adham.mehdawi.MCPnet.Client.1.0.0.nupkg 
   c. Finally upload adham.mehdawi.MCPnet.1.0.0.nupkg

### Alternative Method - CLI Publishing

If you want to try the command line approach again:

1. Create a new API key at https://www.nuget.org/account/apikeys
2. Ensure the key has permission for "adham.mehdawi.*" package IDs
3. Push the packages in the correct order:

```
dotnet nuget push adham.mehdawi.MCPnet.Core.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
dotnet nuget push adham.mehdawi.MCPnet.Client.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
dotnet nuget push adham.mehdawi.MCPnet.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Note: Replace YOUR_API_KEY with the actual API key.

### Package Information

The packages have been set up with the following metadata:
- Package ID: adham.mehdawi.MCPnet.*
- Version: 1.0.0
- Author: Adham Mehdawi
- Description: A .NET client library for connecting with Multimodal Capability Protocol (MCP) servers
- License: MIT
- Project URL: https://github.com/AdhamMehdawi/MCPnet
- Repository URL: https://github.com/AdhamMehdawi/MCPnet.git 