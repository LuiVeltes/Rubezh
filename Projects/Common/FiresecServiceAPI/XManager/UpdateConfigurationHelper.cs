﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Common;
//using XFiresecAPI;

//namespace FiresecClient
//{
//    public static partial class UpdateConfigurationHelper
//    {
//        public static XDeviceConfiguration DeviceConfiguration { get; private set; }

//        public static void Update(XDeviceConfiguration deviceConfiguration)
//        {
//            DeviceConfiguration = deviceConfiguration;
//            if (DeviceConfiguration == null)
//            {
//                DeviceConfiguration = new XDeviceConfiguration();
//            }
//            if (DeviceConfiguration.RootDevice == null)
//            {
//                var systemDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
//                if (systemDriver != null)
//                {
//                    DeviceConfiguration.RootDevice = new XDevice()
//                    {
//                        DriverUID = systemDriver.UID,
//                        Driver = systemDriver
//                    };
//                }
//                else
//                {
//                    Logger.Error("XManager.SetEmptyConfiguration systemDriver = null");
//                }
//            }
//            DeviceConfiguration.ValidateVersion();

//            DeviceConfiguration.Update();
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                device.Driver = XManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
//                if (device.Driver == null)
//                {
//                    //MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
//                }
//            }
//            DeviceConfiguration.Reorder();

//            InitializeProperties();
//            Invalidate();
//            CopyMPTProperties();
//        }

//        static void InitializeProperties()
//        {
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                if (device.Properties == null)
//                    device.Properties = new List<XProperty>();
//                foreach (var property in device.Properties)
//                {
//                    property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
//                }
//                device.Properties.RemoveAll(x => x.DriverProperty == null);

//                foreach (var property in device.DeviceProperties)
//                {
//                    property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
//                }
//                device.DeviceProperties.RemoveAll(x => x.DriverProperty == null);
//                device.InitializeDefaultProperties();
//            }
//        }

//        static void Invalidate()
//        {
//            ClearAllReferences();
//            InitializeDevicesInZone();
//            InitializeLogic();
//            InitializeDirections();
//            InitializePumpStations();
//            InitializeMPTs();
//            InitializeDelays();
//            InitializeGuardUsers();
//            UpdateGKChildrenDescription();
//        }

//        static void ClearAllReferences()
//        {
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                device.Zones = new List<XZone>();
//                device.Directions = new List<XDirection>();
//            }
//            foreach (var zone in DeviceConfiguration.Zones)
//            {
//                zone.Devices = new List<XDevice>();
//                zone.Directions = new List<XDirection>();
//                zone.DevicesInLogic = new List<XDevice>();
//            }
//            foreach (var direction in DeviceConfiguration.Directions)
//            {
//                direction.InputZones = new List<XZone>();
//                direction.InputDevices = new List<XDevice>();
//                direction.OutputDevices = new List<XDevice>();
//            }
//            foreach (var pumpStation in DeviceConfiguration.PumpStations)
//            {
//                pumpStation.ClearClauseDependencies();
//                pumpStation.NSDevices = new List<XDevice>();
//            }
//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                mpt.ClearClauseDependencies();
//                mpt.Devices = new List<XDevice>();
//            }
//            foreach (var delay in DeviceConfiguration.Delays)
//            {
//                delay.ClearClauseDependencies();
//            }
//        }

//        static void InitializeDevicesInZone()
//        {
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                var zoneUIDs = new List<Guid>();
//                if (device.Driver.HasZone)
//                {
//                    foreach (var zoneUID in device.ZoneUIDs)
//                    {
//                        var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
//                        if (zone != null)
//                        {
//                            zoneUIDs.Add(zoneUID);
//                            device.Zones.Add(zone);
//                            zone.Devices.Add(device);
//                        }
//                    }
//                }
//                device.ZoneUIDs = zoneUIDs;
//            }
//        }

//        static void InitializeLogic()
//        {
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                InvalidateOneLogic(device, device.DeviceLogic);
//                if (device.NSLogic != null)
//                    InvalidateOneLogic(device, device.NSLogic);
//            }
//        }

