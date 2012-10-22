﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiagnosticsModule
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
	
	
	public partial class GkJournalDatabase : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertGkEvents(GkEvents instance);
    partial void UpdateGkEvents(GkEvents instance);
    partial void DeleteGkEvents(GkEvents instance);
    partial void InsertJournal(Journal instance);
    partial void UpdateJournal(Journal instance);
    partial void DeleteJournal(Journal instance);
    #endregion
		
		public GkJournalDatabase(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GkJournalDatabase(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GkJournalDatabase(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GkJournalDatabase(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<GkEvents> GkEvents
		{
			get
			{
				return this.GetTable<GkEvents>();
			}
		}
		
		public System.Data.Linq.Table<Journal> Journal
		{
			get
			{
				return this.GetTable<Journal>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="gkEvents")]
	public partial class GkEvents : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Nullable<int> _GKNo;
		
		private System.Nullable<System.DateTime> _Date;
		
		private string _EventName;
		
		private string _EventDescription;
		
		private System.Nullable<int> _KAUNo;
		
		private System.Nullable<int> _GKObjectNo;
		
		private System.Nullable<int> _KAUAddress;
		
		private System.Nullable<int> _Code;
		
		private System.Nullable<int> _ObjectNo;
		
		private System.Nullable<int> _ObjectDeviceType;
		
		private System.Nullable<int> _ObjectDeviceAddress;
		
		private System.Nullable<int> _ObjectFactoryNo;
		
		private System.Nullable<int> _ObjectState;
		
		private int _Id;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnGKNoChanging(System.Nullable<int> value);
    partial void OnGKNoChanged();
    partial void OnDateChanging(System.Nullable<System.DateTime> value);
    partial void OnDateChanged();
    partial void OnEventNameChanging(string value);
    partial void OnEventNameChanged();
    partial void OnEventDescriptionChanging(string value);
    partial void OnEventDescriptionChanged();
    partial void OnKAUNoChanging(System.Nullable<int> value);
    partial void OnKAUNoChanged();
    partial void OnGKObjectNoChanging(System.Nullable<int> value);
    partial void OnGKObjectNoChanged();
    partial void OnKAUAddressChanging(System.Nullable<int> value);
    partial void OnKAUAddressChanged();
    partial void OnCodeChanging(System.Nullable<int> value);
    partial void OnCodeChanged();
    partial void OnObjectNoChanging(System.Nullable<int> value);
    partial void OnObjectNoChanged();
    partial void OnObjectDeviceTypeChanging(System.Nullable<int> value);
    partial void OnObjectDeviceTypeChanged();
    partial void OnObjectDeviceAddressChanging(System.Nullable<int> value);
    partial void OnObjectDeviceAddressChanged();
    partial void OnObjectFactoryNoChanging(System.Nullable<int> value);
    partial void OnObjectFactoryNoChanged();
    partial void OnObjectStateChanging(System.Nullable<int> value);
    partial void OnObjectStateChanged();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    #endregion
		
		public GkEvents()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GKNo", DbType="Int")]
		public System.Nullable<int> GKNo
		{
			get
			{
				return this._GKNo;
			}
			set
			{
				if ((this._GKNo != value))
				{
					this.OnGKNoChanging(value);
					this.SendPropertyChanging();
					this._GKNo = value;
					this.SendPropertyChanged("GKNo");
					this.OnGKNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Date", DbType="DateTime")]
		public System.Nullable<System.DateTime> Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				if ((this._Date != value))
				{
					this.OnDateChanging(value);
					this.SendPropertyChanging();
					this._Date = value;
					this.SendPropertyChanged("Date");
					this.OnDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EventName", DbType="NVarChar(100)")]
		public string EventName
		{
			get
			{
				return this._EventName;
			}
			set
			{
				if ((this._EventName != value))
				{
					this.OnEventNameChanging(value);
					this.SendPropertyChanging();
					this._EventName = value;
					this.SendPropertyChanged("EventName");
					this.OnEventNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EventDescription", DbType="NVarChar(100)")]
		public string EventDescription
		{
			get
			{
				return this._EventDescription;
			}
			set
			{
				if ((this._EventDescription != value))
				{
					this.OnEventDescriptionChanging(value);
					this.SendPropertyChanging();
					this._EventDescription = value;
					this.SendPropertyChanged("EventDescription");
					this.OnEventDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KAUNo", DbType="Int")]
		public System.Nullable<int> KAUNo
		{
			get
			{
				return this._KAUNo;
			}
			set
			{
				if ((this._KAUNo != value))
				{
					this.OnKAUNoChanging(value);
					this.SendPropertyChanging();
					this._KAUNo = value;
					this.SendPropertyChanged("KAUNo");
					this.OnKAUNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GKObjectNo", DbType="Int")]
		public System.Nullable<int> GKObjectNo
		{
			get
			{
				return this._GKObjectNo;
			}
			set
			{
				if ((this._GKObjectNo != value))
				{
					this.OnGKObjectNoChanging(value);
					this.SendPropertyChanging();
					this._GKObjectNo = value;
					this.SendPropertyChanged("GKObjectNo");
					this.OnGKObjectNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KAUAddress", DbType="Int")]
		public System.Nullable<int> KAUAddress
		{
			get
			{
				return this._KAUAddress;
			}
			set
			{
				if ((this._KAUAddress != value))
				{
					this.OnKAUAddressChanging(value);
					this.SendPropertyChanging();
					this._KAUAddress = value;
					this.SendPropertyChanged("KAUAddress");
					this.OnKAUAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Code", DbType="Int")]
		public System.Nullable<int> Code
		{
			get
			{
				return this._Code;
			}
			set
			{
				if ((this._Code != value))
				{
					this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Code = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectNo", DbType="Int")]
		public System.Nullable<int> ObjectNo
		{
			get
			{
				return this._ObjectNo;
			}
			set
			{
				if ((this._ObjectNo != value))
				{
					this.OnObjectNoChanging(value);
					this.SendPropertyChanging();
					this._ObjectNo = value;
					this.SendPropertyChanged("ObjectNo");
					this.OnObjectNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectDeviceType", DbType="Int")]
		public System.Nullable<int> ObjectDeviceType
		{
			get
			{
				return this._ObjectDeviceType;
			}
			set
			{
				if ((this._ObjectDeviceType != value))
				{
					this.OnObjectDeviceTypeChanging(value);
					this.SendPropertyChanging();
					this._ObjectDeviceType = value;
					this.SendPropertyChanged("ObjectDeviceType");
					this.OnObjectDeviceTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectDeviceAddress", DbType="Int")]
		public System.Nullable<int> ObjectDeviceAddress
		{
			get
			{
				return this._ObjectDeviceAddress;
			}
			set
			{
				if ((this._ObjectDeviceAddress != value))
				{
					this.OnObjectDeviceAddressChanging(value);
					this.SendPropertyChanging();
					this._ObjectDeviceAddress = value;
					this.SendPropertyChanged("ObjectDeviceAddress");
					this.OnObjectDeviceAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectFactoryNo", DbType="Int")]
		public System.Nullable<int> ObjectFactoryNo
		{
			get
			{
				return this._ObjectFactoryNo;
			}
			set
			{
				if ((this._ObjectFactoryNo != value))
				{
					this.OnObjectFactoryNoChanging(value);
					this.SendPropertyChanging();
					this._ObjectFactoryNo = value;
					this.SendPropertyChanged("ObjectFactoryNo");
					this.OnObjectFactoryNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectState", DbType="Int")]
		public System.Nullable<int> ObjectState
		{
			get
			{
				return this._ObjectState;
			}
			set
			{
				if ((this._ObjectState != value))
				{
					this.OnObjectStateChanging(value);
					this.SendPropertyChanging();
					this._ObjectState = value;
					this.SendPropertyChanged("ObjectState");
					this.OnObjectStateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
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
	
	[global::System.Data.Linq.Mapping.TableAttribute()]
	public partial class Journal : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Description;
		
		private System.Nullable<System.DateTime> _DateTime;
		
		private System.Nullable<System.Guid> _ObjectUID;
		
		private System.Nullable<int> _GKObjectNo;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnDateTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnDateTimeChanged();
    partial void OnObjectUIDChanging(System.Nullable<System.Guid> value);
    partial void OnObjectUIDChanged();
    partial void OnGKObjectNoChanging(System.Nullable<int> value);
    partial void OnGKObjectNoChanged();
    #endregion
		
		public Journal()
		{
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(100)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> DateTime
		{
			get
			{
				return this._DateTime;
			}
			set
			{
				if ((this._DateTime != value))
				{
					this.OnDateTimeChanging(value);
					this.SendPropertyChanging();
					this._DateTime = value;
					this.SendPropertyChanged("DateTime");
					this.OnDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectUID", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> ObjectUID
		{
			get
			{
				return this._ObjectUID;
			}
			set
			{
				if ((this._ObjectUID != value))
				{
					this.OnObjectUIDChanging(value);
					this.SendPropertyChanging();
					this._ObjectUID = value;
					this.SendPropertyChanged("ObjectUID");
					this.OnObjectUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GKObjectNo", DbType="Int")]
		public System.Nullable<int> GKObjectNo
		{
			get
			{
				return this._GKObjectNo;
			}
			set
			{
				if ((this._GKObjectNo != value))
				{
					this.OnGKObjectNoChanging(value);
					this.SendPropertyChanging();
					this._GKObjectNo = value;
					this.SendPropertyChanged("GKObjectNo");
					this.OnGKObjectNoChanged();
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
}
#pragma warning restore 1591