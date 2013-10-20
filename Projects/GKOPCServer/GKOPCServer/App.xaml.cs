﻿using System;
using System.Windows;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Theme;

namespace GKOPCServer
{
	public partial class App : Application
	{
		private const string SignalId = "CF4257AB-ABFA-4E14-B46A-C1D5C754BE1B";
		private const string WaitId = "5C0BDECC-2391-4996-82CC-8994912B3420";

		private void OnStartup(object sender, StartupEventArgs e)
		{
			try
			{
				PatchManager.Patch();
				ThemeHelper.LoadThemeFromRegister();
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				using (new DoubleLaunchLocker(SignalId, WaitId, true))
				{
					try
					{
						Bootstrapper.Run();
					}
					catch (Exception ex)
					{
						Logger.Error(ex, "App.OnStartup 1");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup 2");
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject as Exception);
		}
	}
}