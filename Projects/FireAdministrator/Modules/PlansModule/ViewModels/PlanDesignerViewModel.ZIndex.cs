﻿using System.Linq;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using PlansModule.Designer;
using System.Collections.Generic;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		void InitializeZIndexCommands()
		{
			MoveToFrontCommand = new RelayCommand(OnMoveToFront, CanMoveExecute);
			SendToBackCommand = new RelayCommand(OnSendToBack, CanMoveExecute);
			MoveForwardCommand = new RelayCommand(OnMoveForward, CanMoveExecute);
			MoveBackwardCommand = new RelayCommand(OnMoveBackward, CanMoveExecute);
		}

		public bool CanMoveExecute(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand MoveToFrontCommand { get; private set; }
		private void OnMoveToFront()
		{
			int maxZIndex = 0;
			foreach (var designerItem in DesignerCanvas.Items)
				maxZIndex = System.Math.Max(designerItem.Element.ZIndex, maxZIndex);

			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.Element.ZIndex = maxZIndex + 1;
				designerItem.SetZIndex();
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand SendToBackCommand { get; private set; }
		private void OnSendToBack()
		{
			int minZIndex = 0;
			foreach (var designerItem in DesignerCanvas.Items)
				minZIndex = System.Math.Min(designerItem.Element.ZIndex, minZIndex);

			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.Element.ZIndex = minZIndex - 1;
				designerItem.SetZIndex();
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand MoveForwardCommand { get; private set; }
		private void OnMoveForward()
		{
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.Element.ZIndex++;
				designerItem.SetZIndex();
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand MoveBackwardCommand { get; private set; }
		private void OnMoveBackward()
		{
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.Element.ZIndex--;
				designerItem.SetZIndex();
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}

		private void NormalizeZIndex()
		{
			Dictionary<int, List<ElementBase>> map = new Dictionary<int, List<ElementBase>>();
			foreach (var item in DesignerCanvas.Items)
			{
				if (!map.ContainsKey(item.Element.ZLayer))
					map.Add(item.Element.ZLayer, new List<ElementBase>());
				map[item.Element.ZLayer].Add(item.Element);
			}
			foreach (int key in map.Keys)
			{
				var list = map[key].OrderBy(item => item.ZIndex).ToList();
				for (int i = 0; i < list.Count; i++)
					list[i].ZIndex = i;
			}
		}
	}
}
