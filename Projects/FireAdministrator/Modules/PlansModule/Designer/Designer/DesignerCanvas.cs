﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.ViewModels;
using Common;
using PlansModule.Designer.DesignerItems;

namespace PlansModule.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public Plan Plan { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
		public ToolboxViewModel Toolbox { get; set; }
		private Point? _startPoint = null;
		private List<ElementBase> _initialElements;
		private Dictionary<Plan, DesignerSurface> _canvasMap;

		public DesignerCanvas()
			: base(ServiceFactory.Events)
		{
			_canvasMap = new Dictionary<Plan, DesignerSurface>();
			Background = Brushes.Orange;
			Width = 100;
			Height = 100;
		}

		public override double Zoom
		{
			get { return PlanDesignerViewModel.Zoom; }
		}
		public override double PointZoom
		{
			get { return PlanDesignerViewModel.DeviceZoom / Zoom; }
		}

		public override void Clear()
		{
			base.Clear();
			_canvasMap.Clear();
		}
		public void RegisterPlan(Plan plan)
		{
			DeselectAll();
			AddCanvas();
			SelectedCanvas.AllowDrop = true;
			CanvasBackground = new SolidColorBrush(Colors.DarkGray);
			CanvasWidth = 100;
			CanvasHeight = 100;
			SelectedCanvas.DataContext = this;
			var pasteItem = new MenuItem()
			{
				Header = "Вставить",
				CommandParameter = this
			};
			pasteItem.SetBinding(MenuItem.CommandProperty, new Binding("Toolbox.PlansViewModel.PasteCommand"));
			ContextMenu = new ContextMenu();
			ContextMenu.Items.Add(pasteItem);
			using (new TimeCounter("\t\tDesignerCanvas.Background: {0}"))
				Update(plan);
			_canvasMap.Add(plan, SelectedCanvas);
			SelectedCanvas.Visibility = System.Windows.Visibility.Collapsed;
		}
		public void RemovePlan()
		{
			if (SelectedCanvas != null)
			{
				DeselectAll();
				RemoveCanvas();
				_canvasMap.Remove(Plan);
				Plan = null;
			}
		}
		public void ShowPlan(Plan plan)
		{
			Plan = plan;
			if (SelectedCanvas != null)
			{
				DeselectAll();
				SelectedCanvas.Visibility = System.Windows.Visibility.Collapsed;
				SelectedCanvas = null;
			}
			if (plan != null)
			{
				SelectedCanvas = _canvasMap[plan];
				Height = SelectedCanvas.Height;
				Width = SelectedCanvas.Width;
				SelectedCanvas.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void RemoveAllSelected()
		{
			var elements = SelectedElements;
			if (elements.Count() == 0)
				return;

			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(elements.ToList());
			foreach (var designerItem in SelectedItems.ToList())
				Remove(designerItem);
			ServiceFactory.SaveService.PlansChanged = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Source == SelectedCanvas && e.ChangedButton == MouseButton.Left)
			{
				_startPoint = new Point?(e.GetPosition(this));
				Toolbox.Apply(_startPoint);
				e.Handled = true;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
				_startPoint = null;
		}

		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
			if (elementBase != null)
			{
				Toolbox.SetDefault();
				//elementBase.SetDefault();

				Point position = e.GetPosition(this);
				elementBase.Position = position;
				CreateDesignerItem(elementBase);
				e.Handled = true;
			}
		}
		public override void CreateDesignerItem(ElementBase elementBase)
		{
			var designerItem = AddElement(elementBase);
			if (designerItem != null)
			{
				DeselectAll();
				designerItem.IsSelected = true;
				PlanDesignerViewModel.MoveToFrontCommand.Execute();
				ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
				Toolbox.SetDefault();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public DesignerItem AddElement(ElementBase elementBase)
		{
			if (elementBase is ElementRectangle)
				Plan.ElementRectangles.Add(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				Plan.ElementEllipses.Add(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				Plan.ElementPolygons.Add(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				Plan.ElementPolylines.Add(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
			else
				Toolbox.PlansViewModel.ElementAdded(elementBase);

			return Create(elementBase);
		}
		public void RemoveElement(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangle)
				Plan.ElementRectangles.Remove(designerItem.Element as ElementRectangle);
			else if (designerItem.Element is ElementEllipse)
				Plan.ElementEllipses.Remove(designerItem.Element as ElementEllipse);
			else if (designerItem.Element is ElementPolygon)
				Plan.ElementPolygons.Remove(designerItem.Element as ElementPolygon);
			else if (designerItem.Element is ElementPolyline)
				Plan.ElementPolylines.Remove(designerItem.Element as ElementPolyline);
			else if (designerItem.Element is ElementTextBlock)
				Plan.ElementTextBlocks.Remove(designerItem.Element as ElementTextBlock);
			else if (designerItem.Element is ElementSubPlan)
				Plan.ElementSubPlans.Remove(designerItem.Element as ElementSubPlan);
			else
				Toolbox.PlansViewModel.ElementRemoved(designerItem.Element);
			Remove(designerItem);
		}
		public void RemoveElement(ElementBase elementBase)
		{
			var designerItem = Items.FirstOrDefault(x => x.Element.UID == elementBase.UID);
			if (designerItem != null)
				RemoveElement(designerItem);
		}

		public DesignerItem Create(ElementBase elementBase)
		{
			var designerItem = DesignerItemFactory.Create(elementBase);
			Toolbox.PlansViewModel.RegisterDesignerItem(designerItem);
			Add(designerItem);
			designerItem.Redraw();
			return designerItem;
		}

		public override void Update()
		{
			Update(Plan);
		}
		private void Update(Plan plan)
		{
			CanvasWidth = plan.Width;
			CanvasHeight = plan.Height;
			if (plan.BackgroundPixels != null)
				CanvasBackground = PainterHelper.CreateBrush(plan.BackgroundPixels);
			else if (plan.BackgroundColor == Colors.Transparent)
				CanvasBackground = PainterHelper.CreateTransparentBrush(Zoom);
			else
				CanvasBackground = new SolidColorBrush(plan.BackgroundColor);
		}
		public override void Remove(List<Guid> elementUIDs)
		{
			foreach (var elementUID in elementUIDs)
			{
				var designerItem = Items.FirstOrDefault(x => x.Element.UID == elementUID);
				if (designerItem != null)
					RemoveElement(designerItem);
			}
		}

		public List<ElementBase> CloneElements(IEnumerable<DesignerItem> designerItems)
		{
			_initialElements = new List<ElementBase>();
			foreach (var designerItem in designerItems)
			{
				designerItem.UpdateElementProperties();
				_initialElements.Add(designerItem.Element.Clone());
			}
			return _initialElements;
		}

		public override void BeginChange(IEnumerable<DesignerItem> designerItems)
		{
			_initialElements = CloneElements(designerItems);
		}
		public override void BeginChange()
		{
			_initialElements = CloneElements(SelectedItems);
		}
		public override void EndChange()
		{
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(_initialElements);
			foreach (var designerItem in SelectedItems)
				designerItem.UpdateElementProperties();
		}

		public void UpdateZoom()
		{
			//if (Plan != null)
			//    Update();
			Toolbox.UpdateZoom();
			foreach (DesignerItem designerItem in Items)
			{
				designerItem.UpdateZoom();
				if (designerItem is DesignerItemPoint)
					designerItem.UpdateZoomPoint();
			}
		}
		public void UpdateZoomPoint()
		{
			foreach (DesignerItem designerItem in Items)
				if (designerItem is DesignerItemPoint)
					designerItem.UpdateZoomPoint();
		}
	}
}