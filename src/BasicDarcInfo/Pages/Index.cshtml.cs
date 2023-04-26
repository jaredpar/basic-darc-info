using BasicDarcInfo.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicDarcInfo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly DarcInfo _darcInfo;

    public string Release { get; set; } = "";
    public List<RepoMergeInfo> RepoMergeInfoList { get; } = new();

    public IndexModel(DarcInfo darcInfo, ILogger<IndexModel> logger)
    {
        _darcInfo = darcInfo;
        _logger = logger;
    }

    public async Task OnGet(string release)
    {
        var list = await _darcInfo.GetRepoMergeInfoList($"release/{release}");
        Release = release;
        RepoMergeInfoList.AddRange(list);
    }
}
