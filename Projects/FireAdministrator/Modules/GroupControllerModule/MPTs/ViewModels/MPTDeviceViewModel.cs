﻿using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public partial class MPTDeviceViewModel : BaseViewModel
	{
		public GKMPTDevice MPTDevice { get; private set; }

		public MPTDeviceViewModel(GKMPTDevice mptDevice)
		{
			MPTDevice = mptDevice;
			Device = mptDevice.Device;
			MPTDevicePropertiesViewModel = new MPTDevicePropertiesViewModel(Device, false);
		}

		GKDevice _device;
		public GKDevice Device
		{
			get { return _device; }
			set
			{
				_device = value;
				OnPropertyChanged(() => Device);
				OnPropertyChanged(() => Description);
			}
		}

		MPTDevicePropertiesViewModel _mptDevicePropertiesViewModel;
		public MPTDevicePropertiesViewModel MPTDevicePropertiesViewModel
		{
			get { return _mptDevicePropertiesViewModel; }
			set
			{
				_mptDevicePropertiesViewModel = value;
				OnPropertyChanged(() => MPTDevicePropertiesViewModel);
			}
		}

		public string PresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				return GKManager.GetPresentationZoneOrLogic(Device);
			}
		}

		public GKMPTDeviceType MPTDeviceType
		{
			get { return MPTDevice.MPTDeviceType; }
		}

		public string Description
		{
			get { return Device != null ? Device.Description : null; }
			set
			{
				if (Device == null)
					return;

				Device.Description = value;
				OnPropertyChanged(() => Description);

				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == Device.UID);
				if (deviceViewModel != null)
				{
					deviceViewModel.OnPropertyChanged("Description");
				}
				Device.OnChanged();

				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}