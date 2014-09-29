﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common;
using Infrastructure.Models;
using System.Xml.Serialization;

namespace Infrastructure
{
	public static class ClientSettings
	{
		public static readonly string ArchiveDefaultStateFileName = AppDataFolderHelper.GetMonitorSettingsPath("ArchiveDefaultState.xml");
		public static readonly string AutoActivationSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("AutoActivationSettings.xml");
		public static readonly string MultiLayoutCameraSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("MultiLayoutCameraSettings.xml");
		public static readonly string RviMultiLayoutCameraSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("RviMultiLayoutCameraSettings.xml");
		public static readonly string SKDSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("SKDSettings.xml");

		static ArchiveDefaultState _archiveDefaultState;
		public static ArchiveDefaultState ArchiveDefaultState
		{
			get { return _archiveDefaultState ?? (_archiveDefaultState = new ArchiveDefaultState()); }
			set { _archiveDefaultState = value; }
		}

		static AutoActivationSettings _autoActivationSettings;
		public static AutoActivationSettings AutoActivationSettings
		{
			get { return _autoActivationSettings ?? (_autoActivationSettings = new AutoActivationSettings()); }
			set { _autoActivationSettings = value; }
		}

		static RviMultiLayoutCameraSettings _rviMultiLayoutCameraSettings;
		public static RviMultiLayoutCameraSettings RviMultiLayoutCameraSettings
		{
			get { return _rviMultiLayoutCameraSettings ?? (_rviMultiLayoutCameraSettings = new RviMultiLayoutCameraSettings()); }
			set { _rviMultiLayoutCameraSettings = value; }
		}

		static SKDSettings _skdSettings;
		public static SKDSettings SKDSettings
		{
			get { return _skdSettings ?? (_skdSettings = new SKDSettings()); }
			set { _skdSettings = value; }
		}

		public static void LoadSettings()
		{
			try
			{
				LoadArchiveDefaultState();
				LoadAutoActivationSettings();
				LoadRviCameraSettings();
				LoadSKDSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.LoadSettings");
			}
		}

		public static void SaveSettings()
		{
			try
			{
				if (Directory.Exists(AppDataFolderHelper.GetMonitorSettingsPath()) == false)
					Directory.CreateDirectory(AppDataFolderHelper.GetMonitorSettingsPath());

				SaveArchiveDefaultState();
				SaveAutoActivationSettings();
				SaveRviCameraSettings();
				SaveSKDSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.SaveSettings");
			}
		}

		static void LoadArchiveDefaultState()
		{
			if (File.Exists(ArchiveDefaultStateFileName))
			{
				using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var xmlSerializer = new XmlSerializer(typeof(ArchiveDefaultState));
					ArchiveDefaultState = (ArchiveDefaultState)xmlSerializer.Deserialize(fileStream);
					if (ArchiveDefaultState.XAdditionalColumns == null)
						ArchiveDefaultState.XAdditionalColumns = new List<XJournalColumnType>();
					if (ArchiveDefaultState.AdditionalJournalColumnTypes == null)
						ArchiveDefaultState.AdditionalJournalColumnTypes = new List<JournalColumnType>();
				}
			}
			else
			{
				ArchiveDefaultState = new ArchiveDefaultState();
			}
		}

		static void LoadAutoActivationSettings()
		{
			if (File.Exists(AutoActivationSettingsFileName))
			{
				using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(typeof(AutoActivationSettings));
					AutoActivationSettings = (AutoActivationSettings)xmlSerializer.Deserialize(fileStream);
				}
			}
			else
			{
				AutoActivationSettings = new AutoActivationSettings();
			}
		}

		static void LoadRviCameraSettings()
		{
			if (File.Exists(RviMultiLayoutCameraSettingsFileName))
			{
				using (var fileStream = new FileStream(RviMultiLayoutCameraSettingsFileName, FileMode.Open))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(RviMultiLayoutCameraSettings));
					RviMultiLayoutCameraSettings = (RviMultiLayoutCameraSettings)dataContractSerializer.ReadObject(fileStream);
					if (RviMultiLayoutCameraSettings.Dictionary == null)
						RviMultiLayoutCameraSettings.Dictionary = new Dictionary<string, Guid>();
				}
			}
			else
			{
				RviMultiLayoutCameraSettings = new RviMultiLayoutCameraSettings();
			}
		}

		static void LoadSKDSettings()
		{
			if (File.Exists(SKDSettingsFileName))
			{
				using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(typeof(SKDSettings));
					SKDSettings = (SKDSettings)xmlSerializer.Deserialize(fileStream);
				}
			}
			else
			{
				SKDSettings = new SKDSettings();
			}
		}

		static void SaveArchiveDefaultState()
		{
			using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Create))
			{
				var xmlSerializer = new XmlSerializer(typeof(ArchiveDefaultState));
				xmlSerializer.Serialize(fileStream, ArchiveDefaultState);
			}
		}

		static void SaveAutoActivationSettings()
		{
			using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Create))
			{
				var xmlSerializer = new XmlSerializer(typeof(AutoActivationSettings));
				xmlSerializer.Serialize(fileStream, AutoActivationSettings);
			}
		}

		static void SaveRviCameraSettings()
		{
			using (var fileStream = new FileStream(RviMultiLayoutCameraSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(RviMultiLayoutCameraSettings));
				dataContractSerializer.WriteObject(fileStream, RviMultiLayoutCameraSettings);
			}
		}

		static void SaveSKDSettings()
		{
			using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Create))
			{
				var xmlSerializer = new XmlSerializer(typeof(SKDSettings));
				xmlSerializer.Serialize(fileStream, SKDSettings);
			}
		}
	}
}