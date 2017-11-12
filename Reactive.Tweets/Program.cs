using System;
using System.Collections.Generic;
using System.Configuration;
using Akka.Actor;
using Akka.Streams;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace Reactive.Tweets
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var sys = ActorSystem.Create("Reactive-Tweets"))
            {
                var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
                var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                var accessToken = ConfigurationManager.AppSettings["AccessToken"];
                var accessTokenSecret = ConfigurationManager.AppSettings["AccessTokenSecret"];
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                var useCachedTweets = true;

                using (var mat = sys.Materializer())
                {
                    Auth.SetCredentials(new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret));

                    if (useCachedTweets)
                    {
                        var graph = TweetsWithWeather.CreateRunnableGraph();
                        graph.Run(mat);
                    }
                    else
                    {
                        var graph = LiveTweets.CreateRunnableGraph();
                        var actor = graph.Run(mat);
                        Utils.StartTweetStream(actor);
                    }

                    Console.ReadLine();
                }
            }
        }
    }
}
