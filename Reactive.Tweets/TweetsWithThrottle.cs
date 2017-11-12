using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Streams;
using Akka.Streams.Dsl;
using Tweetinvi.Models;

namespace Reactive.Tweets
{
    public static class TweetsWithThrottle
    {
        public static IRunnableGraph<NotUsed> CreateRunnableGraph()
        {
            var tweetSource = Source.FromEnumerator(() => new TweetEnumerator());
            var formatUser = Flow.Create<IUser>()
                .Select(Utils.FormatUser);
            var formatCoordinates = Flow.Create<ICoordinates>()
                .Select(Utils.FormatCoordinates);
            var writeSink = Sink.ForEach<string>(Console.WriteLine);

            var graph = GraphDsl.Create(b =>
            {
                var broadcast = b.Add(new Broadcast<ITweet>(2));
                var merge = b.Add(new Merge<string>(2));
                b.From(broadcast.Out(0))
                    .Via(Flow.Create<ITweet>().Select(tweet => tweet.CreatedBy)
                        .Throttle(10, TimeSpan.FromSeconds(1), 1, ThrottleMode.Shaping))
                    .Via(formatUser)
                    .To(merge.In(0));
                b.From(broadcast.Out(1))
                    .Via(Flow.Create<ITweet>().Select(tweet => tweet.Coordinates)
                        .Buffer(10, OverflowStrategy.DropNew)
                        .Throttle(1, TimeSpan.FromSeconds(1), 10, ThrottleMode.Shaping))
                    .Via(formatCoordinates)
                    .To(merge.In(1));

                return new FlowShape<ITweet, string>(broadcast.In, merge.Out);
            });

            return tweetSource.Via(graph).To(writeSink);
        }
    }
}
