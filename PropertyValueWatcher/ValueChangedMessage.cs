
namespace PropertyValueWatcher
{
    public class ValueChangedMessage
    {
        private object _sender;
        private object _key;
        private string _propertyName;
        private object _value;

        public object Sender
        {
            get
            {
                return _sender;
            }
        }

        public object Key
        {
            get
            {
                return _key;
            }
        }
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        public object NewValue
        {
            get
            {
                return _value;
            }
        }

        public ValueChangedMessage(object sender, object key, string propertyName, object newValue)
        {
            _sender = sender;
            _key = key;
            _propertyName = propertyName;
            _value = newValue;
        }
    }
}
