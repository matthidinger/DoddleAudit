//using System;
//using System.Collections.Generic;
//using System.Data.Metadata.Edm;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Data.Objects;
//using Doddle.Linq.Audit;

//namespace Doddle.Linq.Audit.Entity
//{
//    public class AuditableObjectContext : ObjectStateManager, IAuditableContext
//    {
//        public AuditableObjectContext(MetadataWorkspace workspace) : base(workspace)
//        {
            
//        }
//        public void Audit()
//        {
//            ObjectContext t;

//            var added = this.GetObjectStateEntries(System.Data.EntityState.Added);
//            foreach (ObjectStateEntry entry in added)
//            {

//                AuditRecord record = new AuditRecord();
                
//                foreach(string propName in entry.GetModifiedProperties())
//                {
//                    object propValue = entry.CurrentValues[propName];

//                    AuditValue values = new AuditValue();
//                    values.MemberName = propName;
//                    values.OldValue = entry.OriginalValues[propName].ToString();
//                    values.NewValue = entry.CurrentValues[propName].ToString();
//                }
//                //entry.CurrentValues
//            }


//        }

//        public IList<IAuditDefinition> AuditDefinitions
//        {
//            get { throw new System.NotImplementedException(); }
//        }

//        public IEnumerable<object> Inserts
//        {
//            get { throw new System.NotImplementedException(); }
//        }

//        public IEnumerable<object> Updates
//        {
//            get { throw new System.NotImplementedException(); }
//        }

//        public IEnumerable<object> Deletes
//        {
//            get { throw new System.NotImplementedException(); }
//        }

//        public void QueueAudit(AuditRecord record)
//        {
//            throw new System.NotImplementedException();
//        }

//        public IEnumerable<MemberAudit> GetModifiedMembers(object entity)
//        {
//            throw new System.NotImplementedException();
//        }

//        public LambdaExpression EntityPkSelector
//        {
//            get { throw new System.NotImplementedException(); }
//        }
//    }
//}
