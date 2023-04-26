using BasicDarcInfo.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicDarcInfo.Pages;

public class DarcInfoModel : PageModel
{
    private readonly ILogger<DarcInfoModel> _logger;
    private readonly DarcInfo _darcInfo;

    public string Release { get; set; } = "";
    public List<RepoMergeInfo> RepoMergeInfoList { get; } = new();

    public DarcInfoModel(DarcInfo darcInfo, ILogger<DarcInfoModel> logger)
    {
        _darcInfo = darcInfo;
        _logger = logger;
    }

    public async Task OnGet(string release)
    {
        var branch = release == "main" ? release : $"release/{release}";
        var list = await _darcInfo.GetRepoMergeInfoList(branch);
        Release = release;
        RepoMergeInfoList.AddRange(list);
    }
}
