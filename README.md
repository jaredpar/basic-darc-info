# basic-darc-info
Simple website for displaying darc info

https://darc-info.azurewebsites.net/darcinfo

# Local Development
Get a Maestro token from https://maestro-prod.westus2.cloudapp.azure.com/
Get a GitHub PAT token that has public access only 

## Run directly on machine

```cmd
> cd src\BasicDarcInfo
> dotnet user-secrets set darc <token value />
> dotnet user-secrets set github <token value />
```

## Run in docker

Create an environment file with the following structure 

```env
ASPNETCORE_ENVIRONMENT=Development
DARC=
GITHUB=
```

Then build and run the docker image

```cmd
> docker build -t darc-info .
> docker run --env-file ..\env.darc-info -p 8080:8080 -it darc-info
```

# Deployment
Install the Azure Web App Extension and deploy via VS Code
