using System.Collections.Generic;
using Akka.Actor;

namespace AkkaTechTalk
{
    public class CacheActor2 : ReceiveActor
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();
        private Dictionary<string, List<IActorRef>> subscribers = new Dictionary<string, List<IActorRef>>();

        public CacheActor2()
        {
            Become(Caching);
        }

        private void Caching()
        {
            Receive<GetValueMsg>(GetCachedValueOrProduceIt);
            Receive<ValueReadyMsg>(AddToCacheAndRespond);
            Receive<ServeOnlyCachedValuesMsg>(_ => Become(CalculationStopped));
        }

        private void CalculationStopped()
        {
            Receive<GetValueMsg>(GetCachedValueOrSendError);
            Receive<ValueReadyMsg>(AddToCacheAndRespond);
            Receive<ServeAllValuesMsg>(_ => Become(Caching));
        }

        private void GetCachedValueOrSendError(GetValueMsg msg)
        {
            if (dict.ContainsKey(msg.Key))
            {
                var value = dict[msg.Key];
                Sender.Tell(new ValueReadyMsg(msg.Key, value));
            }
            else
                Sender.Tell(new ValueMissingMsg(msg.Key));
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
                var valueProvider = GetOrCreateValueProviderByName($"ValueProviderFor_{msg.Key}") ;
                valueProvider.Tell(msg);
                
                if(!subscribers.ContainsKey(msg.Key))
                    subscribers.Add(msg.Key, new List<IActorRef>());
                subscribers[msg.Key].Add(Sender);
            }
        }

        private void AddToCacheAndRespond(ValueReadyMsg msg)
        {
            dict.Add(msg.Key, msg.Value);
            subscribers[msg.Key].ForEach(sub => sub.Tell(msg));
            subscribers.Remove(msg.Key);
        }
        
        private IActorRef GetOrCreateValueProviderByName(string name)
        {
            return 
                Context.Child(name).IsNobody() 
                    ? Context.ActorOf<ValueProviderActor>(name) 
                    : Context.Child(name);
        }
    }
}