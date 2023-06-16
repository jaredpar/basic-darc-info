# basic-darc-info
Simple website for displaying darc info

https://darc-info.azurewebsites.net/darcinfo

# Local Development
Get a Maestro token from https://maestro-prod.westus2.cloudapp.azure.com/
Get a GitHub PAT token that has public access only 

Run 

```cmd
> cd src\BasicDarcInfo
> dotnet user-secrets set darc <token value />
> dotnet user-secrets set github <token value />
```

# Deployment
Install the Azure Web App Extension and deploy via VS Code
