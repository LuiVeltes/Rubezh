﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public Argument Argument { get; private set; }

		public ArgumentViewModel(Argument argument)
		{
			Argument = argument;
			ExplicitValue = new ExplicitValueViewModel(argument.ExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in argument.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
		}

		public ExplicitType ExplicitType
		{
			get { return Argument.ExplicitType; }
			set
			{
				Argument.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public EnumType EnumType
		{
			get { return Argument.EnumType; }
			set
			{
				Argument.EnumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObjectType ObjectType
		{
			get { return Argument.ObjectType; }
			set
			{
				Argument.ObjectType = value;
				OnPropertyChanged(() => ObjectType);
			}
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

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public new RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValue = new ExplicitValueViewModel();
			if (ExplicitType == ExplicitType.Object)
				ProcedureHelper.SelectObject(ObjectType, explicitValue);
			ExplicitValues.Add(explicitValue);
			Argument.ExplicitValues.Add(explicitValue.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public new RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			Argument.ExplicitValues.Remove(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public new RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var argumentDetailsViewModel = new ArgumentDetailsViewModel(Argument, IsList);
			if (DialogService.ShowModalWindow(argumentDetailsViewModel))
			{
				PropertyCopy.Copy<Argument, Argument>(argumentDetailsViewModel.Argument, Argument);
			}
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			if (IsList)
				ProcedureHelper.SelectObject(ObjectType, explicitValueViewModel);
			else
				ProcedureHelper.SelectObject(ObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
			OnPropertyChanged(() => ExplicitValue);
		}


		public string Description
		{
			get
			{
				var description = "";
				if (!IsList)
				{
					switch (ExplicitType)
					{
						case ExplicitType.Boolean:
							description = ExplicitValue.BoolValue.ToString();
							break;
						case ExplicitType.DateTime:
							description = ExplicitValue.DateTimeValue.ToString();
							break;
						case ExplicitType.Integer:
							description = ExplicitValue.IntValue.ToString();
							break;
						case ExplicitType.String:
							description = ExplicitValue.StringValue.ToString();
							break;
						case ExplicitType.Enum:
							{
								if (EnumType == EnumType.StateType)
									description = ExplicitValue.StateTypeValue.ToDescription();
								if (EnumType == EnumType.DriverType)
									description = ExplicitValue.DriverTypeValue.ToDescription();
							}
							break;
						case ExplicitType.Object:
							{
								description = ExplicitValue.PresentationName;
							}
							break;
					}
					return description;
				}
				else return "Список";
			}
		}
	}
}