//        public static void InvalidateOneLogic(XDevice device, XDeviceLogic deviceLogic)
//        {
//            InvalidateInputObjectsBaseLogic(device, deviceLogic);
//            foreach (var clause in deviceLogic.Clauses)
//            {
//                foreach (var clauseZone in clause.Zones)
//                {
//                    clauseZone.DevicesInLogic.Add(device);
//                }
//                foreach (var clauseDirection in clause.Directions)
//                {
//                    clauseDirection.OutputDevices.Add(device);
//                    device.Directions.Add(clauseDirection);
//                }
//            }
//            foreach (var clause in device.DeviceLogic.OffClauses)
//            {
//                foreach (var clauseZone in clause.Zones)
//                {
//                    clauseZone.DevicesInLogic.Add(device);
//                }
//                foreach (var clauseDirection in clause.Directions)
//                {
//                    clauseDirection.OutputDevices.Add(device);
//                    device.Directions.Add(clauseDirection);
//                }
//            }
//        }

//        static void InitializeDirections()
//        {
//            foreach (var direction in DeviceConfiguration.Directions)
//            {
//                var directionDevices = new List<XDirectionDevice>();
//                foreach (var directionDevice in direction.DirectionDevices)
//                {
//                    if (directionDevice.DeviceUID != Guid.Empty)
//                    {
//                        var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.BaseUID == directionDevice.DeviceUID);
//                        directionDevice.Device = device;
//                        if (device != null)
//                        {
//                            directionDevices.Add(directionDevice);
//                            direction.InputDevices.Add(device);
//                            device.Directions.Add(direction);
//                        }
//                    }
//                }
//                direction.DirectionDevices = directionDevices;

//                var directionZones = new List<XDirectionZone>();
//                foreach (var directionZone in direction.DirectionZones)
//                {
//                    if (directionZone.ZoneUID != Guid.Empty)
//                    {
//                        var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.BaseUID == directionZone.ZoneUID);
//                        directionZone.Zone = zone;
//                        if (zone != null)
//                        {
//                            directionZones.Add(directionZone);
//                            direction.InputZones.Add(zone);
//                            zone.Directions.Add(direction);
//                        }
//                    }
//                }
//                direction.DirectionZones = directionZones;
//            }
//        }

//        static void InitializePumpStations()
//        {
//            foreach (var pumpStation in DeviceConfiguration.PumpStations)
//            {
//                var nsDeviceUIDs = new List<Guid>();
//                foreach (var nsDeviceUID in pumpStation.NSDeviceUIDs)
//                {
//                    var nsDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.BaseUID == nsDeviceUID);
//                    if (nsDevice != null && (nsDevice.DriverType == XDriverType.FirePump || nsDevice.DriverType == XDriverType.JockeyPump || nsDevice.DriverType == XDriverType.RSR2_Bush))
//                    {
//                        nsDeviceUIDs.Add(nsDevice.BaseUID);
//                        pumpStation.NSDevices.Add(nsDevice);
//                    }
//                }
//                pumpStation.NSDeviceUIDs = nsDeviceUIDs;
//                InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.StartLogic);
//                InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.StopLogic);
//                InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.AutomaticOffLogic);
//            }
//        }

//        static void InitializeMPTs()
//        {
//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                InvalidateInputObjectsBaseLogic(mpt, mpt.StartLogic);

//                var mptDevices = new List<MPTDevice>();
//                foreach (var mptDevice in mpt.MPTDevices)
//                {
//                    var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.BaseUID == mptDevice.DeviceUID);
//                    if (device != null && MPTDevice.GetAvailableMPTDriverTypes(mptDevice.MPTDeviceType).Contains(device.DriverType))
//                    {
//                        mptDevice.Device = device;
//                        mptDevices.Add(mptDevice);
//                        device.IsInMPT = true;
//                        mpt.Devices.Add(device);
//                    }
//                }
//                mpt.MPTDevices = mptDevices;
//            }
//        }

//        static void InitializeDelays()
//        {
//            foreach (var delay in DeviceConfiguration.Delays)
//            {
//                if (delay.DeviceLogic == null)
//                    delay.DeviceLogic = new XDeviceLogic();
//                InvalidateInputObjectsBaseLogic(delay, delay.DeviceLogic);
//            }
//        }

