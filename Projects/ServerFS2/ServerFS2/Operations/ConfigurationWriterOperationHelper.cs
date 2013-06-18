﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerFS2.Service;
using FS2Api;
using ServerFS2.ConfigurationWriter;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class ConfigurationWriterOperationHelper
	{
		public static void Write(Device device)
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Формирование базы данных устройств") });
			var systemDatabaseCreator = new SystemDatabaseCreator();
			systemDatabaseCreator.Create();

			var panelDatabase = systemDatabaseCreator.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
			if (panelDatabase == null)
				throw new FS2Exception("Не найдена сформированная для устройства база данных");

			var parentPanel = panelDatabase.ParentPanel;
			var bytes1 = panelDatabase.RomDatabase.BytesDatabase.GetBytes();
			var bytes2 = panelDatabase.FlashDatabase.BytesDatabase.GetBytes();
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo("Запись базы данных в прибор") });
			SetConfigurationOperationHelper.SetDeviceConfig(parentPanel, bytes2, bytes1);
		}
	}
}