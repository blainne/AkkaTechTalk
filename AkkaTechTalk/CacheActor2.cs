﻿using System.Collections.Generic;
using Akka.Actor;

namespace AkkaTechTalk
{
    public class CacheActor2 : ReceiveActor
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();
        private Dictionary<string, List<IActorRef>> subscribers = new Dictionary<string, List<IActorRef>>();

        public CacheActor2()
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
                    ? Context.ActorOf<ValueProviderActor0>(name) 
                    : Context.Child(name);
        }
    }
}