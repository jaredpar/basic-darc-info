using Octokit;
using Microsoft.DotNet.Maestro.Client;

namespace BasicDarcInfo.Util;

public sealed class ClientFactory
{
    public async Task<GitHubClient> CreateGitHubClientAsync()
    {
        var token = await GetTokenAsync("github");
        var github = new GitHubClient(new ProductHeaderValue("darc-info"))
        {
            Credentials = new Credentials(token)
        };

        return github;
    }

    public async Task<IMaestroApi> CreateMaestroApiAsync()
    {
        var token = await GetTokenAsync("darc");
        var api = ApiFactory.GetAuthenticated("https://maestro-prod.westus2.cloudapp.azure.com", token);
        return api;
    }

    private static async Task<string> GetTokenAsync(string name)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "tokens.txt");
        var lines = await File.ReadAllLinesAsync(filePath);
        return lines
            .Select(x => x.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries))
            .Where(x => x[0] == name)
            .Select(x => x[1])
            .Single();
    }

}
