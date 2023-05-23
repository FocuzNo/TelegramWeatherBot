using System.Net;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramWeatherBot.Models;

namespace TelegramWeatherBot;

public static class Program
{
    private static string Token { get; set; } = "6211617462:AAGt9mn7PrE-14XzXE42bVdol2j8TKznzcY";
    private static TelegramBotClient? _client;
    private static string? CityName { get; set; }
    private static string? _nameOfCity;
    private static float _tempOfCity;
    private static string? _tempDescription;

    public static void Main(string[] args)
    {
        _client = new TelegramBotClient(Token);
        using var cts = new CancellationTokenSource();
        ReceiverOptions receiverOptions = new();
        _client.StartReceiving(UpdateAsync, ErrorAsync, receiverOptions, cts.Token);
        Console.ReadLine();
    }
    
    private static async Task UpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message?.Type == MessageType.Text)
        {
            CityName = message.Text;
            if (CityName != null) Weather(CityName);
            if (_client != null)
                await _client.SendTextMessageAsync(message.Chat.Id,
                    $"\nTemperature in {_nameOfCity}: {Math.Round(_tempOfCity)} °C" +
                    $"\n{_tempDescription}" +
                    $"\nTelegram bot was developed by Arian Zhurovich", cancellationToken: token);
        }
    }

    private static void Weather(string? nameCity)
    {
        try
        {
            string url = "https://api.openweathermap.org/data/2.5/weather?q=" + nameCity + "&unit=metric&appid=2351aaee5394613fc0d14424239de2bd";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string response;

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            WeatherResponse? weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);
            
            if (weatherResponse != null)
            {
                _nameOfCity = weatherResponse.Name;
                _tempOfCity = weatherResponse.Main.Temp - 273;
                var desc = weatherResponse.Weather.Select(x => x.Description).ToArray();
                foreach (var description in desc)
                {
                    _tempDescription = description;
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    private static Task ErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }
}