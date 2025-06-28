using System;
using System.Net.Http;
using System.Threading.Tasks;
class Weather
{
    public currentWeather current_weather { get; set; }

    public string Display()
    {
        return $"Temperature: {current_weather.temperature}°C\n" +
               $"Wind Speed: {current_weather.windspeed} m/s\n" +
               $"Wind Direction: {current_weather.winddirection}°\n" +
               $"Is Day: {current_weather.is_day}\n" +
               $"Time: {current_weather.time}\n";
    }
    
}
class currentWeather
{

    public double temperature { get; set; }
    public double windspeed { get; set; }
    public int winddirection { get; set; }
    public int is_day { get; set; }
    public string time { get; set; }
}
class WeatherApp
{
    public Dictionary<string, (double lat, double lon)> coordinates = new Dictionary<string, (double lat, double lon)>
    {
        { "London", (51.5074, -0.1278) },
        { "New York", (40.7128, -74.0060) },
        { "Tokyo", (35.6762, 139.6503) },
        { "Sydney", (-33.8688, 151.2093) },
        { "Berlin", (52.5200, 13.4050) },
        { "Paris", (48.8566, 2.3522) },
        { "Moscow", (55.7558, 37.6173) },
        { "Cairo", (30.0444, 31.2357) },
        { "Rio de Janeiro", (-22.9068, -43.1729) },
        { "Cape Town", (-33.9249, 18.4241) },
        { "Mumbai", (19.0760, 72.8777) },
        { "Toronto", (43.6511, -79.3470) },
        { "Los Angeles", (34.0522, -118.2437) },
        { "Chicago", (41.8781, -87.6298) },
        { "Dubai", (25.276987, 55.296249) },
        { "Singapore", (1.3521, 103.8198) },
        { "Bangkok", (13.7563, 100.5018) },
        { "Buenos Aires", (-34.6037, -58.3816) },
        { "Istanbul", (41.0082, 28.9784) },
        { "Seoul", (37.5665, 126.9780) },
        { "Kuala Lumpur", (3.139,101.6869) },
        { "Barcelona", (41.3851, 2.1734) },
        { "Madrid", (40.4168, -3.7038) },
        { "Amsterdam", (52.3676, 4.9041) },
        { "Lisbon", (38.7223, -9.1393) },
        { "Athens", (37.9838, 23.7275) },
        { "Hanoi", (21.0285, 105.8542) },
        { "Sao Paulo", (-23.5505, -46.6333) }
    };
    private HttpClient client = new HttpClient();
    public async Task<Weather> GetWeatherAsync(string city)
    {
        if (!coordinates.TryGetValue(city, out (double, double) coords))
        {
            throw new ArgumentException("City not found in the coordinates dictionary.");
        }
        double lat = coords.Item1;
        double lon = coords.Item2;
        string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
        string response = await client.GetStringAsync(url);
        if (string.IsNullOrEmpty(response))
        {
            throw new Exception("Failed to retrieve weather data.");
        }
        var weatherData = System.Text.Json.JsonSerializer.Deserialize<Weather>(response);
        return weatherData ?? throw new Exception("Deserialization failed.");
    }
}

class Program
{
    WeatherApp weatherApp = new WeatherApp();
    void DisplaySpecificWeather(string city)
    {
        try
        {
            Weather weather = weatherApp.GetWeatherAsync(city).Result;
            Console.WriteLine($"Weather in {city}:\n{weather.Display()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving weather data: {ex.Message}");
        }
    }
    public async Task SlowDisplayWeatherAsync()
    {
        var cities = new List<string>
        {
            "London", "New York", "Tokyo", "Sydney", "Berlin",
            "Paris", "Moscow", "Cairo", "Rio de Janeiro", "Cape Town",
            "Mumbai", "Toronto", "Los Angeles", "Chicago", "Dubai",
            "Singapore", "Bangkok", "Buenos Aires", "Istanbul",
            "Seoul", "Kuala Lumpur", "Barcelona", "Madrid",
            "Amsterdam", "Lisbon", "Athens", "Hanoi", "Sao Paulo"
        };
        foreach (var city in cities)
        {
            try
            {
                Weather weather = await weatherApp.GetWeatherAsync(city);
                Console.WriteLine($"Weather in {city}:\n{weather.Display()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving weather data for {city}: {ex.Message}");
            }
            await Task.Delay(6000); // Slow down the display
        }
    }
    
    static async Task Main(string[] args)
    {
        bool continueLoop = true;
        while (continueLoop)
        {
            Console.WriteLine("Welcome to the Weather App!");
            Console.WriteLine("1. Display specific weather for a city");
            Console.WriteLine("2. Display weather for all cities slowly");
            Console.WriteLine("3. Exit");
            Console.Write("Please select an option: ");
            string choice = Console.ReadLine();
            Program program = new Program();
            switch (choice)
            {
                case "1":
                    Console.Write("Enter the city name: ");
                    string city = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(city))
                    {
                        Console.WriteLine("City name cannot be empty.");
                        continue;
                    }
                    program.DisplaySpecificWeather(city);
                    break;
                case "2":
                    await program.SlowDisplayWeatherAsync();
                    break;
                case "3":
                    continueLoop = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }
}