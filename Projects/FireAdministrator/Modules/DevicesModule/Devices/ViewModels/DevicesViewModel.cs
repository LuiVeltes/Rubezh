﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DevicesModule.DeviceProperties;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
    {
        public static DevicesViewModel Current { get; private set; }
        public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
        public PropertiesViewModel PropMenu { get; private set; }
        public DevicesViewModel()
        {
            Current = this;
            CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
            CutCommand = new RelayCommand(OnCut, CanCutCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
            PasteAsCommand = new RelayCommand(OnPasteAs, CanPasteAs);
            DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
            Menu = new DevicesMenuViewModel(this);
            PropMenu = new PropertiesViewModel(this);
            RegisterShortcuts();
        }

        public void Initialize()
        {
            BuildTree();
            if (RootDevice != null)
            {
                RootDevice.IsExpanded = true;
                SelectedDevice = RootDevice;
            }
            OnPropertyChanged("RootDevices");
            UpdateGuardVisibility();
        }

        #region DeviceSelection
        public List<DeviceViewModel> AllDevices;

        public void FillAllDevices()
        {
            AllDevices = new List<DeviceViewModel>();
            AddChildPlainDevices(RootDevice);
        }

        void AddChildPlainDevices(DeviceViewModel parentViewModel)
        {
            AllDevices.Add(parentViewModel);
            foreach (var childViewModel in parentViewModel.Children)
                AddChildPlainDevices(childViewModel);
        }

        public void Select(Guid deviceUID)
        {
            if (deviceUID != Guid.Empty)
            {
                var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
                if (deviceViewModel != null)
                    deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }
        #endregion

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        DeviceViewModel _rootDevice;
        public DeviceViewModel RootDevice
        {
            get { return _rootDevice; }
            private set
            {
                _rootDevice = value;
                OnPropertyChanged("RootDevice");
            }
        }

        public DeviceViewModel[] RootDevices
        {
            get { return new DeviceViewModel[] { RootDevice }; }
        }

        void BuildTree()
        {
            RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
            FillAllDevices();
        }

        public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
            FillAllDevices();
            return deviceViewModel;
        }
        private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            if (parentDeviceViewModel != null)
                parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
                AddDeviceInternal(childDevice, deviceViewModel);
            return deviceViewModel;
        }

        Device _deviceToCopy;
        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            _deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, false);
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            _deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, true);
            SelectedDevice.RemoveCommand.Execute();

            FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
            ServiceFactory.SaveService.FSChanged = true;
            UpdateGuardVisibility();
        }
        bool CanCutCopy()
        {
            return !(SelectedDevice == null || SelectedDevice.Parent == null ||
                SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.AutoChild == SelectedDevice.Driver.UID
                || SelectedDevice.Driver.DriverType == DriverType.MPT);
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, false);
            PasteDevice(pasteDevice);
        }
        bool CanPaste()
        {
            return (SelectedDevice != null && _deviceToCopy != null && SelectedDevice.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.UID) && !SelectedDevice.Driver.IsChildAddressReservedRange);
        }

        public RelayCommand PasteAsCommand { get; private set; }
        void OnPasteAs()
        {
            var pasteAsViewModel = new PasteAsViewModel(SelectedDevice.Driver.DriverType);
            if (DialogService.ShowModalWindow(pasteAsViewModel))
            {
                var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, false);

                pasteDevice.DriverUID = pasteAsViewModel.SelectedDriver.UID;
                pasteDevice.Driver = pasteAsViewModel.SelectedDriver;

                FiresecManager.FiresecConfiguration.SynchronizeChildern(pasteDevice);

                PasteDevice(pasteDevice);
            }
        }
        bool CanPasteAs()
        {
            return (SelectedDevice != null && _deviceToCopy != null &&
                (DriversHelper.PanelDrivers.Contains(_deviceToCopy.Driver.DriverType) || DriversHelper.UsbPanelDrivers.Contains(_deviceToCopy.Driver.DriverType)) &&
                (SelectedDevice.Driver.DriverType == DriverType.Computer || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_1 || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_2));
        }

        void PasteDevice(Device device)
        {
            SelectedDevice.Device.Children.Add(device);
            device.Parent = SelectedDevice.Device;

            var newDevice = AddDeviceInternal(device, SelectedDevice);
            newDevice.CollapseChildren();

            FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
            ServiceFactory.SaveService.FSChanged = true;
            UpdateGuardVisibility();
            FillAllDevices();
        }

        public static void UpdateGuardVisibility()
        {
            var hasSecurityDevice = FiresecManager.Devices.Any(x => x.Driver.DeviceType == DeviceType.Sequrity);
            ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Publish(hasSecurityDevice);
        }

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.X, ModifierKeys.Control), CutCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
                {
                    if (SelectedDevice != null)
                    {
                        if (SelectedDevice.AddCommand.CanExecute(null))
                            SelectedDevice.AddCommand.Execute();
                    }
                });
            RegisterShortcut(new KeyGesture(KeyboardKey.M, ModifierKeys.Control), () =>
            {
                if (SelectedDevice != null)
                {
                    if (SelectedDevice.AddToParentCommand.CanExecute(null))
                        SelectedDevice.AddToParentCommand.Execute();
                }
            });
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
                {
                    if (SelectedDevice != null)
                    {
                        if (SelectedDevice.RemoveCommand.CanExecute(null))
                            SelectedDevice.RemoveCommand.Execute();
                    }
                });
            RegisterShortcut(new KeyGesture(KeyboardKey.Right, ModifierKeys.Control), () =>
                {
                    if (SelectedDevice != null)
                    {
                        if (SelectedDevice.HasChildren && !SelectedDevice.IsExpanded)
                            SelectedDevice.IsExpanded = true;
                    }
                });
            RegisterShortcut(new KeyGesture(KeyboardKey.Left, ModifierKeys.Control), () =>
                {
                    if (SelectedDevice != null)
                    {
                        if (SelectedDevice.HasChildren && SelectedDevice.IsExpanded)
                            SelectedDevice.IsExpanded = false;
                    }
                });
        }
    }
}