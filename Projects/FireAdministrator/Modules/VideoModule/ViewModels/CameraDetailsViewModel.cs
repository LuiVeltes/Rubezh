﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public bool IsEditMode { get; private set; }

		public CameraDetailsViewModel(Camera camera = null)
		{
			if (camera != null)
			{
				Camera = camera;
				Title = (Camera.CameraType == CameraType.Dvr ? "Свойства видеорегистратора" :
					Camera.CameraType == CameraType.Camera ? "Свойства камеры" : "Свойства канала");
				IsEditMode = true;
				CopyProperties();
			}
			else
			{
				Title = "Создание нового видеоустройства";
				Camera = new Camera();
				Name = "Видеоустройство";
				Address = "172.16.5.201";
				ChannelsCount = 1;
				ChannelNumber = 1;
				SelectedCameraType = CameraType.Dvr;
				IsEditMode = false;
			}
			ShowZonesCommand = new RelayCommand(OnShowZones);
			ShowCommand = new RelayCommand(OnShow);
			Initialize();
		}

		public List<XStateClass> StateClasses { get; private set; }
		void Initialize()
		{
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);
			CameraTypes = new List<CameraType>();
			CameraTypes.Add(CameraType.Dvr);
			CameraTypes.Add(CameraType.Camera);
			if ((CamerasViewModel.Current.SelectedCamera != null) &&
				((CamerasViewModel.Current.SelectedCamera.Parent != null) ||
				(CamerasViewModel.Current.SelectedCamera.Camera.CameraType == CameraType.Dvr)))
				CameraTypes.Add(CameraType.Channel);
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}

		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		public List<CameraType> CameraTypes { get; private set; }

		private CameraType _selectedCameraType;
		public CameraType SelectedCameraType
		{
			get { return _selectedCameraType; }
			set
			{
				_selectedCameraType = value;
				if (!IsEditMode)
				{
					if (value == CameraType.Dvr)
						Name = "Видеорегистратор";
					if (value == CameraType.Camera)
						Name = "Камера";
					if (value == CameraType.Channel)
						Name = "Канал";
				}
				OnPropertyChanged(() => SelectedCameraType);
			}
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Ip;
			SelectedCameraType = Camera.CameraType;
			SelectedStateClass = Camera.StateClass;
			ChannelNumber = Camera.ChannelNumber + 1;
			Left = Camera.Left;
			Top = Camera.Top;
			Width = Camera.Width;
			Height = Camera.Height;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		int _channelsCount;
		public int ChannelsCount
		{
			get { return _channelsCount; }
			set
			{
				_channelsCount = value;
				OnPropertyChanged(() => ChannelsCount);
			}
		}

		int _channelNumber;
		public int ChannelNumber
		{
			get { return _channelNumber; }
			set
			{
				_channelNumber = value;
				OnPropertyChanged(() => ChannelNumber);
			}
		}

		XStateClass _selectedStateClass;
		public XStateClass SelectedStateClass
		{
			get { return _selectedStateClass; }
			set
			{
				_selectedStateClass = value;
				OnPropertyChanged(() => SelectedStateClass);
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Camera.ZoneUIDs);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Camera.ZoneUIDs = zonesSelectationViewModel.Zones;
				OnPropertyChanged(() => PresentationZones);
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(zones));
				return presentationZones;
			}
		}

		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				if (_vlcControl == null)
					return new BitmapImage();
				return _vlcControl.VideoSource;
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			try
			{
				if (!VlcContext.IsInitialized)
				{
					//Set libvlc.dll and libvlccore.dll directory path
					VlcContext.LibVlcDllsPath = FiresecManager.SystemConfiguration.RviSettings.DllsPath;
					//Set the vlc plugins directory path
					VlcContext.LibVlcPluginsPath = FiresecManager.SystemConfiguration.RviSettings.PluginsPath;

					//Set the startup options
					VlcContext.StartupOptions.IgnoreConfig = true;
					VlcContext.StartupOptions.LogOptions.LogInFile = false;
					VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
					VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

					//Initialize the VlcContext
					VlcContext.Initialize();
				}
				_vlcControl = new VlcControl { Media = new LocationMedia(Camera.RviRTSP) };
				_vlcControl.PositionChanged -= VlcControlOnPositionChanged;
				_vlcControl.PositionChanged += VlcControlOnPositionChanged;
				if (_vlcControl.IsPlaying)
					_vlcControl.Stop();
				_vlcControl.Play();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged(() => Image);
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (_vlcControl.IsPlaying)
				_vlcControl.Stop();
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			if (!Validation())
			{
				OnClosing(false);
				return false;
			}
			Camera.Name = Name;
			Camera.Ip = Address;
			Camera.CameraType = SelectedCameraType;
			Camera.StateClass = SelectedStateClass;
			Camera.Left = Left;
			Camera.Top = Top;
			Camera.Width = Width;
			Camera.Height = Height;
			Camera.ChannelNumber = ChannelNumber - 1;
			for (int i = 0; i < ChannelsCount; i++)
			{
				Camera.Children.Add(new Camera
				{
					ChannelNumber = i,
					Parent = Camera,
					CameraType = CameraType.Channel,
					Name = "Канал",
					Ip = Address,
				});
			}
			if ((Camera.Children != null) && (Camera.Children.Count > 0))
				foreach (var child in Camera.Children)
				{
					child.Ip = Address;
				}
			return base.Save();
		}

		bool Validation()
		{
			if (SelectedCameraType == CameraType.Channel)
			{
				if (CamerasViewModel.Current.SelectedCamera.IsDvr)
				{
					if (CamerasViewModel.Current.SelectedCamera.Children.Any(x => (x.Camera.ChannelNumber == (ChannelNumber - 1)) &&
						x.Camera != Camera))
					{
						MessageBoxService.ShowError("Канал с таким номером уже существует", "Сообщение");
						return false;
					}
				}
				else
				{
					var children = CamerasViewModel.Current.SelectedCamera.Parent.Children;
					if (CamerasViewModel.Current.SelectedCamera.Parent.Children.Any(x => (x.Camera.ChannelNumber == (ChannelNumber - 1)) &&
						x.Camera != Camera))
					{
						MessageBoxService.ShowError("Канал с таким номером уже существует", "Сообщение");
						return false;
					}
				}
			}
			return true;
		}
	}
}
