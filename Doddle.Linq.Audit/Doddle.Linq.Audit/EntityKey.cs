using System;

namespace Doddle.Linq.Audit
{
    public class EntityKey
    {
        public EntityKey(object key)
        {
            Key = key;
            KeyType = key.GetType();
        }

        public object Key { get; private set; }
        public Type KeyType { get; private set; }
    }
}