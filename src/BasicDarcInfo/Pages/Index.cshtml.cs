using BasicDarcInfo.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicDarcInfo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly DarcInfo _darcInfo;

    public List<RepoMergeInfo> RepoMergeInfoList { get; } = new();

    public IndexModel(DarcInfo darcInfo, ILogger<IndexModel> logger)
    {
        _darcInfo = darcInfo;
        _logger = logger;
    }

    public async Task OnGet()
    {
        var list = await _darcInfo.GetRepoMergeInfoList("release/7.0.4xx");
        RepoMergeInfoList.AddRange(list);
    }
}
