using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaTechTalk
{
    public class CacheActor0 : ReceiveActor
    {
        public CacheActor0()
        {
            Receive<GetValueMsg>(GetCachedValueOrProduceIt);
        }

        void GetCachedValueOrProduceIt(GetValueMsg msg)
        {
            if (dict.ContainsKey(msg.Key))
            {
                var value = dict[msg.Key];
                Sender.Tell(new ValueReadyMsg(msg.Key, value));
            }
            else
            {
                var value = ObtainTheValue(msg.Key);
                dict.Add(msg.Key, value);
                Sender.Tell(new ValueReadyMsg(msg.Key, value));
            }
        }

        private object ObtainTheValue(string key)
        {
            Task.Delay(1000).Wait();
            return $"Value for {key} key";
        }
        
        private Dictionary<string, object> dict = new Dictionary<string, object>();
    }
}