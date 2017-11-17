# DoddleAudit
This project will add automatic Auditing to your LINQ to SQL or Entity Framework (EF support is not done yet) application.

### Objective
Automatic auditing of all inserts/updates/deletes for any table in your database with a single line of code,  including:
* What table was modified? 
* What fields changed? 
* Who made the change? 
* When did it occur? 

Naturally, there are many ways to tackle this problem. In the past I have either relied on writing database Triggers for the tables, or wrote the Auditing logic right into the stored procedure that was doing the modification. But this time, since the project is utilizing LINQ to SQL, I had a more ambitious idea in mind. 

### Usage

Simply define your audit definitions at any time before calling SubmitChanges();

```csharp
dataContext.Products.Audit();
dataContext.Categories.Audit();
dataContext.Orders.Audit().AuditAssociation(o => o.Order_Details);
dataContext.Contacts.Audit().AuditAssociation(c => c.Addresses).AuditAssociation(c => c.PhoneNumbers);
```

For full usage declarations please see: [Define Audits](Define-Audits.md)

### Limitations
* **Any tables you want to audit must have one (and only one) Primary Key field, which is of type `int`**
* Depending on demand for this feature I will begin exploring ways around this, including GUID primary keys and composite keys.

### New Features and Fixes in V2
* A significantly refined API for defining which tables to audit, including automatic primary key lookups.
* Inserted records will be queried to retrieve their populated primary key value to store in the audit table.
* Entity inheritance is fully supported and automatically audited (E.g., LINQ to SQL inheritance)
* Built-in support for auditing across relationships
* Ability to define custom “audit property resolvers” to override the default auditing mechanism for properties that you specify.

```csharp
public class ProductAuditResolver : AuditPropertyResolver<Product>
{
    protected override void CustomizeProperties()
    {
        CustomizeProperty(p => p.CategoryID, categoryId => GetCategoryByID(cid).CategoryName, "Category");
    }
}
```

### Instructions
* Download the latest release, and add a reference to **Doddle.Linq.Audit.dll** in your project
* Add a **using Doddle.Linq.Audit;** to any classes where you want to define audits
* Create the tables in your database where audited records will be stored. You are free to use any schema/table structure you wish to store the audits.
	* Below you will see the table structure I chose. To get started, feel free to create the 2 tables below in your database (and add to your DBML).
	* _NOTE: Keep in mind this database schema is entirely customizable. I chose to use two tables to store all of my audits, but you could very easily change this logic to use a separate table for each entity or whatever storage schema fits your needs._ 
![screenshot](http://blog2.matthidinger.com/ContentImages/LINQAuditTrailv2_B491/image_thumb_3.png)

* Open your DBML and click in the designer surface. In the property pane you will need to change the **Base Class** property of your generated DataContext to be Doddle.Linq.Audit.LinqToSql.AuditableDataContext
![screenshot](http://blog2.matthidinger.com/ContentImages/LINQAuditTrailv2_B491/image_thumb_4.png)      

* Lastly you will need to create a partial DataContext class to wire up the auditing infrastructure to match your database schema. Add a new Class file to your project and insert the following code. Customize if necessary to match your auditing schema. 

```csharp
public partial class NorthwindEntitiesDataContext
{
    /// <summary>
    /// The audit code will call this method and pass the populated EntityAuditRecord.
    /// This is where you wire up the EntityAuditRecord to your physical auditing tables.
    /// Customize as necessary.
    /// </summary>
    /// <param name="record">A populated audit record</param>
    protected override void InsertAuditRecordToDatabase(EntityAuditRecord record)
    {
        AuditRecord audit = new AuditRecord();
        audit.Action = (byte)record.Action;
        audit.AuditDate = DateTime.Now;
        audit.AssociationTable = record.AssociationTable;
        audit.AssociationTableKey = record.AssociationTableKey;
        audit.EntityTable = record.EntityTable;
        audit.EntityTableKey = record.EntityTableKey;

        audit.UserName = HttpContext.Current.User.Identity.Name;

        foreach (ModifiedEntityProperty av in record.ModifiedProperties)
        {
            AuditRecordField field = new AuditRecordField();
            field.MemberName = av.MemberName;
            field.OldValue = av.OldValue;
            field.NewValue = av.NewValue;

            audit.AuditRecordFields.Add(field);
        }

        this.AuditRecords.InsertOnSubmit(audit);
    }

    /// <summary>
    /// Define your audit definitions here. 
    /// This is not mandatory:
    /// you can define audit definitions anywhere in code as long as it is before SubmitChanges()
    /// </summary>
    protected override void DefaultAuditDefinitions()
    {
        this.Products.Audit();
        this.Categories.Audit().AuditAssociation(c => c.Products);
        this.Orders.Audit().AuditAssociation(o => o.Order_Details);
    }
}
```
