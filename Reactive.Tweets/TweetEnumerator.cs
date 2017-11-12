using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi;
using Tweetinvi.Models;

namespace Reactive.Tweets
{
    public class TweetEnumerator : IEnumerator<ITweet>
    {
        private StreamReader _reader;
        private ITweet _tweet;
        public TweetEnumerator()
        {
            Reset();
        }

        public ITweet Current => _tweet;

        object IEnumerator.Current => _tweet;

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool MoveNext()
        {
            var line = _reader.ReadLine();
            if (line != null)
            {
                var json = JObject.Parse(line);
                _tweet = new Tweet(json["TweetDTO"]);
            }
            return line != null;
        }

        public void Reset()
        {
            if (_reader != null)
                _reader.Dispose();
            _reader = new StreamReader(@"..\..\tweets.txt");
        }
    }
}
