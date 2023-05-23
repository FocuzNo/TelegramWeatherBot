namespace TelegramWeatherBot.Models;

public class WeatherResponse
{
    public string? Name { get; set; }
    public TempInfo Main { get; set; }
    public List<WeatherInfo> Weather { get; set; }
}