﻿using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaTechTalk
{
public class ValueProviderActor : ReceiveActor
{
    public ValueProviderActor()
    {
        Receive<GetValueMsg>(ObtainValueAndSendBack);
    }

    private void ObtainValueAndSendBack(GetValueMsg msg)
    {
        var value = ObtainTheValue(msg.Key);
        Sender.Tell(new ValueReadyMsg(msg.Key, value));
        Context.Stop(Self);
    }

    private static string ObtainTheValue(string msgKey)
    {
        Task.Delay(1000).Wait();
        return $"Value for {msgKey} key";
    }
}
}