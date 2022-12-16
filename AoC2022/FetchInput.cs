using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AoC2022;

public class FetchInput
{
    public static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();
        return config;
    }

    public static async Task Fetch(int day)
    {
        var filename = $@"..\..\..\day{day:D2}.input";

        var config = InitConfiguration();
        if (!File.Exists(filename) && !string.IsNullOrEmpty(config["session"]))
        {
            var hc = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/2022/day/{day}/input");
            request.Headers.Add("Cookie", $"session={config["session"]}");
             var response = await hc.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (var fs = File.Create(filename))
                {
                    response.Content.CopyTo(fs, null, CancellationToken.None);
                    fs.Close();
                }
            }
            else
            {
                throw new Exception($"input of day {day} could not be fetched. {filename}");
            }
        }
    }
}

