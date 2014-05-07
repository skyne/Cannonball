using DFPluginAPI;
using DFPluginAPI.Channel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PropertyValueWatcher
{
    public class PropertyValueChangeDispatcher : IObjectKey
    {
        public Action<ChannelMessage> NewMessage;
        public object ObjectKey
        {
            get;
            private set;
        }

        private IChannel _channel;
        private List<IObjectKey> _watchedObjects;


        public PropertyValueChangeDispatcher(IPluginManager manager)
        {
            ObjectKey = Guid.NewGuid();
            _channel = manager.RequestChannel(Constants.Channel.Name);

            _channel.Recieved += _channel_Recieved;

            _watchedObjects = new List<IObjectKey>();
        }

        void _channel_Recieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Message.MessageType == typeof(ValueChangedMessage))
            {
                var valChangedMessage = (ValueChangedMessage)e.Message.Message;
                if (valChangedMessage.Sender != this.ObjectKey)
                {
                    var target = _watchedObjects.FirstOrDefault(o => o.ObjectKey == valChangedMessage.Key);

                    if (target != null) //ha nem is figyeljük ezt az objektumot, akkor nem nyúlhatunk bele
                        target.SetPropertyValueByPath(valChangedMessage.PropertyName, valChangedMessage.NewValue);
                }
            }
        }

        public void Register(object instance)
        {
            var type = instance.GetType();
            if (type.HasInterface(typeof(IObjectKey)))
            {
                _watchedObjects.Add((IObjectKey)instance);
                ((INotifyPropertyChanged)instance).PropertyChanged += (o, p) => _channel.Send<ValueChangedMessage>(
                         new ValueChangedMessage(
                                 this.ObjectKey,
                                 ((IObjectKey)instance).ObjectKey,
                                 p.PropertyName,
                                 instance.GetPropertyValueByPath(p.PropertyName)
                             )
                         );
            }
        }
    }
}
