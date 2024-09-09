// See https://aka.ms/new-console-template for more information
using Azure.Identity;
using Microsoft.DotNet.Maestro.Client;

Console.WriteLine("Hello, World!");

var credential = new DefaultAzureCredential();
var azdoToken = await credential.GetTokenAsync(new Azure.Core.TokenRequestContext(["54c17f3d-7325-4eca-9db7-f090bfc765a8/.default"]));
var api = MaestroApiFactory.GetAuthenticated(
    azdoToken.Token,
    managedIdentityId: null,
    disableInteractiveAuth: true);
var result = await api.DefaultChannels.ListAsync(repository: "dotnet/roslyn");
foreach (var item in result)
{
    Console.WriteLine($"{item.Repository} {item.Channel.Name} {item.Branch}");
}

