using Moscow_weather.Models;

namespace Moscow_weather.Repositories.Interfaces;

public interface IWeatherDataRepo
{
    public IQueryable<WeatherData> GetData(int year, int? month, int skip = 0);
    public List<int> GetYears();
    public int AddRange(List<WeatherData> toAdd);
}