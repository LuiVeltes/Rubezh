﻿using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceControls;
using DeviceLibrary;
using Firesec;
using Infrastructure;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            DeviceControl = new DeviceControl();
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            ShowStatesCommand = new RelayCommand(OnShowStates);
            AdditionalStates = new List<string>();
            States = new ObservableCollection<StateViewModel>();
        }

        public static DeviceViewModel Current { get; private set; }

        public void Initialize()
        {
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == this.Id);
            this.States = new ObservableCollection<StateViewModel>();
            for (var stateId = 0; stateId < 9; stateId++)
            {
                if (stateId < 7)
                    if (driver.state.FirstOrDefault(x => x.@class == Convert.ToString(stateId)) == null) continue;
                var stateViewModel = new StateViewModel();
                stateViewModel.Id = Convert.ToString(stateId);
                var frameViewModel = new FrameViewModel();
                frameViewModel.Duration = 300;
                frameViewModel.Image = Helper.EmptyFrame;
                stateViewModel.Frames = new ObservableCollection<FrameViewModel>();
                stateViewModel.Frames.Add(frameViewModel);
                this.States.Add(stateViewModel);
                LibraryViewModel.Current.Update();
            }
        }

        private DeviceControl _deviceControl;
        public DeviceControl DeviceControl
        {
            get { return _deviceControl; }
            set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }

        public string IconPath
        {
            get
            {
                try
                {
                    return Helper.DevicesIconsPath + LibraryManager.Drivers.FirstOrDefault(x => x.id == Id).dev_icon + ".ico";
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Name = _name = DriversHelper.GetDriverNameById(_id);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                if (value == null)
                    return;
                _selectedState = value;
                LibraryViewModel.Current.SelectedState = _selectedState;
                OnPropertyChanged("SelectedState");
            }
        }

        private ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public List<string> AdditionalStates;

        public RelayCommand ShowDevicesCommand { get; private set; }
        private static void OnShowDevices()
        {
            var devicesListViewModel = new DevicesListViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(devicesListViewModel);
        }

        public RelayCommand ShowStatesCommand { get; private set; }
        public static void OnShowStates()
        {
            var statesListViewModel = new StatesListViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(statesListViewModel);
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        private void OnRemoveDevice()
        {
            LibraryViewModel.Current.Devices.Remove(this);
            LibraryViewModel.Current.SelectedState = null;
            LibraryViewModel.Current.Update();
        }
    }
}
