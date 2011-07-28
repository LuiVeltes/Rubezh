﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            AddManyCommand = new RelayCommand(OnAddMany, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
            ShowIndicatorLogicCommand = new RelayCommand(OnShowIndicatorLogic);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
            PropertiesViewModel = new DeviceProperties.PropertiesViewModel(device);
        }
        public Device Device { get; private set; }

        public PropertiesViewModel PropertiesViewModel { get; private set; }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public string Id
        {
            get { return Device.Id; }
        }

        public Driver Driver
        {
            get { return Device.Driver; }
        }

        public string Address
        {
            get { return Device.Driver.HasAddress ? Device.PresentationAddress : ""; }
            set
            {
                Device.SetAddress(value);
                OnPropertyChanged("Address");
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");
            }
        }

        public IEnumerable<Zone> Zones
        {
            get
            {
                return from Zone zone in FiresecManager.Configuration.Zones
                       orderby Convert.ToInt32(zone.No)
                       select zone;
            }
        }

        public Zone Zone
        {
            get
            {
                return FiresecManager.Configuration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
            }
            set
            {
                Device.ZoneNo = value.No;
                OnPropertyChanged("Zone");
            }
        }

        public string ConnectedTo
        {
            get { return Device.ConnectedTo; }
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.Initialize(Device);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
            if (result)
            {
            }
        }

        public RelayCommand ShowIndicatorLogicCommand { get; private set; }
        void OnShowIndicatorLogic()
        {
            IndicatorDetailsViewModel indicatorDetailsViewModel = new IndicatorDetailsViewModel();
            indicatorDetailsViewModel.Initialize(Device);
            ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel);
        }

        public bool CanAdd(object obj)
        {
            return Driver.CanAddChildren;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (CanAdd(null))
            {
                NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel(this);
                ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
            }
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            if (CanAdd(null))
            {
                NewDeviceRangeViewModel newDeviceRangeViewModel = new NewDeviceRangeViewModel(this);
                ServiceFactory.UserDialogs.ShowModalWindow(newDeviceRangeViewModel);
            }
        }

        bool CanRemove(object obj)
        {
            if (Parent == null)
                return false;

            if (Driver.IsAutoCreate)
                return false;

            return true;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (CanRemove(null))
            {
                Parent.IsExpanded = false;
                Parent.Device.Children.Remove(Device);
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;

                FiresecManager.Configuration.Update();
            }
        }

        bool CanShowProperties(object obj)
        {
            switch (Device.Driver.DriverName)
            {
                case "Индикатор":
                case "Задвижка":
                case "Насос":
                case "Жокей-насос":
                case "Компрессор":
                case "Насос компенсации утечек":
                case "Группа":
                    return true;
            }
            return false;
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            switch (Device.Driver.DriverName)
            {
                case "Индикатор":
                    OnShowIndicatorLogic();
                    break;

                case "Задвижка":
                    ValveDetailsViewModel valveDetailsViewModel = new ValveDetailsViewModel(Device);
                    ServiceFactory.UserDialogs.ShowModalWindow(valveDetailsViewModel);
                    break;

                case "Насос":
                case "Жокей-насос":
                case "Компрессор":
                case "Насос компенсации утечек":
                    PumpDetailsViewModel pumpDetailsViewModel = new PumpDetailsViewModel(Device);
                    ServiceFactory.UserDialogs.ShowModalWindow(pumpDetailsViewModel);
                    break;

                case "Группа":
                    GroupDetailsViewModel groupDetailsViewModel = new GroupDetailsViewModel(Device);
                    ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel);
                    break;
            }
        }
    }
}
