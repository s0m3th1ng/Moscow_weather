namespace Moscow_weather.Models;

public class BrowseViewModel
{
    public BrowseViewModel(List<int> years)
    {
        DisplayData = false;
        Years = years;
        Error = null;
    }

    public BrowseViewModel()
    {
        Years = new List<int>();
    }
    
    public List<int> Years { get; set; }
    public bool DisplayData { get; set; }
    public int? SelectedYear { get; set; }
    public string? SelectedMonth { get; set; }
    public string? Error { get; set; }
}