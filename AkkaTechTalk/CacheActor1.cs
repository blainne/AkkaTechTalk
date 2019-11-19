using System.Collections.Generic;
using Akka.Actor;

namespace AkkaTechTalk
{
    public class CacheActor1 : ReceiveActor
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();
        private Dictionary<string, IActorRef> subscribers = new Dictionary<string, IActorRef>();

        public CacheActor1()
        {
            Receive<GetValueMsg>(GetCachedValueOrProduceIt);
            Receive<ValueReadyMsg>(AddToCacheAndRespond);
        }

        private void GetCachedValueOrProduceIt(GetValueMsg msg)
        {
            if (dict.ContainsKey(msg.Key))
            {
                var value = dict[msg.Key];
                Sender.Tell(new ValueReadyMsg(msg.Key, value));
            }
            else
            {
                var valueProvider = CreateValueProviderActor();
                valueProvider.Tell(msg);
                subscribers.Add(msg.Key, Sender);
            }
        }
        
        private void AddToCacheAndRespond(ValueReadyMsg msg)
        {
            dict.Add(msg.Key, msg.Value);
            subscribers[msg.Key].Tell(msg);
            subscribers.Remove(msg.Key);
        }

        private static IActorRef CreateValueProviderActor() 
            => Context.ActorOf<ValueProviderActor>();
    }
}