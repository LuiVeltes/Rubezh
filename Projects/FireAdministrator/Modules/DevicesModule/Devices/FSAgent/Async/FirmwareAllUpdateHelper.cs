﻿using System.IO;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Threading;
using System;

namespace DevicesModule.ViewModels
{
    public static class FirmwareAllUpdateHelper
    {
        static Device Device;
        static string FileName;
        static string ReportString;
        static OperationResult<string> OperationResult_1;
        static OperationResult<string> OperationResult_2;
        static List<DriverType> DriverTypesToUpdate;
        static List<UpdatingDevice> UpdatingDevices;
        static bool IsRewriteSame;
        static bool IsRewriteOld;

        
        public static bool IsManyDevicesToUpdate(Device device)
        {
            Device = device;
            GetDriverTypesToUpdate();
            return FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.Where(x => DriverTypesToUpdate.Contains(x.Driver.DriverType)).Count() > 1;
        }

        public static void Run(Device device)
        {
            Device = device;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
                GetDriverTypesToUpdate();
                UpdatingDevices = new List<UpdatingDevice>();
                FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.Where(
                    x => DriverTypesToUpdate.Contains(x.Driver.DriverType)).ToList().ForEach(
                        x => UpdatingDevices.Add(new UpdatingDevice(x)));
                ServiceFactory.ProgressService.Run(OnVerifyProgress, OnVerifyCompleted, "Сравнение прошивок");
                if(UpdatingDevices.Any(x => x.IsOldVersion))
                    IsRewriteOld = MessageBoxService.ShowQuestion("В приборе содержится более новая версия программного обеспечения. Вы уверены что хотите вернуться к старой версии ПО ?") == MessageBoxResult.Yes;
                if (UpdatingDevices.Any(x => x.IsSameVersion))
                    IsRewriteSame = MessageBoxService.ShowQuestion("В приборе уже установлена данная версия программного обеспечения. Продолжить ?") == MessageBoxResult.Yes;
                ServiceFactory.ProgressService.Run(OnUpdateProgress, OnUpdateCompleted, "Обновление прошивки");
            }
        }

        static void OnVerifyProgress()
        {
            foreach (var updatingDevice in UpdatingDevices)
            {
                OperationResult_1 = FiresecManager.DeviceVerifyFirmwareVersion(updatingDevice.Device, updatingDevice.Device.IsUsb, new FileInfo(FileName).FullName);
                if (OperationResult_1.HasError)
                {
                    ReportString += "Ошибка при проверке версии " + updatingDevice.Device.PresentationAddressAndName + " " + OperationResult_1.Error + "\n";
                    updatingDevice.IsError = true;
                }
                else if (OperationResult_1.Result == "В приборе уже установлена данная версия программного обеспечения. Продолжить ?")
                {
                    updatingDevice.IsSameVersion = true;
                }
                else if (OperationResult_1.Result == "В приборе содержится более новая версия программного обеспечения. Вы уверены что хотите вернуться к старой версии ПО ?")
                {
                    updatingDevice.IsOldVersion = true;
                }
            }
        }

        static void OnVerifyCompleted()
        {
            ;
        }


        static void OnUpdateProgress()
        {
			foreach (var updatingDevice in UpdatingDevices.Where(x => ShouldBeUpdated(x)))
            {
                OperationResult_2 = FiresecManager.DeviceUpdateFirmware(updatingDevice.Device, updatingDevice.Device.IsUsb, new FileInfo(FileName).FullName);
                if (OperationResult_2.HasError)
                    ReportString += "Ошибка при выполнении операции " + updatingDevice.Device.PresentationAddressAndName + " " + OperationResult_2.Error + "\n";
                else
                    ReportString += "Операция завершилась успешно " + updatingDevice.Device.PresentationAddressAndName + "\n";
            }
        }

        static bool ShouldBeUpdated(UpdatingDevice updatingDevice)
        {
            if(IsRewriteOld && IsRewriteSame)
                return !updatingDevice.IsError;
            else if(IsRewriteOld)
                return !updatingDevice.IsError && !updatingDevice.IsSameVersion;
            else if(IsRewriteSame)
                return !updatingDevice.IsError && !updatingDevice.IsOldVersion;
            else
                return !updatingDevice.IsError && !updatingDevice.IsSameVersion && !updatingDevice.IsOldVersion;
        }

        static void GetDriverTypesToUpdate()
        {
            DriverTypesToUpdate = new List<DriverType>();
            var driverType = Device.Driver.DriverType;
            DriverTypesToUpdate.Add(Device.Driver.DriverType);
            if (driverType == DriverType.BUNS)
                DriverTypesToUpdate.Add(DriverType.USB_BUNS);
            if (driverType == DriverType.Rubezh_2AM)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_2AM);
            if (driverType == DriverType.Rubezh_2OP)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_2OP);
            if (driverType == DriverType.Rubezh_4A)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_4A);
            if (driverType == DriverType.Rubezh_P)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_P);

            if (driverType == DriverType.USB_BUNS)
                DriverTypesToUpdate.Add(DriverType.BUNS);
            if (driverType == DriverType.USB_Rubezh_2AM)
                DriverTypesToUpdate.Add(DriverType.Rubezh_2AM);
            if (driverType == DriverType.USB_Rubezh_2OP)
                DriverTypesToUpdate.Add(DriverType.Rubezh_2OP);
            if (driverType == DriverType.USB_Rubezh_4A)
                DriverTypesToUpdate.Add(DriverType.Rubezh_4A);
            if (driverType == DriverType.USB_Rubezh_P)
                DriverTypesToUpdate.Add(DriverType.Rubezh_P);
        }

        static void OnUpdateCompleted()
        {
            MessageBoxService.Show(ReportString);
        }

    }

    public class UpdatingDevice
    {
        public Device Device;
        public bool IsError;
        public bool IsSameVersion;
        public bool IsOldVersion;
        public UpdatingDevice(Device device)
        {
            Device = device;
        }
    }
}
