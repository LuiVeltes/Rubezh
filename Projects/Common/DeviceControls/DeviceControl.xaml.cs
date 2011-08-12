﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeviceLibrary;

namespace DeviceControls
{
    public partial class DeviceControl : INotifyPropertyChanged
    {
        public DeviceControl()
        {
            InitializeComponent();

            DataContext = this;
            StateCanvases = new ObservableCollection<Canvas>();
            AdditionalStates = new List<string>();
        }

        public string DriverId { get; set; }

        string _stateId;
        public string StateId
        {
            get { return _stateId; }
            set
            {
                _stateId = value;
                Update();
            }
        }

        List<string> _additionalStates;
        public List<string> AdditionalStates
        {
            get { return _additionalStates; }
            set
            {
                _additionalStates = value;
                Update();
            }
        }

        ObservableCollection<Canvas> _stateCanvases;
        public ObservableCollection<Canvas> StateCanvases
        {
            get { return _stateCanvases; }
            set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }

        List<StateViewModel> _stateViewModelList;

        void Update()
        {
            if (_stateViewModelList != null)
                _stateViewModelList.ForEach(x => x.Dispose());
            _stateViewModelList = new List<StateViewModel>();

            var device = LibraryManager.Devices.FirstOrDefault(x => x.Id == DriverId);
            if (device == null)
                return;

            StateCanvases = new ObservableCollection<Canvas>();
            var state = device.States.FirstOrDefault(x => (x.Class == StateId) && (int.Parse(x.Class) >= 0));
            if (state != null)
            {
                _stateViewModelList.Add(new StateViewModel(state, StateCanvases));
                if (AdditionalStates == null) return;
                foreach (var additionalStateId in AdditionalStates)
                {
                    var aState = device.States.FirstOrDefault(x => (x.Class == additionalStateId));
                    if (aState == null) continue;
                    _stateViewModelList.Add(new StateViewModel(aState, StateCanvases));
                }
            }
            else
                foreach (var additionalStateId in AdditionalStates)
                {
                    var additionalState = device.States.FirstOrDefault(x => (x.Class == additionalStateId));
                    if (additionalState != null)
                    {
                        _stateViewModelList.Add(new StateViewModel(additionalState, StateCanvases));
                    }
                }
        }

        void UserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _itemsControl.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}