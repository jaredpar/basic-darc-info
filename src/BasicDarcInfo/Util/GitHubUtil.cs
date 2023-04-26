namespace BasicDarcInfo.Util;

public static class GitHubUtil
{
    public static string GetRepoUri(string owner, string name) => $"https://github.com/{owner}/{name}";
    public static string GetCommitUri(string owner, string name, string commit) =>
        $"{GetRepoUri(owner, name)}/commits/{commit}";
}
