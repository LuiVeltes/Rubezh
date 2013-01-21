﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Infrastructure.Common
{
	public class AppDataFolderHelper
	{
		static string AppDataFolderName;

		static AppDataFolderHelper()
		{
			var appDataFolderName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			AppDataFolderName = Path.Combine(appDataFolderName, "Firesec2");
		}

		public static string GetFolder(string folderName)
		{
			return Path.Combine(AppDataFolderName, folderName);
		}

		public static string GetTempFileName()
		{
			return Path.Combine(AppDataFolderName, "Temp", Path.GetTempFileName());
		}

		public static string GetTempFolder()
		{
			return Path.Combine(AppDataFolderName, "Temp", Guid.NewGuid().ToString());
		}

		public static string GetClientConfigurationDirectory()
		{
			return Path.Combine(AppDataFolderName, "CommonClientConfiguration");
		}

		public static string GetMonitorSettingsPath(string fileName = null)
		{
			var filePath = Path.Combine(AppDataFolderName, "Monitor", "Settings");
			if (!string.IsNullOrEmpty(fileName))
				filePath = Path.Combine(filePath, fileName);
			return filePath;
		}

		public static string GetServerAppDataPath(string fileOrDirectoryName = null)
		{
			var fileName = Path.Combine(AppDataFolderName, "Server");
			if (!string.IsNullOrEmpty(fileOrDirectoryName))
				fileName = Path.Combine(fileName, fileOrDirectoryName);
			return fileName;
		}

		public static string GetDBFile(string fileName)
		{
			return Path.Combine(AppDataFolderName, "DB", fileName);
		}

		public static string GetMulticlientDirectory()
		{
			return Path.Combine(AppDataFolderName, "Multiclient");
		}

		public static string GetMulticlientFile()
		{
			return Path.Combine(AppDataFolderName, "Multiclient", "MulticlientConfiguration.xml");
		}

		public static string GetTempMulticlientFile()
		{
			return Path.Combine(AppDataFolderName, "Multiclient", "TempConfiguration.xml");
		}

		public static string GetLogsFolder(string folderName)
		{
			return Path.Combine(AppDataFolderName, "Logs", folderName);
		}
	}
}