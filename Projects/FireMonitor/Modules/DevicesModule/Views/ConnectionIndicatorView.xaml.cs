﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FiresecClient;
using FSAgentClient;
using Infrastructure.Common.BalloonTrayTip;
using FS2Client;

namespace DevicesModule.Views
{
	public partial class ConnectionIndicatorView : UserControl, INotifyPropertyChanged
	{
		public ConnectionIndicatorView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			SafeFiresecService.ConnectionLost += new Action(OnService_ConnectionLost);
			SafeFiresecService.ConnectionAppeared += new Action(OnService_ConnectionAppeared);

			if (FiresecManager.IsFS2Enabled)
			{
				FS2ClientContract.ConnectionLost += new Action(FS2OrAgent_ConnectionLost);
				FS2ClientContract.ConnectionAppeared += new Action(FS2OrAgent_ConnectionAppeared);
			}
			else
			{
				FSAgent.ConnectionLost += new Action(FS2OrAgent_ConnectionLost);
				FSAgent.ConnectionAppeared += new Action(FS2OrAgent_ConnectionAppeared);
			}
		}

		bool _isServiceConnected = true;
		public bool IsServiceConnected
		{
			get { return _isServiceConnected; }
			set
			{
				_isServiceConnected = value;
				OnPropertyChanged("IsServiceConnected");
				if (value)
				{
					_serviceConnectionControl.ToolTip = "Связь с сервером в норме";
					_serviceConnectionControl.Background = Brushes.Transparent;
					_serviceConnectionIndicator.Opacity = 0.4;
				}
				else
				{
					_serviceConnectionControl.ToolTip = "Связь с сервером потеряна";
					_serviceConnectionControl.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_serviceConnectionControl.Background = Brushes.DarkOrange;
					_serviceConnectionIndicator.Opacity = 1;
					BalloonHelper.ShowFromMonitor("Связь с сервером потеряна");
				}
			}
		}

		bool _isDeviceConnected = true;
		public bool IsDeviceConnected
		{
			get { return _isDeviceConnected; }
			set
			{
				_isDeviceConnected = value;
				OnPropertyChanged("IsDeviceConnected");
				if (value)
				{
					_deviceConnectionControl.ToolTip = "Связь с устройствами в норме";
					_deviceConnectionControl.Background = Brushes.Transparent;
					_deviceConnectionIndicator.Opacity = 0.4;
				}
				else
				{
					_deviceConnectionControl.ToolTip = "Связь с устройствами потеряна";
					_deviceConnectionControl.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_deviceConnectionIndicator.Opacity = 1;
					BalloonHelper.ShowFromMonitor("Связь с агентом потеряна");
				}
			}
		}

		ObjectAnimationUsingKeyFrames GetAnimation(bool start)
		{
			var animation = new ObjectAnimationUsingKeyFrames();
			if (!start)
			{
				animation.Duration = TimeSpan.FromSeconds(1.5);
				animation.RepeatBehavior = RepeatBehavior.Forever;
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));
			}
			else
			{
				animation.Duration = Duration.Forever;
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible));
			}
			return animation;
		}

		void OnService_ConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServiceConnected = false;
				_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			}));
		}

		void OnService_ConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServiceConnected = true;
				_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			}));
		}

		void FS2OrAgent_ConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsDeviceConnected = false;
				_deviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsDeviceConnected));
			}));
		}

		void FS2OrAgent_ConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsDeviceConnected = true;
				_deviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsDeviceConnected));
			}));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}