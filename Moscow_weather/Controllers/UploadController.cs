using Microsoft.AspNetCore.Mvc;
using Moscow_weather.Models;
using Moscow_weather.Services.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Moscow_weather.Controllers;

public class UploadController : Controller
{
    private IUploadService _uploadService;
    
    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    public IActionResult Index()
    {
        return View(new UploadViewModel());
    }

    [HttpPost]
    public IActionResult Index(UploadViewModel model)
    {
        model.InitialView = false;
        if (!model.Files.Any())
        {
            return View(model);
        }

        var uploadResult = _uploadService.UploadFiles(model.Files);
        model.AddedInfo = uploadResult.Item1;
        model.Errors = uploadResult.Item2;
        return View(model);
    }
}