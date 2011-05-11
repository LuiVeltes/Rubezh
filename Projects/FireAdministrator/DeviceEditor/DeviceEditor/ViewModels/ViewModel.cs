﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common;
using DeviceLibrary;
using Frame = DeviceLibrary.Models.Frame;

namespace DeviceEditor.ViewModels
{
    public class ViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceViewModel> _deviceViewModels;
        private DeviceViewModel _selectedDeviceViewModel;
        private StateViewModel _selectedStateViewModel;
        public ViewModel()
        {
            Current = this;
            Load();
            SaveCommand = new RelayCommand(OnSave);
        }
        public static ViewModel Current { get; private set; }
        /// <summary>
        /// Комманда сохранения текущей конфигурации в файл.
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }
        /// <summary>
        /// Список всех устройств
        /// </summary>
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return _deviceViewModels; }
            set
            {
                _deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }
        /// <summary>
        /// Выбранное устройство.
        /// </summary>
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return _selectedDeviceViewModel; }
            set
            {
                _selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }
        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                _selectedStateViewModel = value;
                SelectedStateViewModel.SelectedFrameViewModel = _selectedStateViewModel.FrameViewModels[0];
                SelectedStateViewModel.ParentDevice.DeviceControl.DriverId = SelectedStateViewModel.ParentDevice.Id;
                SelectedStateViewModel.ParentDevice.DeviceControl.IsAdditional = SelectedStateViewModel.IsAdditional;
                SelectedStateViewModel.ParentDevice.DeviceControl.StateId = SelectedStateViewModel.Id;
                SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStatesIds = SelectedStateViewModel.IsAdditional ? null : SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }
        public void OnSave(object obj)
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                                      "Окно подтверждения", MessageBoxButton.OKCancel,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
                return;
            LibraryManager.Devices = new List<Device>();
            foreach (var deviceViewModel in DeviceViewModels)
            {
                var device = new
                Device
                {
                    Id = deviceViewModel.Id
                };
                LibraryManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (var stateViewModel in deviceViewModel.StatesViewModel)
                {
                    var state = new State {Id = stateViewModel.Id, IsAdditional = stateViewModel.IsAdditional};
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (var frame in stateViewModel.FrameViewModels.Select(frameViewModel => new
                    Frame
                    {
                        Id = frameViewModel.Id,
                        Image = frameViewModel.Image,
                        Duration = frameViewModel.Duration, 
                        Layer = frameViewModel.Layer
                    }
                    ))
                    {
                        state.Frames.Add(frame);
                    }
                }
            }
            LibraryManager.Save();
        }
        public void Load()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                var deviceViewModel = new
                DeviceViewModel
                {
                    Id = device.Id,
                    StatesViewModel = new ObservableCollection<StateViewModel>()
                };
                DeviceViewModels.Add(deviceViewModel);
                try
                {
                    deviceViewModel.IconPath = Helper.IconsPath + LibraryManager.Drivers.FirstOrDefault(x => x.id == deviceViewModel.Id).dev_icon + ".ico";
                }
                catch { }
                foreach (var state in device.States)
                {
                    var stateViewModel = new StateViewModel { IsAdditional = state.IsAdditional, Id = state.Id };
                    deviceViewModel.StatesViewModel.Add(stateViewModel);
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (var frameViewModel in state.Frames.Select(frame => new
                    FrameViewModel
                    {
                        Id = frame.Id,
                        Image = frame.Image,
                        Duration = frame.Duration,
                        Layer = frame.Layer
                    }))
                    {
                        stateViewModel.FrameViewModels.Add(frameViewModel);
                    }
                }
            }
        }
    }
}