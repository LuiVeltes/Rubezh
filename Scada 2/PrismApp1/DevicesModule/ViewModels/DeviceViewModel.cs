﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using ServiceApi;
using System.Collections.ObjectModel;
using ClientApi;
using DevicesModule.Views;
using System.Diagnostics;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public Device Device;
        public Firesec.Metadata.drvType Driver;

        public ObservableCollection<DeviceViewModel> SourceDevices { get; set; }

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            ShowPlanCommand = new RelayCommand(OnShowPlan);
            ShowZoneCommand = new RelayCommand(OnShowZone);
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                DateTime start = DateTime.Now;

                isExpanded = value;

                if (isExpanded)
                {
                    ExpandChildren(this);
                }
                else
                {
                    HideChildren(this);
                }

                OnPropertyChanged("IsExpanded");
                DateTime end = DateTime.Now;
                TimeSpan interval = end.Subtract(start);
                Trace.WriteLine(interval.Milliseconds);
            }
        }

        void HideChildren(DeviceViewModel parentDeviceViewModel)
        {
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                if (SourceDevices.Contains(deviceViewModel))
                    SourceDevices.Remove(deviceViewModel);
                HideChildren(deviceViewModel);
            }
        }

        void ExpandChildren(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.IsExpanded)
            {
                int indexOf = SourceDevices.IndexOf(parentDeviceViewModel);
                for (int i = 0; i < parentDeviceViewModel.Children.Count; i++)
                {
                    if (SourceDevices.Contains(parentDeviceViewModel.Children[i]) == false)
                    {
                        SourceDevices.Insert(indexOf + 1 + i, parentDeviceViewModel.Children[i]);
                    }
                }

                foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChildren(deviceViewModel);
                }
            }
        }

        public bool HasChildren
        {
            get
            {
                return (Children.Count > 0);
            }
        }

        public int Level
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }
                else
                {
                    return Parent.Level + 1;
                }
            }
        }

        public void Update()
        {
            OnPropertyChanged("HasChildren");
        }

        public string DriverId
        {
            get
            {
                if (Device != null)
                    return Device.DriverId;
                return null;
            }
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            SourceDevices = sourceDevices;

            this.Device = device;
            Driver = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
        }

        public bool IsZoneDevice
        {
            get
            {
                if ((Driver.minZoneCardinality == "0") && (Driver.maxZoneCardinality == "0"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                if ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")))
                {
                    return true;
                }
                return false;
            }
        }

        public string PresentationZone
        {
            get
            {
                if (string.IsNullOrEmpty(Device.ZoneNo))
                    return "";

                Zone zone = ServiceClient.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                return Device.ZoneNo + "." + zone.Name;
            }
        }

        public string ShortDriverName
        {
            get
            {
                return Driver.shortName;
            }
        }

        public string DriverName
        {
            get
            {
                return Driver.name;
            }
        }

        public bool HasAddress
        {
            get
            {
                return (!string.IsNullOrEmpty(Address));
            }
        }

        public string Address
        {
            get
            {
                return Device.Address;
            }
        }

        public string Description
        {
            get
            {
                return Device.Description;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (Parent == null)
                    return null;
                else
                {
                    string parentPart = Parent.ShortDriverName;
                    if (Parent.Driver.ar_no_addr != "1")
                        parentPart += " - " + Parent.Address;

                    if (Parent.ConnectedTo == null)
                        return parentPart;

                    if (Parent.Parent.ConnectedTo == null)
                        return parentPart;

                    return parentPart + @"\" + Parent.ConnectedTo;
                }
            }
        }

        public bool HasImage
        {
            get
            {
                return (ImageSource != @"C:/Program Files/Firesec/Icons/Device_Device.ico");
            }
        }

        public string ImageSource
        {
            get
            {
                string ImageName;
                if (!string.IsNullOrEmpty(Driver.dev_icon))
                {
                    ImageName = Driver.dev_icon;
                }
                else
                {
                    Firesec.Metadata.classType metadataClass = ServiceClient.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == Driver.clsid);
                    ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
                }

                return @"C:/Program Files/Firesec/Icons/" + ImageName + ".ico";
                //return @"pack://application:,,,/Icons/" + ImageName + ".ico";
            }
        }

        public DeviceViewModel Parent { get; set; }

        ObservableCollection<DeviceViewModel> children;
        public ObservableCollection<DeviceViewModel> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                DevicesViewModel.Current.SelectedDeviceViewModel = this;
            }
        }

        public ObservableCollection<string> SelfStates
        {
            get
            {
                ObservableCollection<string> selfStates = new ObservableCollection<string>();
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Device.Path);
                if (deviceState.SelfStates != null)
                    foreach (string selfState in deviceState.SelfStates)
                    {
                        selfStates.Add(selfState);
                    }
                return selfStates;
            }
        }

        public ObservableCollection<string> ParentStates
        {
            get
            {
                ObservableCollection<string> parentStates = new ObservableCollection<string>();
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Device.Path);
                if (deviceState.ParentStringStates != null)
                    foreach (string parentState in deviceState.ParentStringStates)
                    {
                        parentStates.Add(parentState);
                    }
                return parentStates;
            }
        }

        public ObservableCollection<string> Parameters
        {
            get
            {
                ObservableCollection<string> parameters = new ObservableCollection<string>();
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Device.Path);
                if (deviceState.Parameters != null)
                    foreach (Parameter parameter in deviceState.Parameters)
                    {
                        if (string.IsNullOrEmpty(parameter.Value))
                            continue;
                        if (parameter.Value == "<NULL>")
                            continue;
                        parameters.Add(parameter.Caption + " - " + parameter.Value);
                    }
                return parameters;
            }
        }

        public string State
        {
            get
            {
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Device.Path);
                return deviceState.State;
            }
        }

        public RelayCommand ShowPlanCommand { get; private set; }
        void OnShowPlan()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            string zoneNo = Device.ZoneNo;
            if (string.IsNullOrEmpty(zoneNo) == false)
            {
                ServiceFactory.Events.GetEvent<ShowZonesEvent>().Publish(zoneNo);
            }
        }
    }
}
