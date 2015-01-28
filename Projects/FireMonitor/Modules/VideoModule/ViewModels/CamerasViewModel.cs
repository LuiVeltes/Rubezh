﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Entities.DeviceOriented;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static CamerasViewModel Current { get; private set; }
		public CamerasViewModel()
		{
			Current = this;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, () => SelectedCamera != null && SelectedCamera.Camera.PlanElementUIDs.Count > 0);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, () => SelectedCamera != null);
			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
			Initialize();
		}

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			ApplicationService.BeginInvoke(() =>
			{
				SelectedCamera.Connect();
				SelectedCamera.StartAll();
			});
		}

		bool CanConnect()
		{
			return ((SelectedCamera != null) && (SelectedCamera.Status != DeviceStatuses.Connected));
		}

		public RelayCommand DisconnectCommand { get; private set; }
		void OnDisconnect()
		{
			ApplicationService.BeginInvoke(() =>
			{
				SelectedCamera.Disconnect();
				SelectedCamera.StopAll();
			});
		}

		bool CanDisconnect()
		{
			return ((SelectedCamera != null) && (SelectedCamera.Status != DeviceStatuses.Disconnected));
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(SelectedCamera.Camera));
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(camera);
				Cameras.Add(cameraViewModel);
			}
			AllCameras = new List<CameraViewModel>();
			foreach (var camera in Cameras)
			{
				AllCameras.Add(camera);
				AllCameras.AddRange(camera.Children);
			}

			SelectedCamera = Cameras.FirstOrDefault();
		}

		ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactoryBase.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(SelectedCamera.Camera);
		}

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = AllCameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
		}

		public List<CameraViewModel> AllCameras;
	}
}