## How to Define your Audits
There are a few different ways to enlist an entity for auditing, with varying degrees of flexibility.

#### Use your DataContext table properties
This is the best way to create audit definitions, and should handle 95% of use cases. This logic relies on your DataContext having properties for each of your tables. It will use attribute reflection to look up the primary keys of each table.
```
dataContext.Products.Audit();
dataContext.Categories.Audit();
dataContext.Orders.Audit();
```

Alternatively, you can explicitly define your primary key fields if you do not want to rely on reflection, or if you have any problems with the implicit method.
```
dataContext.Products.Audit(p => p.ProductID);
dataContext.Categories.Audit(c => c.CategoryID);
dataContext.Orders.Audit(o => o.OrderID);
```

#### Specify the Entity Type manually
Use of this method will be rare, but it will come in handy if your DataContext does not have properties to access your tables.
```
dataContext.Audit<Product>();
dataContext.Audit<Category>();
dataContext.Audit<Order>();
```

You can also use this method to define the primary keys.
```
dataContext.Audit<Product>(p => p.ProductID);
dataContext.Audit<Category>(c => c.CategoryID);
dataContext.Audit<Order>(o => o.OrderID);
```


## How to Audit Associations
You will probably want to audit some "children" relationships eventually. This will come in handy if you want to Audit the Orders table, but you also want to audit all OrderDetails that are related to the Order.

#### Use the Association Property of your Entity
This is the best way to audit associations, but it assumes that you have an IEnumerable property on your Entity to access the association.
```
dataContext.Orders.Audit()
       .AuditAssociation(o => o.OrderDetails);


dataContext.Contacts.Audit()
       .AuditAssociation(c => c.Addresses)
       .AuditAssociation(c => c.PhoneNumbers);
```

#### Explicity Map the PK/FK Properties
This method will come in handy if you do not have an association property on your entity to rely on
```
dataContext.Orders.Audit()
       .AuditAssociation<OrderDetail>(od => od.OrderDetailID, od => od.OrderID);


dataContext.Contacts.Audit()
       .AuditAssociation<Address>(a => a.AddressID, a => a.ContactID)
       .AuditAssociation<PhoneNumber>(p => p.PhoneNumberID, p => p.ContactID);
```
