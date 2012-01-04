﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoddleAudit.Tests.LinqToSql
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="DoddleAuditECommerce")]
	public partial class ECommerceDataContext : DoddleAudit.LinqToSql.AuditableDataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertCategory(Category instance);
    partial void UpdateCategory(Category instance);
    partial void DeleteCategory(Category instance);
    partial void InsertProduct(Product instance);
    partial void UpdateProduct(Product instance);
    partial void DeleteProduct(Product instance);
    partial void InsertPromotion(Promotion instance);
    partial void UpdatePromotion(Promotion instance);
    partial void DeletePromotion(Promotion instance);
    partial void InsertAuditRecordProperty(AuditRecordProperty instance);
    partial void UpdateAuditRecordProperty(AuditRecordProperty instance);
    partial void DeleteAuditRecordProperty(AuditRecordProperty instance);
    partial void InsertAuditRecord(AuditRecord instance);
    partial void UpdateAuditRecord(AuditRecord instance);
    partial void DeleteAuditRecord(AuditRecord instance);
    #endregion
		
		public ECommerceDataContext() : 
				base(global::DoddleAudit.Tests.Properties.Settings.Default.DoddleAuditECommerceConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public ECommerceDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ECommerceDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ECommerceDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ECommerceDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Category> Categories
		{
			get
			{
				return this.GetTable<Category>();
			}
		}
		
		public System.Data.Linq.Table<Product> Products
		{
			get
			{
				return this.GetTable<Product>();
			}
		}
		
		public System.Data.Linq.Table<Promotion> Promotions
		{
			get
			{
				return this.GetTable<Promotion>();
			}
		}
		
		public System.Data.Linq.Table<AuditRecordProperty> AuditRecordProperties
		{
			get
			{
				return this.GetTable<AuditRecordProperty>();
			}
		}
		
		public System.Data.Linq.Table<AuditRecord> AuditRecords
		{
			get
			{
				return this.GetTable<AuditRecord>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Categories")]
	public partial class Category : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _CategoryName;
		
		private EntitySet<Product> _Products;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCategoryNameChanging(string value);
    partial void OnCategoryNameChanged();
    #endregion
		
		public Category()
		{
			this._Products = new EntitySet<Product>(new Action<Product>(this.attach_Products), new Action<Product>(this.detach_Products));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryName", DbType="NVarChar(MAX)")]
		public string CategoryName
		{
			get
			{
				return this._CategoryName;
			}
			set
			{
				if ((this._CategoryName != value))
				{
					this.OnCategoryNameChanging(value);
					this.SendPropertyChanging();
					this._CategoryName = value;
					this.SendPropertyChanged("CategoryName");
					this.OnCategoryNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Product", Storage="_Products", ThisKey="Id", OtherKey="CategoryId")]
		public EntitySet<Product> Products
		{
			get
			{
				return this._Products;
			}
			set
			{
				this._Products.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Products(Product entity)
		{
			this.SendPropertyChanging();
			entity.Category = this;
		}
		
		private void detach_Products(Product entity)
		{
			this.SendPropertyChanging();
			entity.Category = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Products")]
	public partial class Product : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _ProductName;
		
		private int _CategoryId;
		
		private EntitySet<Promotion> _Promotions;
		
		private EntityRef<Category> _Category;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnProductNameChanging(string value);
    partial void OnProductNameChanged();
    partial void OnCategoryIdChanging(int value);
    partial void OnCategoryIdChanged();
    #endregion
		
		public Product()
		{
			this._Promotions = new EntitySet<Promotion>(new Action<Promotion>(this.attach_Promotions), new Action<Promotion>(this.detach_Promotions));
			this._Category = default(EntityRef<Category>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductName", DbType="NVarChar(MAX)")]
		public string ProductName
		{
			get
			{
				return this._ProductName;
			}
			set
			{
				if ((this._ProductName != value))
				{
					this.OnProductNameChanging(value);
					this.SendPropertyChanging();
					this._ProductName = value;
					this.SendPropertyChanged("ProductName");
					this.OnProductNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryId", DbType="Int NOT NULL")]
		public int CategoryId
		{
			get
			{
				return this._CategoryId;
			}
			set
			{
				if ((this._CategoryId != value))
				{
					if (this._Category.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._CategoryId = value;
					this.SendPropertyChanged("CategoryId");
					this.OnCategoryIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Product_Promotion", Storage="_Promotions", ThisKey="Id", OtherKey="ProductId")]
		public EntitySet<Promotion> Promotions
		{
			get
			{
				return this._Promotions;
			}
			set
			{
				this._Promotions.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Product", Storage="_Category", ThisKey="CategoryId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public Category Category
		{
			get
			{
				return this._Category.Entity;
			}
			set
			{
				Category previousValue = this._Category.Entity;
				if (((previousValue != value) 
							|| (this._Category.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Category.Entity = null;
						previousValue.Products.Remove(this);
					}
					this._Category.Entity = value;
					if ((value != null))
					{
						value.Products.Add(this);
						this._CategoryId = value.Id;
					}
					else
					{
						this._CategoryId = default(int);
					}
					this.SendPropertyChanged("Category");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Promotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.Product = this;
		}
		
		private void detach_Promotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.Product = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Promotions")]
	public partial class Promotion : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private decimal _DiscountAmount;
		
		private int _ProductId;
		
		private EntityRef<Product> _Product;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnDiscountAmountChanging(decimal value);
    partial void OnDiscountAmountChanged();
    partial void OnProductIdChanging(int value);
    partial void OnProductIdChanged();
    #endregion
		
		public Promotion()
		{
			this._Product = default(EntityRef<Product>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DiscountAmount", DbType="Decimal(18,2) NOT NULL")]
		public decimal DiscountAmount
		{
			get
			{
				return this._DiscountAmount;
			}
			set
			{
				if ((this._DiscountAmount != value))
				{
					this.OnDiscountAmountChanging(value);
					this.SendPropertyChanging();
					this._DiscountAmount = value;
					this.SendPropertyChanged("DiscountAmount");
					this.OnDiscountAmountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductId", DbType="Int NOT NULL")]
		public int ProductId
		{
			get
			{
				return this._ProductId;
			}
			set
			{
				if ((this._ProductId != value))
				{
					if (this._Product.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnProductIdChanging(value);
					this.SendPropertyChanging();
					this._ProductId = value;
					this.SendPropertyChanged("ProductId");
					this.OnProductIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Product_Promotion", Storage="_Product", ThisKey="ProductId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public Product Product
		{
			get
			{
				return this._Product.Entity;
			}
			set
			{
				Product previousValue = this._Product.Entity;
				if (((previousValue != value) 
							|| (this._Product.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Product.Entity = null;
						previousValue.Promotions.Remove(this);
					}
					this._Product.Entity = value;
					if ((value != null))
					{
						value.Promotions.Add(this);
						this._ProductId = value.Id;
					}
					else
					{
						this._ProductId = default(int);
					}
					this.SendPropertyChanged("Product");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AuditRecordProperties")]
	public partial class AuditRecordProperty : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _AuditRecordId;
		
		private string _PropertyName;
		
		private string _OldValue;
		
		private string _NewValue;
		
		private EntityRef<AuditRecord> _AuditRecord;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnAuditRecordIdChanging(int value);
    partial void OnAuditRecordIdChanged();
    partial void OnPropertyNameChanging(string value);
    partial void OnPropertyNameChanged();
    partial void OnOldValueChanging(string value);
    partial void OnOldValueChanged();
    partial void OnNewValueChanging(string value);
    partial void OnNewValueChanged();
    #endregion
		
		public AuditRecordProperty()
		{
			this._AuditRecord = default(EntityRef<AuditRecord>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuditRecordId", DbType="Int NOT NULL")]
		public int AuditRecordId
		{
			get
			{
				return this._AuditRecordId;
			}
			set
			{
				if ((this._AuditRecordId != value))
				{
					if (this._AuditRecord.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnAuditRecordIdChanging(value);
					this.SendPropertyChanging();
					this._AuditRecordId = value;
					this.SendPropertyChanged("AuditRecordId");
					this.OnAuditRecordIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PropertyName", DbType="NVarChar(MAX)")]
		public string PropertyName
		{
			get
			{
				return this._PropertyName;
			}
			set
			{
				if ((this._PropertyName != value))
				{
					this.OnPropertyNameChanging(value);
					this.SendPropertyChanging();
					this._PropertyName = value;
					this.SendPropertyChanged("PropertyName");
					this.OnPropertyNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OldValue", DbType="NVarChar(MAX)")]
		public string OldValue
		{
			get
			{
				return this._OldValue;
			}
			set
			{
				if ((this._OldValue != value))
				{
					this.OnOldValueChanging(value);
					this.SendPropertyChanging();
					this._OldValue = value;
					this.SendPropertyChanged("OldValue");
					this.OnOldValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NewValue", DbType="NVarChar(MAX)")]
		public string NewValue
		{
			get
			{
				return this._NewValue;
			}
			set
			{
				if ((this._NewValue != value))
				{
					this.OnNewValueChanging(value);
					this.SendPropertyChanging();
					this._NewValue = value;
					this.SendPropertyChanged("NewValue");
					this.OnNewValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="AuditRecord_AuditRecordProperty", Storage="_AuditRecord", ThisKey="AuditRecordId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public AuditRecord AuditRecord
		{
			get
			{
				return this._AuditRecord.Entity;
			}
			set
			{
				AuditRecord previousValue = this._AuditRecord.Entity;
				if (((previousValue != value) 
							|| (this._AuditRecord.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._AuditRecord.Entity = null;
						previousValue.AuditRecordProperties.Remove(this);
					}
					this._AuditRecord.Entity = value;
					if ((value != null))
					{
						value.AuditRecordProperties.Add(this);
						this._AuditRecordId = value.Id;
					}
					else
					{
						this._AuditRecordId = default(int);
					}
					this.SendPropertyChanged("AuditRecord");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AuditRecords")]
	public partial class AuditRecord : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private byte _Action;
		
		private System.DateTime _AuditDate;
		
		private string _ParentTable;
		
		private int _ParentKey;
		
		private string _Table;
		
		private int _TableKey;
		
		private string _UserName;
		
		private EntitySet<AuditRecordProperty> _AuditRecordProperties;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnActionChanging(byte value);
    partial void OnActionChanged();
    partial void OnAuditDateChanging(System.DateTime value);
    partial void OnAuditDateChanged();
    partial void OnParentTableChanging(string value);
    partial void OnParentTableChanged();
    partial void OnParentKeyChanging(int value);
    partial void OnParentKeyChanged();
    partial void OnTableChanging(string value);
    partial void OnTableChanged();
    partial void OnTableKeyChanging(int value);
    partial void OnTableKeyChanged();
    partial void OnUserNameChanging(string value);
    partial void OnUserNameChanged();
    #endregion
		
		public AuditRecord()
		{
			this._AuditRecordProperties = new EntitySet<AuditRecordProperty>(new Action<AuditRecordProperty>(this.attach_AuditRecordProperties), new Action<AuditRecordProperty>(this.detach_AuditRecordProperties));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Action", DbType="TinyInt NOT NULL")]
		public byte Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				if ((this._Action != value))
				{
					this.OnActionChanging(value);
					this.SendPropertyChanging();
					this._Action = value;
					this.SendPropertyChanged("Action");
					this.OnActionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AuditDate", DbType="DateTime NOT NULL")]
		public System.DateTime AuditDate
		{
			get
			{
				return this._AuditDate;
			}
			set
			{
				if ((this._AuditDate != value))
				{
					this.OnAuditDateChanging(value);
					this.SendPropertyChanging();
					this._AuditDate = value;
					this.SendPropertyChanged("AuditDate");
					this.OnAuditDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentTable", DbType="NVarChar(MAX)")]
		public string ParentTable
		{
			get
			{
				return this._ParentTable;
			}
			set
			{
				if ((this._ParentTable != value))
				{
					this.OnParentTableChanging(value);
					this.SendPropertyChanging();
					this._ParentTable = value;
					this.SendPropertyChanged("ParentTable");
					this.OnParentTableChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentKey", DbType="Int NOT NULL")]
		public int ParentKey
		{
			get
			{
				return this._ParentKey;
			}
			set
			{
				if ((this._ParentKey != value))
				{
					this.OnParentKeyChanging(value);
					this.SendPropertyChanging();
					this._ParentKey = value;
					this.SendPropertyChanged("ParentKey");
					this.OnParentKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Table]", Storage="_Table", DbType="NVarChar(MAX)")]
		public string Table
		{
			get
			{
				return this._Table;
			}
			set
			{
				if ((this._Table != value))
				{
					this.OnTableChanging(value);
					this.SendPropertyChanging();
					this._Table = value;
					this.SendPropertyChanged("Table");
					this.OnTableChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TableKey", DbType="Int NOT NULL")]
		public int TableKey
		{
			get
			{
				return this._TableKey;
			}
			set
			{
				if ((this._TableKey != value))
				{
					this.OnTableKeyChanging(value);
					this.SendPropertyChanging();
					this._TableKey = value;
					this.SendPropertyChanged("TableKey");
					this.OnTableKeyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserName", DbType="NVarChar(MAX)")]
		public string UserName
		{
			get
			{
				return this._UserName;
			}
			set
			{
				if ((this._UserName != value))
				{
					this.OnUserNameChanging(value);
					this.SendPropertyChanging();
					this._UserName = value;
					this.SendPropertyChanged("UserName");
					this.OnUserNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="AuditRecord_AuditRecordProperty", Storage="_AuditRecordProperties", ThisKey="Id", OtherKey="AuditRecordId")]
		public EntitySet<AuditRecordProperty> AuditRecordProperties
		{
			get
			{
				return this._AuditRecordProperties;
			}
			set
			{
				this._AuditRecordProperties.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_AuditRecordProperties(AuditRecordProperty entity)
		{
			this.SendPropertyChanging();
			entity.AuditRecord = this;
		}
		
		private void detach_AuditRecordProperties(AuditRecordProperty entity)
		{
			this.SendPropertyChanging();
			entity.AuditRecord = null;
		}
	}
}
#pragma warning restore 1591
