using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaTechTalk
{
    public class ValueProviderActor1 : ReceiveActor
    {
        public ValueProviderActor1()
        {
            Receive<GetValueMsg>(ObtainValueAndSendBack);
        }

        private void Finished()
        {
            Receive<GetValueMsg>(DoNothing);
            Receive<ValueCachedSucessfullyMsg>(_ => Context.Stop(Self));
        }


        private void ObtainValueAndSendBack(GetValueMsg msg)
        {
            var value = ObtainTheValue(msg.Key);
            Sender.Tell(new ValueReadyMsg(msg.Key, value));
            Become(Finished);
        }
    
        private static void DoNothing(GetValueMsg _){}

        private static string ObtainTheValue(string msgKey)
        {
            Task.Delay(1000).Wait();
            return $"Value for {msgKey} key";
        }
    
    

    }
}