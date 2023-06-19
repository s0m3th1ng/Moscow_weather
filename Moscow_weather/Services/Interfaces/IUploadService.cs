namespace Moscow_weather.Services.Interfaces;

public interface IUploadService
{
    public (string, List<string>) UploadFiles(List<IFormFile> files);
}