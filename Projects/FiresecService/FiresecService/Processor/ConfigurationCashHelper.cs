﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Ionic.Zip;

namespace FiresecService
{
	public static class ConfigurationCashHelper
	{
		public static SecurityConfiguration SecurityConfiguration { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }

		public static void Update()
		{
			SecurityConfiguration = GetSecurityConfiguration();
			SystemConfiguration = GetSystemConfiguration();
			XManager.DeviceConfiguration = GetDeviceConfiguration();
			SKDManager.SKDConfiguration = GetSKDConfiguration();

			SystemConfiguration.UpdateConfiguration();

			GKDriversCreator.Create();
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			DescriptorsManager.CreateDynamicObjectsInXManager();
			XManager.UpdateConfiguration();

			SKDManager.UpdateConfiguration();
		}

		public static SecurityConfiguration GetSecurityConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var securityConfiguration = (SecurityConfiguration)GetConfigurationFomZip(zipFile, "SecurityConfiguration.xml", typeof(SecurityConfiguration));
			securityConfiguration.AfterLoad();
			zipFile.Dispose();
			return securityConfiguration;
		}

		static SystemConfiguration GetSystemConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			if (zipFile != null)
			{
				var systemConfiguration = (SystemConfiguration)GetConfigurationFomZip(zipFile, "SystemConfiguration.xml", typeof(SystemConfiguration));
				if (systemConfiguration != null)
				{
					systemConfiguration.AfterLoad();
				}
				else
				{
					systemConfiguration = new SystemConfiguration();
				}
				if (systemConfiguration.AutomationConfiguration == null)
					systemConfiguration.AutomationConfiguration = new AutomationConfiguration();
				if (systemConfiguration.AutomationConfiguration.AutomationSchedules == null)
				{
					systemConfiguration.AutomationConfiguration.AutomationSchedules = new List<AutomationSchedule>();
				}
				zipFile.Dispose();
				return systemConfiguration;
			}
			return new SystemConfiguration();
		}

		static XDeviceConfiguration GetDeviceConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var deviceConfiguration = (XDeviceConfiguration)GetConfigurationFomZip(zipFile, "XDeviceConfiguration.xml", typeof(XDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new XDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			zipFile.Dispose();
			return deviceConfiguration;
		}

		static SKDConfiguration GetSKDConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			if (zipFile != null)
			{
				var skdConfiguration = (SKDConfiguration)GetConfigurationFomZip(zipFile, "SKDConfiguration.xml", typeof(SKDConfiguration));
				if (skdConfiguration != null)
				{
					skdConfiguration.AfterLoad();
				}
				else
				{
					skdConfiguration = new SKDConfiguration();
				}
				zipFile.Dispose();
				return skdConfiguration;
			}
			return new SKDConfiguration();
		}

		static VersionedConfiguration GetConfigurationFomZip(ZipFile zipFile, string fileName, Type type)
		{
			try
			{
				var configurationEntry = zipFile[fileName];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;

					var dataContractSerializer = new DataContractSerializer(type);
					var versionedConfiguration = (VersionedConfiguration)dataContractSerializer.ReadObject(configurationMemoryStream);
					versionedConfiguration.ValidateVersion();
					return versionedConfiguration;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}
	}
}