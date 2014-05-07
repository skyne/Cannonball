using NotifyPropertyChanged;
using PropertyValueWatcher;
using System;

namespace Cannonball.SphereTest
{
    [NotifyPropertyChanged]
    class GameObject : IObjectKey
    {
        public object ObjectKey
        {
            get;
            set;
        }

        public int Counter { get; set; }

        public GameObject()
        {
            ObjectKey = Guid.NewGuid();
        }
    }
}
