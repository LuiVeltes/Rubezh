﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Mail;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace NotificationModule.ViewModels
{
	public class NotificationViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public NotificationViewModel()
		{
			Menu = new NotificationMenuViewModel(this);
			TestCommand = new RelayCommand(OnTest, CanEditDelete);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			EditSenderCommand = new RelayCommand(OnEditSender);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			Emails = new ObservableCollection<EmailViewModel>();

			if (FiresecManager.SystemConfiguration.Emails == null ||
				FiresecManager.SystemConfiguration.Emails.Count == 0)
			{
				FiresecManager.SystemConfiguration.Emails = new List<Email>();
				AddSampleEmail();
			}

			foreach (var email in FiresecManager.SystemConfiguration.Emails)
			{
				var emailViewModel = new EmailViewModel(email);
				Emails.Add(emailViewModel);
			}

			SelectedEmail = Emails.FirstOrDefault();
		}

		ObservableCollection<EmailViewModel> _emails;

		public ObservableCollection<EmailViewModel> Emails
		{
			get { return _emails; }
			set
			{
				_emails = value;
				OnPropertyChanged("Emails");
			}
		}

		EmailViewModel _selectedEmail;

		public EmailViewModel SelectedEmail
		{
			get { return _selectedEmail; }
			set
			{
				_selectedEmail = value;
				OnPropertyChanged("SelectedEmail");
			}
		}

		private static void AddSampleEmail()
		{
			Email email = new Email
			{
				Address = "obychevma@rubezh.ru",
				Name = "Максим Обычев",
				States = new List<StateType>()
			};
			email.States.Add(StateType.Fire);
			email.States.Add(StateType.Failure);
			FiresecManager.SystemConfiguration.Emails.Add(email);
		}

		public RelayCommand TestCommand { get; private set; }

		private void OnTest()
		{
			string message = "Этот адресат уведомляется о состояниях " +
				SelectedEmail.PresenrationStates + " " +
				"в зонах " +
				"kjdgkljlfdjgl";
			MailHelper.Send(FiresecManager.SystemConfiguration.SenderParams, SelectedEmail.Email.Address,
							message,
							"Тестовое собщение Firesec-2");
		}

		public RelayCommand AddCommand { get; private set; }

		private void OnAdd()
		{
			var emailDetailsViewModel = new EmailDetailsViewModel();
			if (DialogService.ShowModalWindow(emailDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.Emails.Add(emailDetailsViewModel.EmailViewModel.Email);
				var emailViewModel = new EmailViewModel(emailDetailsViewModel.EmailViewModel.Email);
				Emails.Add(emailViewModel);
				SelectedEmail = emailViewModel;
				ServiceFactory.SaveService.CamerasChanged = true;
				//ServiceFactory.SaveService.EmailsChanged = true;
			}
		}

		private bool CanEditDelete()
		{
			return SelectedEmail != null;
		}

		public RelayCommand DeleteCommand { get; private set; }

		private void OnDelete()
		{
			FiresecManager.SystemConfiguration.Emails.Remove(SelectedEmail.Email);
			Emails.Remove(SelectedEmail);
			ServiceFactory.SaveService.CamerasChanged = true;
			//ServiceFactory.SaveService.EmailsChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }

		private void OnEdit()
		{
			var emailDetailsViewModel = new EmailDetailsViewModel(SelectedEmail.Email);
			if (DialogService.ShowModalWindow(emailDetailsViewModel))
			{
				SelectedEmail.Email = emailDetailsViewModel.EmailViewModel.Email;
				ServiceFactory.SaveService.CamerasChanged = true;
				//ServiceFactory.SaveService.EmailsChanged = true;
			}
		}

		public RelayCommand EditSenderCommand { get; private set; }

		private void OnEditSender()
		{
			if (FiresecManager.SystemConfiguration.SenderParams == null)
			{
				FiresecManager.SystemConfiguration.SenderParams = SenderParams.SetDefaultParams();
			}
			var senderDetailsViewModel = new SenderDetailsViewModel(FiresecManager.SystemConfiguration.SenderParams);
			if (DialogService.ShowModalWindow(senderDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.SenderParams = senderDetailsViewModel.SenderParamsViewModel.SenderParams;
				Trace.WriteLine(FiresecManager.SystemConfiguration.SenderParams.From);
				ServiceFactory.SaveService.CamerasChanged = true;
				//ServiceFactory.SaveService.EmailsChanged = true;
			}
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}