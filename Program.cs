using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


namespace WeatherApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Load environment variables from .env file if present
            DotNetEnv.Env.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    // This is your main window (form)
    public class MainForm : Form
    {
    // Read the OpenWeatherMap API key from an environment variable, never returns null
    private static string ApiKey => Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? string.Empty;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        private TextBox txtCity;
        private Button btnGetWeather;
        private Label lblResult;

        public MainForm()
        {
            // Set up the form
            this.Text = "Simple Weather App";
            this.Width = 350;
            this.Height = 180;

            // City input
            txtCity = new TextBox();
            txtCity.Left = 10;
            txtCity.Top = 10;
            txtCity.Width = 200;
            txtCity.PlaceholderText = "Enter city name";

            // Button
            btnGetWeather = new Button();
            btnGetWeather.Text = "Get Weather";
            btnGetWeather.Left = 220;
            btnGetWeather.Top = 10;
            btnGetWeather.Click += BtnGetWeather_Click;

            // Result label
            lblResult = new Label();
            lblResult.Left = 10;
            lblResult.Top = 50;
            lblResult.Width = 320;
            lblResult.Height = 60;
            lblResult.Text = "";

            // Add controls to the form
            this.Controls.Add(txtCity);
            this.Controls.Add(btnGetWeather);
            this.Controls.Add(lblResult);
        }

        // This runs when the button is clicked
    private async void BtnGetWeather_Click(object? sender, EventArgs e)
        {
            string city = txtCity.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("Please enter a city name.");
                return;
            }

            lblResult.Text = "Loading...";
            try
            {
                string weatherData = await GetWeatherDataAsync(city);
                ParseAndDisplayWeather(weatherData);
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Error: {ex.Message}";
            }
        }

        // Fetch weather data from OpenWeatherMap
        private async Task<string> GetWeatherDataAsync(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric";
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("City not found or API error.");
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        // Parse the JSON and show the result
        private void ParseAndDisplayWeather(string weatherData)
        {
            var json = JObject.Parse(weatherData);
            string temperature = json["main"]?["temp"]?.ToString() ?? "N/A";
            string description = json["weather"]?[0]?["description"]?.ToString() ?? "N/A";
            lblResult.Text = $"Temperature: {temperature}°C\nDescription: {description}";
        }
    }
}
