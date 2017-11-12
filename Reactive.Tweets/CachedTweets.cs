using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Tweetinvi.Models;

namespace Reactive.Tweets
{
    public static class CachedTweets
    {
        public static IRunnableGraph<NotUsed> CreateRunnableGraph()
        {
            var tweetSource = Source.FromEnumerator(() => new TweetEnumerator());
            var formatFlow = Flow.Create<ITweet>().Select(Utils.FormatTweet);
            var writeSink = Sink.ForEach<string>(Console.WriteLine);
            return tweetSource.Via(formatFlow).To(writeSink);
        }
    }
}
