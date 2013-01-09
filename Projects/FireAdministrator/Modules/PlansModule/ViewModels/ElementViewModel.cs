﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class ElementViewModel : ElementBaseViewModel
	{
		public ElementViewModel(ObservableCollection<ElementBaseViewModel> sourceElement, DesignerItem designerItem)
		{
			Source = sourceElement;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			DesignerItem = designerItem;
			DesignerItem.TitleChanged += (s, e) => OnPropertyChanged("Name");
		}

		public DesignerItem DesignerItem { get; private set; }
		public string Name { get { return DesignerItem.Title; } }

		public bool IsVisible
		{
			get { return DesignerItem.IsVisibleLayout; }
			set
			{
				DesignerItem.IsVisibleLayout = value;
				OnPropertyChanged("IsVisible");
				((DesignerCanvas)DesignerItem.DesignerCanvas).Toolbox.SetDefault();
			}
		}

		public bool IsSelectable
		{
			get { return DesignerItem.IsSelectable; }
			set
			{
				DesignerItem.IsSelectable = value;
				OnPropertyChanged("IsSelectable");
				((DesignerCanvas)DesignerItem.DesignerCanvas).Toolbox.SetDefault();
			}
		}

		public override ContextMenu ContextMenu
		{
			get { return DesignerItem.GetContextMenu(); }
		}

		void OnShowOnPlan()
		{
			if (DesignerItem.IsSelectable)
				ServiceFactory.Events.GetEvent<ShowElementEvent>().Publish(DesignerItem.Element.UID);
		}
	}
}