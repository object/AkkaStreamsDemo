using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Tweetinvi;
using Tweetinvi.Models;

namespace Reactive.Tweets
{
    public class LiveTweets
    {
        public static IRunnableGraph<IActorRef> CreateRunnableGraph()
        {
            var tweetSource = Source.ActorRef<ITweet>(100, OverflowStrategy.DropHead);
            var formatFlow = Flow.Create<ITweet>().Select(Utils.FormatTweet);
            var writeSink = Sink.ForEach<string>(Console.WriteLine);
            return tweetSource.Via(formatFlow).To(writeSink);
        }
    }
}
