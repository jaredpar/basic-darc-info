using Microsoft.DotNet.Maestro.Client.Models;

namespace BasicDarcInfo.Util;

public sealed class DarcInfo
{
    private ClientFactory ClientFactory { get; }

    public DarcInfo(ClientFactory clientFactory)
    {
        ClientFactory = clientFactory;
    }

    public async Task<List<RepoMergeInfo>> GetRepoMergeInfoList(string targetBranch)
    {
        var api = await ClientFactory.CreateMaestroApiAsync();
        var subscriptions = await api.Subscriptions.ListSubscriptionsAsync(targetRepository: GitHubUtil.GetRepoUri("dotnet", "sdk"));

        var repoList = new List<(string Owner, string Name, List<DefaultChannel> DefaultChannels)>()
        {
            await GetRepository("dotnet", "roslyn"),
            await GetRepository("dotnet", "razor")
        };

        var github = await ClientFactory.CreateGitHubClientAsync();
        var list = new List<RepoMergeInfo>();
        foreach (var repo in repoList)
        {
            var repoUri = GitHubUtil.GetRepoUri(repo.Owner, repo.Name);
            var sub = subscriptions.SingleOrDefault(x => x.TargetBranch == targetBranch && x.SourceRepository == repoUri);
            if (sub is null)
            {
                continue;
            }

            string? branch = null;
            if (repo.DefaultChannels.FirstOrDefault(x => x.Channel.Id == sub.Channel.Id) is {} defaultChannel)
            {
                branch = defaultChannel.Branch;
            }

            string? lastCommit = null;
            DateTimeOffset? lastMerge = null;
            if (sub.LastAppliedBuild?.Commit is string commit)
            {
                lastCommit = commit;
                var commitInfo = await github.Git.Commit.Get(repo.Owner, repo.Name, commit);
                lastMerge = commitInfo.Committer.Date;
            }

            list.Add(new RepoMergeInfo(
                repo.Owner,
                repo.Name,
                branch,
                lastCommit,
                lastMerge));
        }

        return list;

        async Task<(string Owner, string Name, List<DefaultChannel> DefaultChannels)> GetRepository(string owner, string name)
        {
            var repository = new Repository(owner, name);
            var channels = await api.DefaultChannels.ListAsync(repository: GitHubUtil.GetRepoUri(owner, name));
            return (owner, name, channels.ToList());
        }
    }
}

public sealed class RepoMergeInfo
{
    public string Owner { get; }
    public string Name { get; }
    public string? Branch { get; }
    public string? LastCommit { get; }
    public DateTimeOffset? LastCommitMerge { get; }

    public string LastCommitUri => LastCommit is { } c
        ? GitHubUtil.GetCommitUri(Owner, Name, c)
        : "";

    public RepoMergeInfo(
        string owner,
        string name,
        string? branch,
        string? commit,
        DateTimeOffset? lastCommitMerge)
    {
        Owner = owner;
        Name = name;
        Branch = branch;
        LastCommit = commit;
        LastCommitMerge = lastCommitMerge;
    }

    public override string ToString() => $"{Owner}/{Name} {Branch}";
}

file class Repository
{
    public string Owner { get;} 
    public string Name { get; }

    public List<DefaultChannel> DefaultChannels { get; } = new();

    public string GitHubUri => $"https://github.com/{Owner}/{Name}";

    public Repository(string owner, string name)
    {
        Owner = owner;
        Name = name;
    }

}
