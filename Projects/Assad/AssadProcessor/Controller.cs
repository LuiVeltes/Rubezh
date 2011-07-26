﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecClient;
using System.Windows.Forms;
using Firesec;
using AssadProcessor.Devices;
using System.Diagnostics;
using FiresecClient.Models;

namespace AssadProcessor
{
    public class Controller
    {
        internal static Controller Current { get; private set; }
        Watcher _watcher;

        public Controller()
        {
            Current = this;
        }

        public void Start()
        {
            FiresecManager.Connect("adm", "");

            Services.NetManager.Start();
            _watcher = new Watcher();
            _watcher.Start();
        }

        internal void AssadConfig(Assad.MHconfigTypeDevice innerDevice, bool all)
        {
            if (all)
            {
                Services.DeviceManager.Config(innerDevice);
            }
        }

        public Assad.DeviceType[] QueryState(Assad.MHqueryStateType content)
        {
            var assadBase = Configuration.BaseDevices.FirstOrDefault(a => a.DeviceId == content.deviceId);
            if (assadBase != null)
            {
                List<AssadBase> devices = assadBase.FindAllChildren();

                List<Assad.DeviceType> deviceItems = new List<Assad.DeviceType>();
                foreach (var childAssadBase in devices)
                {
                    deviceItems.Add(childAssadBase.GetStates());
                }
                return deviceItems.ToArray();
            }
            return null;
        }

        public Assad.DeviceType QueryAbility(Assad.MHqueryAbilityType content)
        {
            var device = Configuration.BaseDevices.First(a => a.DeviceId == content.deviceId);
            Assad.DeviceType ability = device.QueryAbility();
            return ability;
        }

        public void AssadExecuteCommand(Assad.MHdeviceControlType controlType)
        {
            var assadDevice = Configuration.Devices.First(x => x.DeviceId == controlType.deviceId);
            string commandName = controlType.cmdId;
            if (commandName == "Обновить")
            {
                Assad.CPqueryConfigurationType cPqueryConfigurationType = new Assad.CPqueryConfigurationType();
                NetManager.Send(cPqueryConfigurationType, null);
            }
            else
            {
                var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == assadDevice.Id);
                if (device != null)
                {
                    if (commandName.StartsWith("Сброс "))
                    {
                        commandName = commandName.Replace("Сброс ", "");

                        if (device.Driver.DriverName == "Компьютер")
                        {
                            foreach (var resetDevice in FiresecManager.Configuration.Devices)
                            {
                                if (resetDevice.Driver.States.Any(x => ((x.Name == commandName) && (x.IsManualReset))))
                                {
                                    FiresecResetHelper.ResetOne(resetDevice.Id, commandName);
                                }
                            }
                        }
                        else
                        {
                            FiresecResetHelper.ResetOne(device.Id, commandName);
                        }
                    }
                }
            }
        }

        public void ResetAllStates(string deviceId)
        {
            var assadDevice = Configuration.Devices.First(x => x.DeviceId == deviceId);
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == assadDevice.Id);
            if (device != null)
            {
                foreach (var state in device.Driver.States)
                {
                    if (state.IsManualReset)
                    {
                        FiresecResetHelper.ResetOne(device.Id, state.Name);
                    }
                }
            }
        }

        public void Stop()
        {
            Services.LogEngine.Save();
            FiresecManager.Disconnect();
            Services.NetManager.Stop();
        }

        internal bool Ready { get; set; }
    }
}
