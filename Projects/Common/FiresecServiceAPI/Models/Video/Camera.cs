using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Entities.DeviceOriented;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera : IStateProvider, IDeviceState<XStateClass>, IIdentity, IPlanPresentable
	{
		public Camera()
		{
			UID = Guid.NewGuid();
			Children = new List<Camera>();
			ZoneUIDs = new List<Guid>();
			PlanElementUIDs = new List<Guid>();
			Width = 300;
			Height = 300;
			AllowMultipleVizualization = false;
			Status = DeviceStatuses.Disconnected;

			Name = "Новая камера";
			Port = 37777;
			Login = "admin";
			Password = "admin";
		}

		[XmlIgnore]
		public Camera Parent { get; set; }

		[XmlIgnore]
		public string ImageSource
		{
			get { return this.CameraType.ToString(); }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Ip { get; set; }

		[XmlIgnore]
		public DeviceStatuses Status { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public int ChannelNumber { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public List<Camera> Children { get; set; }

		[DataMember]
		public int Left { get; set; }

		[DataMember]
		public int Top { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public bool IgnoreMoveResize { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public CameraType CameraType { get; set; }

		[XmlIgnore]
		public XStateClass CameraStateStateClass
		{
			get { return XStateClass.Norm; }
		}

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		[XmlIgnore]
		public string PresentationName
		{
			get { return Name + " " + Ip; }
		}

		public event Action Changed;

		#region IStateProvider Members

		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return this; }
		}

		#endregion

		#region IDeviceState<XStateClass> Members

		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return StateClass; }
		}

		event Action IDeviceState<XStateClass>.StateChanged
		{
			add { }
			remove { }
		}

		#endregion
	}
}