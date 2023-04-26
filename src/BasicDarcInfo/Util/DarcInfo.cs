using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Maestro.Client;
using Microsoft.DotNet.Maestro.Client.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace BasicDarcInfo.Util;

public sealed class DarcInfo
{
    private sealed class DarcSubscriptionInfo
    {
        public ImmutableArray<Subscription> SdkSubscriptions { get; }
        public ImmutableArray<(string Owner, string Name, ImmutableArray<DefaultChannel> DefaultChannels)> RepoDefaultChannels;

        public DarcSubscriptionInfo(
            ImmutableArray<Subscription> sdkSubscriptions,
            ImmutableArray<(string Owner, string Name, ImmutableArray<DefaultChannel> DefaultChannels)> repoDefaultChannels)
        {
            SdkSubscriptions = sdkSubscriptions;
            RepoDefaultChannels = repoDefaultChannels;
        }
    }

    private static readonly TimeSpan CacheLimit = TimeSpan.FromMinutes(5);
    private Tuple<DateTime, DarcSubscriptionInfo>? _cache;

    private ClientFactory ClientFactory { get; }

    public DarcInfo(ClientFactory clientFactory)
    {
        ClientFactory = clientFactory;
    }

    private async Task<DarcSubscriptionInfo> GetDarcSubscriptionInfo(bool useCache)
    {
        if (useCache &&
            _cache is { } tuple &&
            DateTime.UtcNow - tuple.Item1 < CacheLimit)
        {
            return tuple.Item2;
        }

        var info = await GetCore();
        _cache = Tuple.Create(DateTime.UtcNow, info);
        return info;

        async Task<DarcSubscriptionInfo> GetCore()
        {
            var api = ClientFactory.CreateMaestroApi();
            var subscriptions = await api.Subscriptions.ListSubscriptionsAsync(targetRepository: GitHubUtil.GetRepoUri("dotnet", "sdk"));

            var repoList = new List<(string Owner, string Name, ImmutableArray<DefaultChannel> DefaultChannels)>()
            {
                await GetRepository(api, "dotnet", "roslyn"),
                await GetRepository(api, "dotnet", "razor"),
                await GetRepository(api, "dotnet", "msbuild"),
                await GetRepository(api, "dotnet", "format")
            };

            return new DarcSubscriptionInfo(
                subscriptions.ToImmutableArray(),
                repoList.ToImmutableArray());
        }

        static async Task<(string Owner, string Name, ImmutableArray<DefaultChannel> DefaultChannels)> GetRepository(IMaestroApi api, string owner, string name)
        {
            var repository = new Repository(owner, name);
            var channels = await api.DefaultChannels.ListAsync(repository: GitHubUtil.GetRepoUri(owner, name));
            return (owner, name, channels.ToImmutableArray());
        }
    }

    public async Task<List<RepoMergeInfo>> GetRepoMergeInfoList(string targetBranch, bool useCache = true)
    {
        var darcInfo = await GetDarcSubscriptionInfo(useCache);
        var github = ClientFactory.CreateGitHubClient();
        var subscriptions = darcInfo.SdkSubscriptions;
        var list = new List<RepoMergeInfo>();
        foreach (var repo in darcInfo.RepoDefaultChannels)
        {
            var repoUri = GitHubUtil.GetRepoUri(repo.Owner, repo.Name);
            var sub = subscriptions.SingleOrDefault(x => x.TargetBranch == targetBranch && x.SourceRepository == repoUri);
            if (sub is null)
            {
                list.Add(new RepoMergeInfo(
                    repo.Owner,
                    repo.Name,
                    subscription: null,
                    branch: null,
                    commit: null,
                    lastCommitMerge: null));
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
                sub.Channel.Name,
                branch,
                lastCommit,
                lastMerge));
        }

        return list;

    }
}

public sealed class RepoMergeInfo
{
    public string Owner { get; }
    public string Name { get; }
    public string? Subscription { get; }
    public string? Branch { get; }
    public string? LastCommit { get; }
    public DateTimeOffset? LastCommitMerge { get; }

    public string LastCommitUri => LastCommit is { } c
        ? GitHubUtil.GetCommitUri(Owner, Name, c)
        : "";

    public RepoMergeInfo(
        string owner,
        string name,
        string? subscription,
        string? branch,
        string? commit,
        DateTimeOffset? lastCommitMerge)
    {
        Owner = owner;
        Name = name;
        Subscription = subscription;
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
