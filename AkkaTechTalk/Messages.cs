namespace AkkaTechTalk
{
    public class ValueCachedSucessfullyMsg
    {
    }

    public class ServeAllValuesMsg
    {
    }

    public class ServeOnlyCachedValuesMsg
    {
    }

    public class ValueMissingMsg
    {
        public ValueMissingMsg(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    public class GetValueMsg
    {
        public GetValueMsg(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    public class ValueReadyMsg
    {
        public ValueReadyMsg(string key, object value)
        {
            Value = value;
            Key = key;
        }

        public string Key { get; }
        public object Value { get; }
    }
}