//        public static void InvalidateInputObjectsBaseLogic(XBase xBase, XDeviceLogic deviceLogic)
//        {
//            deviceLogic.Clauses = InvalidateOneInputObjectsBaseLogic(xBase, deviceLogic.Clauses);
//            deviceLogic.OffClauses = InvalidateOneInputObjectsBaseLogic(xBase, deviceLogic.OffClauses);
//        }

//        public static List<XClause> InvalidateOneInputObjectsBaseLogic(XBase xBase, List<XClause> inputClauses)
//        {
//            var clauses = new List<XClause>();
//            foreach (var clause in inputClauses)
//            {
//                clause.Devices = new List<XDevice>();
//                clause.Zones = new List<XZone>();
//                clause.Directions = new List<XDirection>();
//                clause.MPTs = new List<XMPT>();
//                clause.Delays = new List<XDelay>();

//                var deviceUIDs = new List<Guid>();
//                foreach (var deviceUID in clause.DeviceUIDs)
//                {
//                    var clauseDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.BaseUID == deviceUID);
//                    if (clauseDevice != null && !clauseDevice.IsNotUsed)
//                    {
//                        deviceUIDs.Add(deviceUID);
//                        clause.Devices.Add(clauseDevice);
//                        if (!xBase.ClauseInputDevices.Contains(clauseDevice))
//                            xBase.ClauseInputDevices.Add(clauseDevice);
//                    }
//                }
//                clause.DeviceUIDs = deviceUIDs;

//                var zoneUIDs = new List<Guid>();
//                foreach (var zoneUID in clause.ZoneUIDs)
//                {
//                    var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
//                    if (zone != null)
//                    {
//                        zoneUIDs.Add(zoneUID);
//                        clause.Zones.Add(zone);
//                        if (!xBase.ClauseInputZones.Contains(zone))
//                            xBase.ClauseInputZones.Add(zone);
//                    }
//                }
//                clause.ZoneUIDs = zoneUIDs;

//                var directionUIDs = new List<Guid>();
//                foreach (var directionUID in clause.DirectionUIDs)
//                {
//                    var direction = DeviceConfiguration.Directions.FirstOrDefault(x => x.BaseUID == directionUID);
//                    if (direction != null)
//                    {
//                        directionUIDs.Add(directionUID);
//                        clause.Directions.Add(direction);
//                        if (xBase.ClauseInputDirections != null)
//                        if (!xBase.ClauseInputDirections.Contains(direction))
//                            xBase.ClauseInputDirections.Add(direction);
//                    }
//                }
//                clause.DirectionUIDs = directionUIDs;

//                var mptUIDs = new List<Guid>();
//                foreach (var mptUID in clause.MPTUIDs)
//                {
//                    var mpt = DeviceConfiguration.MPTs.FirstOrDefault(x => x.BaseUID == mptUID);
//                    if (mpt != null)
//                    {
//                        mptUIDs.Add(mptUID);
//                        clause.MPTs.Add(mpt);
//                        if (xBase.ClauseInputMPTs != null)
//                        if (!xBase.ClauseInputMPTs.Contains(mpt))
//                            xBase.ClauseInputMPTs.Add(mpt);
//                    }
//                }
//                clause.MPTUIDs = mptUIDs;

//                var delayUIDs = new List<Guid>();
//                foreach (var delayUID in clause.DelayUIDs)
//                {
//                    var delay = DeviceConfiguration.Delays.FirstOrDefault(x => x.BaseUID == delayUID);
//                    if (delay != null)
//                    {
//                        delayUIDs.Add(delayUID);
//                        clause.Delays.Add(delay);
//                        if (xBase.ClauseInputDelays != null)
//                        if (!xBase.ClauseInputDelays.Contains(delay))
//                            xBase.ClauseInputDelays.Add(delay);
//                    }
//                }
//                clause.DelayUIDs = delayUIDs;

//                if (clause.HasObjects())
//                    clauses.Add(clause);
//            }
//            return clauses;
//        }

//        static void InitializeGuardUsers()
//        {
//            foreach (var guardUser in DeviceConfiguration.GuardUsers)
//            {
//                var zoneUIDs = new List<Guid>();
//                guardUser.Zones = new List<XZone>();
//                if (guardUser.ZoneUIDs != null)
//                    foreach (var zoneUID in guardUser.ZoneUIDs)
//                    {
//                        var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
//                        if (zone != null)
//                        {
//                            guardUser.Zones.Add(zone);
//                            zoneUIDs.Add(zoneUID);
//                        }
//                    }
//                guardUser.ZoneUIDs = zoneUIDs;
//            }
//        }

