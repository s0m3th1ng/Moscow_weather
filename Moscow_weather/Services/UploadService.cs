using System.Globalization;
using Moscow_weather.Models;
using Moscow_weather.Repositories.Interfaces;
using Moscow_weather.Services.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Moscow_weather.Services;

public class UploadService : IUploadService
{
    private readonly IWeatherDataRepo _weatherDataRepo;

    public UploadService(IWeatherDataRepo weatherDataRepo)
    {
        _weatherDataRepo = weatherDataRepo;
    }
    
    public (string, List<string>) UploadFiles(List<IFormFile> files)
    {
        List<string> errors = new List<string>();
        List<WeatherData> toAdd = new List<WeatherData>();

        foreach (var formFile in files)
        {
            XSSFWorkbook workbook;
            using (var file = formFile.OpenReadStream())
            {
                workbook = new XSSFWorkbook(file);
            }

            var error = workbook.NumberOfSheets == 12
                ? null
                : $"<b>Wrong number of sheets.</b> <u style:'text-decoration: underline;'>File:</u> {formFile.FileName}. <i>File ignored.</i>";
            if (error != null)
            {
                errors.Add(error);
                continue;
            }

            for (var i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                
                error = GetSheetError(sheet, formFile.FileName);
                if (error != null)
                {
                    errors.Add(error);
                    continue;
                }
                
                for (var j = 4; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);

                    error = GetRowError(row, formFile.FileName, sheet.SheetName);
                    if (error != null)
                    {
                        errors.Add(error);
                        continue;
                    }

                    toAdd.Add(CreateDataEntity(row));
                }
            }
        }

        var duplicatesNum = _weatherDataRepo.AddRange(toAdd);
        var addedInfo = $"Added {toAdd.Count - duplicatesNum} records.";
        if (duplicatesNum > 0)
        {
            addedInfo = $"Skipped {duplicatesNum} duplicates. " + addedInfo;
        }
        
        return (addedInfo, errors);
    }

    private static string? GetSheetError(ISheet sheet, string fileName)
    {
        return string.Equals(sheet.GetRow(2)?.GetCell(0)?.StringCellValue, "Дата") &&
               string.Equals(sheet.GetRow(2)?.GetCell(1)?.StringCellValue, "Время") &&
               string.Equals(sheet.GetRow(3)?.GetCell(1)?.StringCellValue, "(московское)") &&
               string.Equals(sheet.GetRow(2)?.GetCell(2)?.StringCellValue, "Т") &&
               string.Equals(sheet.GetRow(2)?.GetCell(3)?.StringCellValue, "Отн. влажность") &&
               string.Equals(sheet.GetRow(3)?.GetCell(3)?.StringCellValue, "воздуха, %") &&
               string.Equals(sheet.GetRow(2)?.GetCell(4)?.StringCellValue, "Td") &&
               string.Equals(sheet.GetRow(2)?.GetCell(5)?.StringCellValue, "Атм. давление,") &&
               string.Equals(sheet.GetRow(3)?.GetCell(5)?.StringCellValue, "мм рт.ст.") &&
               string.Equals(sheet.GetRow(2)?.GetCell(6)?.StringCellValue, "Направление") &&
               string.Equals(sheet.GetRow(3)?.GetCell(6)?.StringCellValue, "ветра") &&
               string.Equals(sheet.GetRow(2)?.GetCell(7)?.StringCellValue, "Скорость") &&
               string.Equals(sheet.GetRow(3)?.GetCell(7)?.StringCellValue, "ветра, м/с") &&
               string.Equals(sheet.GetRow(2)?.GetCell(8)?.StringCellValue, "Облачность,") &&
               string.Equals(sheet.GetRow(3)?.GetCell(8)?.StringCellValue, "%") &&
               string.Equals(sheet.GetRow(2)?.GetCell(9)?.StringCellValue, "h") &&
               string.Equals(sheet.GetRow(2)?.GetCell(10)?.StringCellValue, "VV") &&
               string.Equals(sheet.GetRow(2)?.GetCell(11)?.StringCellValue, "Погодные явления")
                ? null
                : $"<b>Wrong headers.</b> <u style:'text-decoration: underline;'>File:</u> {fileName}, <u style:'text-decoration: underline;'>Sheet:</u> {sheet.SheetName}. <i>Sheet ignored.</i>";
    }

    private static string? GetRowError(IRow row, string fileName, string sheetName)
    {
        for (int i = 0; i < 12; i++)
        {
            row.GetCell(i)?.SetCellType(CellType.String);
        }

        var date = row.GetCell(0)?.StringCellValue;
        var time = row.GetCell(1)?.StringCellValue;
        
        return DateTime.TryParseExact(
            $"{date} {time}", //$"{date} {time} + 3"
            "dd.MM.yyyy HH:mm", //"dd.MM.yyyy HH:mm z" for UTC conversion
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _)
            ? null
            : $"<b>Wrong date or time.</b> <u style:'text-decoration: underline;'>File:</u> {fileName}, <u style:'text-decoration: underline;'>Sheet:</u> {sheetName}, <u style:'text-decoration: underline;'>Row:</u> {row.RowNum + 1}. <i>Row ignored.</i>";
    }

    private static WeatherData CreateDataEntity(IRow row)
    {
        var date = row.GetCell(0).StringCellValue;
        var time = row.GetCell(1).StringCellValue;
        var dateTime = DateTime.ParseExact($"{date} {time}", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

        var style = NumberStyles.Number;
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        
        return new WeatherData()
        {
            Date = dateTime,
            Temperature = double.TryParse(row.GetCell(2)?.StringCellValue.Replace(',', '.'), style, culture, out var t)
                ? t : null,
            Humidity = int.TryParse(row.GetCell(3)?.StringCellValue, out var h) ? h : null,
            DewPoint = double.TryParse(row.GetCell(4)?.StringCellValue.Replace(',', '.'), style, culture, out var dp)
                ? dp : null,
            AtmosphericPressure = int.TryParse(row.GetCell(5)?.StringCellValue, out var ap) ? ap : null,
            WindDirection = row.GetCell(6)?.StringCellValue,
            WindSpeed = int.TryParse(row.GetCell(7)?.StringCellValue, out var ws) ? ws : null,
            Cloudiness = int.TryParse(row.GetCell(8)?.StringCellValue, out var c) ? c : null,
            CloudinessLowerEdge = int.TryParse(row.GetCell(9)?.StringCellValue, out var cle) ? cle : null,
            Visibility = int.TryParse(row.GetCell(10)?.StringCellValue, out var v) ? v : null,
            Weather = row.GetCell(11)?.StringCellValue
        };
    }
}