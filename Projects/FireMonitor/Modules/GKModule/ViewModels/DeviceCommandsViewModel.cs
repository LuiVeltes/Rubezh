﻿using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);

			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommands)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
		}

		void SendControlCommand(XStateType stateType)
		{
			if (Device.Driver.IsDeviceOnShleif)
			{
				var code = 0x80 + (int)stateType;
				ObjectCommandSendHelper.SendControlCommand(Device, (byte)code);
			}
		}

        public bool IsTriStateControl
        {
            get { return Device.Driver.IsDeviceOnShleif && Device.Driver.IsControlDevice; }
        }

        public bool IsBiStateControl
        {
            get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice; }
        }

        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (DeviceState.States.Contains(XStateType.Ignore))
                    return DeviceControlRegime.Ignore;

                if (DeviceState.States.Contains(XStateType.Norm))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }

		public bool IsControlRegime
		{
            get { return ControlRegime == DeviceControlRegime.Manual; }
		}

        public RelayCommand SetAutomaticStateCommand { get; private set; }
        void OnSetAutomaticState()
        {
			SendControlCommand(XStateType.SetRegime_Automatic);
        }

        public RelayCommand SetManualStateCommand { get; private set; }
        void OnSetManualState()
        {
			SendControlCommand(XStateType.SetRegime_Manual);
        }

        public RelayCommand SetIgnoreStateCommand { get; private set; }
        void OnSetIgnoreState()
        {
			SendControlCommand(XStateType.SetRegime_Off);
        }

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}
	}
}