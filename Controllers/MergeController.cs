using Microsoft.AspNetCore.Mvc;
using AutoMergeWeb.Services;

namespace AutoMergeWeb.Controllers
{
    public class MergeController : Controller
    {
        private readonly MergeService _mergeService;

        public MergeController(MergeService mergeService)
        {
            _mergeService = mergeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Merge([FromBody] MergeRequest request)
        {
            var result = _mergeService.Merge(request.Original, request.Version1, request.Version2, request.IgnoreWhitespace);
            return Json(result);
        }
    }

    public class MergeRequest
    {
        public string Original { get; set; } = "";
        public string Version1 { get; set; } = "";
        public string Version2 { get; set; } = "";
        public bool IgnoreWhitespace { get; set; } = false;
    }
}