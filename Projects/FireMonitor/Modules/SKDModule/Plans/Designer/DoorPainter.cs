﻿using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using SKDModule.Events;
using SKDModule.ViewModels;
using Infrastructure.Events;

namespace SKDModule.Plans.Designer
{
	class DoorPainter : BasePointPainter<Door, ShowDoorEvent>
	{
		private DoorViewModel _doorViewModel;

		public DoorPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_doorViewModel = new ViewModels.DoorViewModel(Item);
		}

		protected override Door CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementDoor;
			return element == null ? null : PlanPresenter.Cache.Get<Door>(element.DoorUID);
		}
		protected override StateTooltipViewModel<Door> CreateToolTip()
		{
			return new DoorTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Открыть", "pack://application:,,,/Controls;component/Images/BTurnOn.png", _doorViewModel.OpenCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Закрыть", "pack://application:,,,/Controls;component/Images/BTurnOff.png", _doorViewModel.CloseCommand));
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DoorDetailsViewModel(Item);
		}

		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.DoorPicture.GetBrush(background);
		}

		private Color GetStateColor()
		{
			return Colors.Green;
		}
	}
}