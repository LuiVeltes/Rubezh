﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;
using FiresecAPI.AutomationCallback;

namespace FireMonitor.Layout.ViewModels
{
    public class MonitorLayoutShellViewModel : MonitorShellViewModel
    {
        private XmlLayoutSerializer _serializer;
        private AutoActivationViewModel _autoActivationViewModel;
        private SoundViewModel _soundViewModel;

        public MonitorLayoutShellViewModel(FiresecAPI.Models.Layouts.Layout layout)
            : base("Monitor.Layout")
        {
            Layout = layout;
            ChangeUserCommand = new RelayCommand(OnChangeUser, CanChangeUser);
            ChangeLayoutCommand = new RelayCommand<LayoutModel>(OnChangeLayout, CanChangeLayout);
        }

        public void UpdateLayout(LayoutModel layout)
        {
            Layout = layout;
            UpdateRibbon();
            Initialize();
            Manager.GridSplitterHeight = Layout.SplitterSize;
            Manager.GridSplitterWidth = Layout.SplitterSize;
            Manager.GridSplitterBackground = new SolidColorBrush(Layout.SplitterColor);
            Manager.BorderBrush = new SolidColorBrush(Layout.BorderColor);
            Manager.BorderThickness = new Thickness(Layout.BorderThickness);
            if (_serializer != null && Layout != null && !string.IsNullOrEmpty(Layout.Content))
                using (var tr = new StringReader(Layout.Content))
                    _serializer.Deserialize(tr);
            LoadLayoutProperties();
        }
        private void Initialize()
        {
            var list = new List<LayoutPartViewModel>();
            var map = new Dictionary<Guid, ILayoutPartPresenter>();
            foreach (var module in ApplicationService.Modules)
            {
                var monitorModule = module as MonitorLayoutModule;
                if (monitorModule != null)
                    monitorModule.MonitorLayoutShellViewModel = this;
                var layoutProviderModule = module as ILayoutProviderModule;
                if (layoutProviderModule != null)
                    foreach (var layoutPart in layoutProviderModule.GetLayoutParts())
                        map.Add(layoutPart.UID, layoutPart);
            }
            if (Layout != null)
                foreach (var layoutPart in Layout.Parts)
                    list.Add(new LayoutPartViewModel(layoutPart, map.ContainsKey(layoutPart.DescriptionUID) ? map[layoutPart.DescriptionUID] : new UnknownLayoutPartPresenter(layoutPart.DescriptionUID)));
            LayoutParts = new ObservableCollection<LayoutPartViewModel>(list);
        }
        private void LoadLayoutProperties()
        {
            var properties = FiresecManager.FiresecService.GetChangedProperties(Layout.UID);
            if (properties != null)
                foreach (var property in properties)
                {
                    var layoutPart = LayoutParts.FirstOrDefault(item => item.UID == property.LayoutPart);
                    if (layoutPart != null)
                        layoutPart.SetProperty(property.Property, property.Value);
                }
        }

        private ObservableCollection<LayoutPartViewModel> _layoutParts;
        public ObservableCollection<LayoutPartViewModel> LayoutParts
        {
            get { return _layoutParts; }
            set
            {
                if (_layoutParts != null)
                    _layoutParts.CollectionChanged -= LayoutPartsChanged;
                _layoutParts = value;
                _layoutParts.CollectionChanged += LayoutPartsChanged;
                OnPropertyChanged(() => LayoutParts);
            }
        }

        private LayoutPartViewModel _activeLayoutPart;
        public LayoutPartViewModel ActiveLayoutPart
        {
            get { return _activeLayoutPart; }
            set
            {
                _activeLayoutPart = value;
                OnPropertyChanged(() => ActiveLayoutPart);
            }
        }

        private DockingManager _manager;
        public DockingManager Manager
        {
            get { return _manager; }
            set
            {
                if (Manager != null)
                {
                    Manager.DocumentClosing -= LayoutPartClosing;
                    Manager.LayoutChanged -= LayoutChanged;
                }
                _manager = value;
                if (_serializer != null)
                {
                    _serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
                    _serializer = null;
                }
                if (_manager != null)
                {
                    Manager.LayoutChanged += LayoutChanged;
                    Manager.DocumentClosing += LayoutPartClosing;
                    Manager.LayoutUpdateStrategy = new LayoutUpdateStrategy();
                    _serializer = new XmlLayoutSerializer(Manager);
                    _serializer.LayoutSerializationCallback += LayoutSerializationCallback;
                    UpdateLayout(Layout);
                }
            }
        }

