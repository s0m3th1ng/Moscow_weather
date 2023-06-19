using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Moscow_weather.Models;
using Moscow_weather.Repositories.Interfaces;

namespace Moscow_weather.Controllers;

public class BrowseController : Controller
{
    private readonly IWeatherDataRepo _weatherDataRepo;
    
    public BrowseController(IWeatherDataRepo weatherDataRepo)
    {
        _weatherDataRepo = weatherDataRepo;
    }

    public IActionResult Index()
    {
        var years = _weatherDataRepo.GetYears();
        var model = new BrowseViewModel(years);
        if (!years.Any())
        {
            model.Error = "No records added";
        }
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(BrowseViewModel model)
    {
        if (model.SelectedYear == null)
        {
            model.Error = "Select year";
            return View(model);
        }
        
        var month = DateTime.TryParseExact(
            model.SelectedMonth,
            "MMMM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var m)
            ? m.Month : (int?)null;
        var data = _weatherDataRepo.GetData(model.SelectedYear.Value, month);
        model.DisplayData = true;
        if (!data.Any())
        {
            model.DisplayData = false;
            model.Error = "No records found";
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult GetData(int year, string month, int firstItem = 0)
    {
        var monthNum = DateTime.TryParseExact(
            month,
            "MMMM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var m)
            ? m.Month : (int?)null;
        var data = _weatherDataRepo.GetData(year, monthNum, firstItem).ToList();
        if (!data.Any())
        {
            return StatusCode(204);
        }

        return PartialView("TableData", data);
    }
}