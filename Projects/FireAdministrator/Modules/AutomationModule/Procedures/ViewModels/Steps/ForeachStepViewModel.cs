﻿using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System;

namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseStepViewModel
	{
		public ForeachArguments ForeachArguments { get; private set; }
		public ArgumentViewModel ListParameter { get; private set; }
		public ArgumentViewModel ItemParameter { get; private set; }

		public ForeachStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ForeachArguments = stepViewModel.Step.ForeachArguments;
			ListParameter = new ArgumentViewModel(ForeachArguments.ListParameter, stepViewModel.Update, false);
			ListParameter.UpdateVariableHandler += UpdateItemVariable;
			ItemParameter = new ArgumentViewModel(ForeachArguments.ItemParameter, stepViewModel.Update, false);
			UpdateContent();
		}

		public override void UpdateContent()
		{
			ListParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ListParameter.SelectedVariable.ExplicitType
				&& x.ObjectType == ListParameter.SelectedVariable.SelectedObjectType && x.EnumType == ListParameter.SelectedVariable.SelectedEnumType));
		}

		public override string Description
		{
			get { return "Список: " + ListParameter.Description + " Элемент: " + ItemParameter.Description; }
		}

	}
}
