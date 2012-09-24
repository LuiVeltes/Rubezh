﻿using System;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        public FiresecService.Service.FiresecService FiresecService { get; set; }
        public Guid UID { get; set; }
        public string IpAddress { get; set; }
        public ClientType ClientType { get; set; }
        public DateTime ConnectionDate { get; set; }

        public void BeginAddOperation(string operationName)
        {
            ClientToServerOperationName = operationName;
        }

        public void EndAddOperation()
        {
            ClientToServerOperationName = "";
        }

        string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        string _clientToServerOperationName;
        public string ClientToServerOperationName
        {
            get { return _clientToServerOperationName; }
            set
            {
                _clientToServerOperationName = value;
                OnPropertyChanged("ClientToServerOperationName");
            }
        }

        string _serverToClientOperationName;
        public string ServerToClientOperationName
        {
            get { return _serverToClientOperationName; }
            set
            {
                _serverToClientOperationName = value;
                OnPropertyChanged("ServerToClientOperationName");
            }
        }

        string _state;
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }
    }
}