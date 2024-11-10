using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace Gotify2Pushover;

internal class Program
{
    private static readonly string PushoverUserKey = Environment.GetEnvironmentVariable("PUSHOVER_USERKEY") ?? string.Empty;
    private static readonly string PushoverAppKey = Environment.GetEnvironmentVariable("PUSHOVER_APPKEY") ?? string.Empty;
    private static readonly string GotifyHost = Environment.GetEnvironmentVariable("GOTIFY_HOST") ?? string.Empty;
    private static readonly string GotifyToken = Environment.GetEnvironmentVariable("GOTIFY_TOKEN") ?? string.Empty;

    static async Task Main(string[] args)
    {
        var serverUri = new Uri($"{GotifyHost}/stream");
        using (var ws = new ClientWebSocket())
        {
            ws.Options.SetRequestHeader("X-Gotify-Key", GotifyToken);
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Opened connection");

            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(message);

                var msg = JsonConvert.DeserializeObject<GotifyNotification>(message)!;
                await Notify(msg.Message, msg.Title);
            }

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            Console.WriteLine("### closed ###");
        }
    }

    private static async Task Notify(string message, string title)
    {
        try
        {
            Console.WriteLine($"Sending to Pushover: {title}:{message}");

            var parameters = new Dictionary<string, string>
            {
                ["token"] = PushoverAppKey,
                ["user"] = PushoverUserKey,
                ["message"] = message,
                ["title"] = title
            };

            // Replace with your code to send a regular HTTP POST request
            // Example:
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://api.pushover.net/1/messages.json", new
                    FormUrlEncodedContent(parameters));
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public class GotifyNotification
    {
        public int Id { get; set; }
        public int Appid { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; }
        //public Extras Extras { get; set; }
        public DateTime Date { get; set; }
    }

    //public class Extras
    //{
    //    public ClientDisplay ClientDisplay { get; set; }
    //}

    //public class ClientDisplay
    //{
    //    public string ContentType { get; set; }
    //}

}
