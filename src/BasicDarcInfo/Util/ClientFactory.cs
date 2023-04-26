using Octokit;
using Microsoft.DotNet.Maestro.Client;

namespace BasicDarcInfo.Util;

public sealed class ClientFactory
{
    private readonly string _githubToken;
    private readonly string _darcToken;

    public ClientFactory(IConfiguration configuration)
    {
        _githubToken = configuration["github"]!;
        _darcToken = configuration["darc"]!;
    }

    public GitHubClient CreateGitHubClient()
    {
        var github = new GitHubClient(new ProductHeaderValue("darc-info"))
        {
            Credentials = new Credentials(_githubToken)
        };

        return github;
    }

    public IMaestroApi CreateMaestroApi()
    {
        var api = ApiFactory.GetAuthenticated("https://maestro-prod.westus2.cloudapp.azure.com", _darcToken);
        return api;
    }
}
