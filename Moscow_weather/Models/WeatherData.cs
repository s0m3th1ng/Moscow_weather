namespace Moscow_weather.Models;

public class WeatherData
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double? Temperature { get; set; }
    public int? Humidity { get; set; }
    public double? DewPoint { get; set; }
    public int? AtmosphericPressure { get; set; }
    public string? WindDirection { get; set; }
    public int? WindSpeed { get; set; }
    public int? Cloudiness { get; set; }
    public int? CloudinessLowerEdge { get; set; }
    public int? Visibility { get; set; }
    public string? Weather { get; set; }
}