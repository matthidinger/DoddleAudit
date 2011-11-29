using System.Collections.Generic;

namespace Doddle.Audit
{
    public class RelationshipChangeEntry
    {
        public RelationshipChangeEntry()
        {
            Entities = new List<RelationshipChangeEntity>();
        }
        public List<RelationshipChangeEntity> Entities { get; set; }
    }

    public class RelationshipChangeEntity
    {
        public object KeyValue { get; set; }

        public string TableName { get; set; }
        public string KeyName { get; set; }
    }
}