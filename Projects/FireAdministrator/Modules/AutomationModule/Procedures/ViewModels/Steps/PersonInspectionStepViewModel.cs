﻿using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel: BaseViewModel, IStepViewModel
	{
		public ArithmeticParameterViewModel CardNumber { get; set; }
		public Procedure Procedure { get; private set; }

		public PersonInspectionStepViewModel(PersonInspectionArguments personInspectionArguments, Procedure procedure)
		{
			Procedure = procedure;
			var variableTypes = new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable, VariableType.IsValue };
			CardNumber = new ArithmeticParameterViewModel(personInspectionArguments.CardNumber, variableTypes);
			UpdateContent();
		}

		public void UpdateContent()
		{
			CardNumber.Update(Procedure.Variables);
		}

		public string Description { get { return ""; } }
	}
}