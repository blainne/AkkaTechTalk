﻿using System;
 using System.Threading.Tasks;
using Akka.Actor;
using AkkaTechTalk;

namespace AkkaTechTalk
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var system = ActorSystem.Create("TestSystem");
            
            var cache0 = system.ActorOf<CacheActor0>("cache0");
            var console = system.ActorOf<ConsoleActor>();
            Console.WriteLine(DateTime.Now.ToString("hh.mm.ss.ffffff"));
            cache0.Tell(new GetValueMsg("1"), console);
            cache0.Tell(new GetValueMsg("2"), console);
            cache0.Tell(new GetValueMsg("1"), console);
            
            var cache1 = system.ActorOf<CacheActor1>("cache1");
            cache1.Tell(new GetValueMsg("1"), console);
            cache1.Tell(new GetValueMsg("2"), console);
            
            var cache2 = system.ActorOf<CacheActor2>("cache2");
            cache2.Tell(new GetValueMsg("1"), console);
            cache2.Tell(new GetValueMsg("2"), console);
            cache2.Tell(new GetValueMsg("1"), console);
            cache2.Tell(new GetValueMsg("2"), console);
            
            var cache3 = system.ActorOf<CacheActor3>("cache3");
            cache3.Tell(new GetValueMsg("1"), console);
            cache3.Tell(new GetValueMsg("2"), console);
            cache3.Tell(new GetValueMsg("1"), console);
            cache3.Tell(new GetValueMsg("2"), console);
            
            await Task.Delay(5000);
            
            Console.WriteLine("Now trying behavior change");
            cache3.Tell(new ServeOnlyCachedValuesMsg());
            cache3.Tell(new GetValueMsg("2"), console);
            cache3.Tell(new GetValueMsg("3"), console);
            
            cache3.Tell(new ServeAllValuesMsg());
            cache3.Tell(new GetValueMsg("2"), console);
            cache3.Tell(new GetValueMsg("3"), console);
            await Task.Delay(5000);
        }
    }

    public class ConsoleActor : ReceiveActor
    {
        public ConsoleActor()
        {
            Receive<ValueReadyMsg>(msg =>
            {
                Console.WriteLine($"[{DateTime.Now:hh.mm.ss.ffffff} received {msg.Value} from {Sender.Path}]");
            });
            Receive<ValueMissingMsg>(msg =>
            {
                Console.WriteLine($"[{DateTime.Now:hh.mm.ss.ffffff} received value missing from {Sender.Path}]");
            });
        }
    }
}