//        static void UpdateGKChildrenDescription()
//        {
//            foreach (var gkDevice in DeviceConfiguration.RootDevice.Children)
//            {
//                UpdateGKPredefinedName(gkDevice);
//            }
//        }

//        public static void UpdateGKPredefinedName(XDevice device)
//        {
//            if (device.DriverType == XDriverType.GK && device.Children.Count >= 15)
//            {
//                device.Children[0].PredefinedName = "Неисправность";
//                device.Children[1].PredefinedName = "Пожар 1";
//                device.Children[2].PredefinedName = "Пожар 2";
//                device.Children[3].PredefinedName = "Внимание";
//                device.Children[4].PredefinedName = "Включение ПУСК";
//                device.Children[5].PredefinedName = "Тест";
//                device.Children[6].PredefinedName = "Отключение";
//                device.Children[7].PredefinedName = "Автоматика отключена";
//                device.Children[8].PredefinedName = "Звук отключен";
//                device.Children[9].PredefinedName = "Останов пуска";
//                device.Children[10].PredefinedName = "Реле 1";
//                device.Children[11].PredefinedName = "Реле 2";
//                device.Children[12].PredefinedName = "Реле 3";
//                device.Children[13].PredefinedName = "Реле 4";
//                device.Children[14].PredefinedName = "Реле 5";
//            }
//        }

//        static void CopyMPTProperties()
//        {
//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                foreach (var mptDevice in mpt.MPTDevices)
//                {
//                    SetIsMPT(mptDevice);
//                }
//            }
//        }

//        public static void SetIsMPT(MPTDevice mptDevice)
//        {
//            if (mptDevice.Device != null)
//            {
//                mptDevice.Device.IsInMPT = true;
//                XManager.ChangeDeviceLogic(mptDevice.Device, new XDeviceLogic());
//                mptDevice.Device.ZoneUIDs = new List<Guid>();
//                mptDevice.Device.Zones.Clear();
//            }
//        }

//        public static void SetMPTDefaultProperty(XDevice device)
//        {
//            if (device != null)
//            {
//                switch (device.DriverType)
//                {
//                    case XDriverType.RSR2_AM_1:
//                        SetDeviceProperty(device, "Конфигурация", 1);
//                        break;

//                    case XDriverType.RSR2_OPS:
//                    case XDriverType.RSR2_OPZ:
//                    case XDriverType.RSR2_OPK:
//                        SetDeviceProperty(device, "Задержка на включение, с", 0);
//                        SetDeviceProperty(device, "Время удержания, с", 65000);
//                        SetDeviceProperty(device, "Задержка на выключение, с", 0);
//                        SetDeviceProperty(device, "Состояние для модуля Выключено", 0);
//                        SetDeviceProperty(device, "Состояние для режима Удержания", 4);
//                        SetDeviceProperty(device, "Состояние для режима Включено", 16);
//                        break;

//                    case XDriverType.RSR2_MVK8:
//                        SetDeviceProperty(device, "Задержка на включение, с", 0);
//                        SetDeviceProperty(device, "Время удержания, с", 2);
//                        SetDeviceProperty(device, "Задержка на выключение, с", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Удержания", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
//                        SetDeviceProperty(device, "Контроль", 0);
//                        SetDeviceProperty(device, "Норма питания, 0.1В", 80);
//                        break;

//                    case XDriverType.RSR2_RM_1:
//                        SetDeviceProperty(device, "Задержка на включение, с", 0);
//                        SetDeviceProperty(device, "Время удержания, с", 2);
//                        SetDeviceProperty(device, "Задержка на выключение, с", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Удержания", 0);
//                        SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
//                        break;
//                }
//            }
//        }

//        static void SetDeviceProperty(XDevice device, string propertyName, int value)
//        {
//            var property = device.Properties.FirstOrDefault(x => x.Name == propertyName);
//            if (property == null)
//            {
//                property = new XProperty()
//                {
//                    Name = propertyName,
//                    DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == propertyName)
//                };
//                device.Properties.Add(property);
//            }
//            property.Value = (ushort)value;
//        }
//    }
//}