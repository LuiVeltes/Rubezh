﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public bool IsEditMode { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
			ExplicitValue = new ExplicitValueViewModel(variable.DefaultExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in variable.DefaultExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
			OnPropertyChanged(() => ExplicitValues);
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);

		}

		public bool IsList
		{
			get { return Variable.IsList; }
			set
			{
				Variable.IsList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public string Name
		{
			get { return Variable.Name; }
			set
			{
				Variable.Name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public ExplicitType ExplicitType
		{
			get { return Variable.ExplicitType; }
			set
			{
				Variable.ExplicitType = value;
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType SelectedEnumType
		{
			get { return Variable.EnumType; }
			set
			{
				Variable.EnumType = value;
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType SelectedObjectType
		{
			get { return Variable.ObjectType; }
			set
			{
				Variable.ObjectType = value;
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => IsList);
			OnPropertyChanged(() => ExplicitValue);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValueViewModel = new ExplicitValueViewModel(new ExplicitValue());
			if (ExplicitType == ExplicitType.Object)
				ProcedureHelper.SelectObject(SelectedObjectType, explicitValueViewModel);
			ExplicitValues.Add(explicitValueViewModel);
			Variable.DefaultExplicitValues.Add(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			Variable.DefaultExplicitValues.Remove(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			if (IsList)
				ProcedureHelper.SelectObject(SelectedObjectType, explicitValueViewModel);
			else
				ProcedureHelper.SelectObject(SelectedObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
			OnPropertyChanged(() => ExplicitValue);
		}
	}
}