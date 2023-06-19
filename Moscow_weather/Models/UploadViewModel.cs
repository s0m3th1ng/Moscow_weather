namespace Moscow_weather.Models;

public class UploadViewModel
{
    public UploadViewModel()
    {
        Files = new List<IFormFile>();
        InitialView = true;
        Errors = new List<string>();
        AddedInfo = null;
    }

    public List<IFormFile> Files { get; set; }
    public bool InitialView { get; set; }
    public List<string> Errors { get; set; }
    public string? AddedInfo { get; set; }
}