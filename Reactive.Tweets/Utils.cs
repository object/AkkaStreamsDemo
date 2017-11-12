using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Stream = Tweetinvi.Stream;
using System.Net.Http;

namespace Reactive.Tweets
{
    public static class Utils
    {
        public static void StartTweetStream(IActorRef actor)
        {
            var stream = Stream.CreateSampleStream();
            stream.TweetReceived += (_, arg) =>
            {
                if (arg.Tweet.Coordinates != null)
                {
                    arg.Tweet.Text = arg.Tweet.Text.Replace("\r", " ").Replace("\n", " ");
                    var json = JsonConvert.SerializeObject(arg.Tweet);
                    File.AppendAllText("tweets.txt", $"{json}\r\n");
                    actor.Tell(arg.Tweet);
                }
            };
            stream.StartStream();
        }

        public static async Task<decimal> GetWeatherAsync(ICoordinates coordinates)
        {
            var httpClient = new HttpClient();
            var requestUrl = $"http://api.met.no/weatherapi/locationforecast/1.9/?lat={coordinates.Latitude};lon={coordinates.Latitude}";
            var result = await httpClient.GetStringAsync(requestUrl);
            var doc = XDocument.Parse(result);
            var temp = doc.Root.Descendants("temperature").First().Attribute("value").Value;
            return decimal.Parse(temp);
        }

        public static string FormatTweet(ITweet tweet)
        {
            var builder = new StringBuilder();
            builder.AppendLine("---------------------------------------------------------");
            builder.AppendLine($"Tweet from {tweet.CreatedBy} at {tweet.Coordinates?.Latitude},{tweet.Coordinates?.Longitude}");
            builder.AppendLine(tweet.Text);
            return builder.ToString();
        }

        public static string FormatUser(IUser user)
        {
            return user.ToString();
        }

        public static string FormatCoordinates(ICoordinates coordinates)
        {
            return $"------------------------------------{coordinates?.Latitude},{coordinates?.Longitude}";
        }

        public static string FormatTemperature(decimal temperature)
        {
            return $"------------------------------------{temperature}° Celcius";
        }
    }
}
