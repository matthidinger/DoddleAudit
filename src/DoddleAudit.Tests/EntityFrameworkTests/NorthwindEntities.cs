using System;

namespace Doddle.Linq.Audit.Tests.EntityFrameworkTests
{
    //public partial class NorthwindEntities
    //{
    //    partial void OnContextCreated()
    //    {
    //        PropertyAuditRules.Add((m, e) => !m.Name.EndsWith("Id"));
    //    }

    //    protected override void InsertAuditRecordToDatabase(AuditedEntity record)
    //    {
    //        AuditRecord audit = new AuditRecord();
    //        audit.Action = (byte)record.Action;
    //        audit.AuditDate = DateTime.Now;
    //        audit.AssociationTable = record.AssociationTable;
    //        audit.AssociationTableKey = (int?)record.AssociationTableKey.Key;
    //        audit.EntityTable = record.EntityTable;
    //        audit.EntityTableKey = (int)record.EntityTableKey.Key;
    //        audit.UserName = "Matt";
    //        foreach (AuditedEntityField av in record.ModifiedFields)
    //        {
    //            AuditRecordField field = new AuditRecordField();
    //            field.MemberName = av.FieldName;
    //            field.OldValue = av.OldValue;
    //            field.NewValue = av.NewValue;
    //            audit.AuditRecordFields.Add(field);
    //        }
    //        this.AddToAuditRecordSet(audit);

    //    }
    //}
}