﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class RolesViewModel : RegionViewModel, IEditingViewModel
    {
        public RolesViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);

            Roles = new ObservableCollection<RoleViewModel>();
            foreach (var role in FiresecManager.SecurityConfiguration.UserRoles)
                Roles.Add(new RoleViewModel(role));

            if (Roles.Count > 0)
                SelectedRole = Roles[0];
        }

        public ObservableCollection<RoleViewModel> Roles { get; private set; }

        RoleViewModel _selectedRole;
        public RoleViewModel SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged("SelectedRole");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var roleDetailsViewModel = new RoleDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(roleDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.UserRoles.Add(roleDetailsViewModel.Role);
                Roles.Add(new RoleViewModel(roleDetailsViewModel.Role));

                SecurityModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var result = DialogBox.DialogBox.Show(string.Format("Вы уверенны, что хотите удалить роль \"{0}\" из списка? Тогда будут удалены и все пользователи с этой ролью", SelectedRole.Role.Name),
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
                FiresecManager.SecurityConfiguration.Users = FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleId != SelectedRole.Role.Id).ToList();
                Roles.Remove(SelectedRole);

                SecurityModule.HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var roleDetailsViewModel = new RoleDetailsViewModel(SelectedRole.Role);
            if (ServiceFactory.UserDialogs.ShowModalWindow(roleDetailsViewModel))
            {
                RemovePermissionsFromUsersWithRole(SelectedRole.Role.Id, SelectedRole.Role.Permissions, roleDetailsViewModel.Role.Permissions);
                AddPermissionsToUsersWithRole(SelectedRole.Role.Id, SelectedRole.Role.Permissions, roleDetailsViewModel.Role.Permissions);

                FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
                SelectedRole.Role = roleDetailsViewModel.Role;
                FiresecManager.SecurityConfiguration.UserRoles.Add(SelectedRole.Role);

                SecurityModule.HasChanges = true;
            }
        }

        void RemovePermissionsFromUsersWithRole(UInt64 roleId, List<PermissionType> oldPermissions, List<PermissionType> newPermissions)
        {
            foreach (var permissionType in oldPermissions.Where(x => newPermissions.Contains(x) == false))
            {
                foreach (var user in FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleId == roleId))
                    user.Permissions.Remove(permissionType);
            }
        }

        void AddPermissionsToUsersWithRole(UInt64 roleId, List<PermissionType> oldPermissions, List<PermissionType> newPermissions)
        {
            foreach (var permissionType in newPermissions.Where(x => oldPermissions.Contains(x) == false))
            {
                foreach (var user in FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleId == roleId))
                    user.Permissions.Add(permissionType);
            }
        }

        bool CanEditDelete()
        {
            return SelectedRole != null;
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new RolesMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}