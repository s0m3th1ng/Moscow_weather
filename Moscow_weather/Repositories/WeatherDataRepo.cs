using Microsoft.EntityFrameworkCore;
using Moscow_weather.Models;
using Moscow_weather.Repositories.Interfaces;

namespace Moscow_weather.Repositories;

public class WeatherDataRepo : IWeatherDataRepo
{
    private readonly ApplicationContext _db;

    public WeatherDataRepo(ApplicationContext db)
    {
        _db = db;
    }

    public IQueryable<WeatherData> GetData(int year, int? month, int skip = 0)
    {
        var firstDay = new DateTime(year, 1, 1);
        var nextYearFirstDay = new DateTime(year + 1, 1, 1);
        var result = _db.Weathers
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .Where(x => x.Date >= firstDay && x.Date < nextYearFirstDay);
        
        if (month != null)
        {
            result = result.Where(x => x.Date.Month == month.Value);
        }

        return result.Skip(skip).Take(50);
    }

    public List<int> GetYears()
    {
        return _db.Weathers.AsNoTracking().Select(x => x.Date.Year).Distinct().ToList();
    }

    public int AddRange(List<WeatherData> toAdd)
    {
        var duplicates = toAdd.Where(x => _db.Weathers.Any(y => y.Date == x.Date)).ToList();
        _db.Weathers.AddRange(toAdd.Except(duplicates));
        _db.SaveChanges();
        
        return duplicates.Count;
    }
}