        private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Model.ContentId))
            {
                var layoutPart = LayoutParts.FirstOrDefault(item => item.UID == Guid.Parse(e.Model.ContentId));
                if (layoutPart != null)
                {
                    layoutPart.Margin = e.Model.Margin;
                    layoutPart.BorderColor = e.Model.BorderColor;
                    layoutPart.BorderThickness = e.Model.BorderThickness;
                    layoutPart.BackgroundColor = e.Model.BackgroundColor;
                }
                e.Content = layoutPart;
            }
        }
        private void LayoutChanged(object sender, EventArgs e)
        {
        }
        private void LayoutPartClosing(object sender, DocumentClosingEventArgs e)
        {
            var layoutPartViewModel = e.Document.Content as LayoutPartViewModel;
            if (layoutPartViewModel != null)
            {
                LayoutParts.Remove(layoutPartViewModel);
                e.Cancel = true;
            }
        }
        private void LayoutPartsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void UpdateRibbon()
        {
            if (Layout.IsRibbonEnabled)
            {
                RibbonContent = new RibbonMenuViewModel();
                var ribbonViewModel = new RibbonViewModel()
                {
                    Content = RibbonContent,
                };
                ribbonViewModel.PopupOpened += (s, e) => UpdateRibbonItems();
                HeaderMenu = ribbonViewModel;
                AddRibbonItem();
                AllowLogoIcon = false;
            }
            else
            {
                HeaderMenu = null;
                AllowLogoIcon = true;
            }
            RibbonVisible = false;
        }
        private void UpdateRibbonItems()
        {
            RibbonContent.Items[2][0].ImageSource = _autoActivationViewModel.IsAutoActivation ? "/Controls;component/Images/BWindowNormal.png" : "/Controls;component/Images/windowCross.png";
            RibbonContent.Items[2][0].ToolTip = _autoActivationViewModel.IsAutoActivation ? "Автоматическая активация ВКЛючена" : "Автоматическая активация ВЫКЛючена";
            RibbonContent.Items[2][0].Text = _autoActivationViewModel.IsAutoActivation ? "Выключить автоактивицию" : "Включить автоактивацию";
            RibbonContent.Items[2][1].ImageSource = _autoActivationViewModel.IsPlansAutoActivation ? "/Controls;component/Images/BMapOn.png" : "/Controls;component/Images/BMapOff.png";
            RibbonContent.Items[2][1].ToolTip = _autoActivationViewModel.IsPlansAutoActivation ? "Автоматическая активация планов ВКЛючена" : "Автоматическая активация планов ВЫКЛючена";
            RibbonContent.Items[2][1].Text = _autoActivationViewModel.IsPlansAutoActivation ? "Выключить автоактивицию плана" : "Включить автоактивацию плана";
            RibbonContent.Items[2][2].ImageSource = _soundViewModel.IsSoundOn ? "/Controls;component/Images/BSound.png" : "/Controls;component/Images/BMute.png";
            RibbonContent.Items[2][2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
            RibbonContent.Items[2][2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
        }
        private void AddRibbonItem()
        {
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить пользователя", ChangeUserCommand, "/Controls;component/Images/BUser.png"));

            var ip = ConnectionSettingsManager.IsRemote ? null : FiresecManager.GetIP();
            var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.HostNameOrAddressList.Contains(ip))).OrderBy(item => item.Caption);
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон", new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "/Controls;component/Images/BLayouts.png", item.Description))), "/Controls;component/Images/BLayouts.png"));

            _autoActivationViewModel = new AutoActivationViewModel();
            _soundViewModel = new SoundViewModel();
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Автоактивиция", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangeAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangePlansAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _soundViewModel.PlaySoundCommand) { IsNewGroup = true },
			}, "/Controls;component/Images/BConfig.png"));
            if (AllowClose)
                RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "/Controls;component/Images/BExit.png") { Order = int.MaxValue });
        }

        public RelayCommand ChangeUserCommand { get; private set; }
        private void OnChangeUser()
        {
            ApplicationService.ShutDown();
            Process.Start(Application.ResourceAssembly.Location);
        }
        private bool CanChangeUser()
        {
            return FiresecManager.CheckPermission(PermissionType.Oper_Logout);
        }

        public RelayCommand<LayoutModel> ChangeLayoutCommand { get; private set; }
        private void OnChangeLayout(LayoutModel layout)
        {
            ApplicationService.CloseAllWindows();
            UpdateLayout(layout);
        }
        private bool CanChangeLayout(LayoutModel layout)
        {
            return layout != Layout;
        }

        public void ListenAutomationEvents()
        {
            SafeFiresecService.AutomationEvent -= OnAutomationCallback;
            SafeFiresecService.AutomationEvent += OnAutomationCallback;
        }
        private void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
        {
            if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.GetVisualProperty || automationCallbackResult.AutomationCallbackType == AutomationCallbackType.SetVisualProperty)
            {
                var visuaPropertyData = (VisualPropertyData)automationCallbackResult.Data;
                var layoutPart = LayoutParts.FirstOrDefault(item => item.UID == visuaPropertyData.LayoutPart);
                if (layoutPart != null)
                {
                    var sendResponse = false;
                    object value = null;
                    ApplicationService.Invoke(() =>
                    {
                        switch (automationCallbackResult.AutomationCallbackType)
                        {
                            case AutomationCallbackType.GetVisualProperty:
                                value = layoutPart.GetProperty(visuaPropertyData.Property);
                                sendResponse = true;
                                break;
                            case AutomationCallbackType.SetVisualProperty:
                                layoutPart.SetProperty(visuaPropertyData.Property, visuaPropertyData.Value);
                                break;
                        }
                    });
                    if (sendResponse)
                        FiresecManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.ProcedureUID, value);
                }
            }
        }
    }
}