﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Argument Argument { get; private set; }
		Procedure Procedure { get; set; }
		public ArgumentViewModel(Argument argument, Procedure procedure)
		{
			Argument = argument;
			Procedure = procedure;
			ObjectTypes = new ObservableCollection<ObjectType>
			{
				ObjectType.Card, ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone, ObjectType.Person, ObjectType.Plan,
				ObjectType.SKDDevice, ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
		}

		public string Name
		{
			get
			{
				var variable = Procedure.Arguments.FirstOrDefault(x => x.Uid == Argument.ArgumentUid);
				if (variable != null)
					return variable.Name;
				return "";
			}
		}

		public bool IsList
		{
			get { return Argument.IsList; }
			set
			{
				Argument.IsList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public bool BoolValue
		{
			get { return Argument.BoolValue; }
			set
			{
				Argument.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return Argument.DateTimeValue; }
			set
			{
				Argument.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return Argument.IntValue; }
			set
			{
				Argument.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get { return Argument.ObjectType; }
			set
			{
				Argument.ObjectType = value;
				OnPropertyChanged(() => ObjectType);
			}
		}

		public string StringValue
		{
			get { return Argument.StringValue; }
			set
			{
				Argument.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public VariableType VariableType
		{
			get { return Argument.VariableType; }
			set
			{
				Argument.VariableType = value;
				OnPropertyChanged(() => VariableType);
			}
		}
	}
}