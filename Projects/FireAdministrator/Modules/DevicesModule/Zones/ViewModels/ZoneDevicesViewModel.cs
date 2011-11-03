﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneDevicesViewModel : BaseViewModel
    {
        public ZoneDevicesViewModel()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();

            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
        }

        ulong? _zoneNo;

        public void Initialize(ulong? zoneNo)
        {
            _zoneNo = zoneNo;

            var devices = new HashSet<Device>();
            var availableDevices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (device.ZoneNo == null)
                    {
                        device.AllParents.ForEach(x => { availableDevices.Add(x); });
                        availableDevices.Add(device);
                    }

                    if (device.ZoneNo == zoneNo)
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.Zones.Contains(zoneNo)))
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }
            }

            Devices.Clear();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices.Where(x => x.Device.Parent != null))
            {
                var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            AvailableDevices.Clear();
            foreach (var device in availableDevices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, AvailableDevices);
                deviceViewModel.IsExpanded = true;
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices.Where(x => x.Device.Parent != null))
            {
                var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                device.Parent = parent;
                parent.Children.Add(device);
            }

            if (Devices.Count > 0)
                SelectedDevice = Devices[Devices.Count - 1];

            if (AvailableDevices.Count > 0)
                SelectedAvailableDevice = AvailableDevices[AvailableDevices.Count - 1];
        }

        public void Clear()
        {
            Devices.Clear();
            AvailableDevices.Clear();
            SelectedDevice = null;
            SelectedAvailableDevice = null;
        }

        public void DropDevicesZoneNo()
        {
            foreach (var device in Devices)
            {
                device.Device.ZoneNo = null;
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public ObservableCollection<DeviceViewModel> AvailableDevices { get; private set; }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        public bool CanAdd()
        {
            return SelectedAvailableDevice != null && SelectedAvailableDevice.Device.Driver.IsZoneDevice;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            SelectedAvailableDevice.Device.ZoneNo = _zoneNo;
            Initialize(_zoneNo);
            DevicesModule.HasChanges = true;
        }

        public bool CanRemove()
        {
            return SelectedDevice != null && SelectedDevice.Device.Driver.IsZoneDevice;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            SelectedDevice.Device.ZoneNo = null;
            Initialize(_zoneNo);

            DevicesModule.HasChanges = true;
        }

        public bool CanShowZoneLogic()
        {
            return SelectedDevice != null && SelectedDevice.Device.Driver.IsZoneLogicDevice;
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            var zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.Initialize(SelectedDevice.Device);

            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel))
                DevicesModule.HasChanges = true;
        }
    }
}