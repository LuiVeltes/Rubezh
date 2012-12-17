﻿using System.Windows;
using System;
using Common;
using System.Diagnostics;
using Infrastructure.Common.Theme;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;

namespace FSAgentServer
{
    public partial class App : Application
    {
		private const string SignalId = "7D46A5A4-AC89-4F36-A834-1070CFCFF609";
		private const string WaitId = "A64BC0A9-319C-4028-B666-5CE56BFD1B1B";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

            using (new DoubleLaunchLocker(SignalId, WaitId, true))
            {
                try
                {
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    ThemeHelper.LoadThemeFromRegister();
					FSAgentLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    FSAgentLoadHelper.SetStatus(FSAgentState.Opening);
                    Bootstrapper.Run();
                }
                catch(Exception ex)
                {
                    BalloonHelper.Show("Агент Firesec", "Ошибка во время загрузки");
                    Logger.Error(ex, "App.OnStartup");
                }
            }
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject as Exception, "App.CurrentDomain_UnhandledException");
			Restart();
		}

        public static void Restart()
        {
			Logger.Error("App.Restart");
#if DEBUG
            return;
#endif
            BalloonHelper.Show("Агент Firesec", "Перезапуск");
            Bootstrapper.Close();
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = Application.ResourceAssembly.Location,
            };
            System.Diagnostics.Process.Start(processStartInfo);

			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
        }
    }
}