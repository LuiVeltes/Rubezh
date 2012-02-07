﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FireAdministrator.Views;
using FiresecClient;

namespace FireAdministrator.ViewModels
{
    public class ProgressViewModel : DialogContent
    {
        public ProgressViewModel(string title)
        {
            StopCommand = new RelayCommand(OnStop);
            Title = title;
            FiresecCallbackService.ProgressEvent += new FiresecCallbackService.ProgressDelegate(Progress);
        }

        public void CloseProgress()
        {
            FiresecCallbackService.ProgressEvent -= new FiresecCallbackService.ProgressDelegate(Progress);
            Close(true);
        }

        public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            Description = comment;
            Percent = percentComplete;

            CancelText = "Отменить";
            if (stage == -100)
                CancelText = "Остановить";

            if (stage > 0)
            {
                int stageNo = stage / (256 * 256);
                int stageCount = stage - stageNo * 256 * 256;
            }

            return IsCanceled;
        }


        int _percent;
        public int Percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                OnPropertyChanged("Percent");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        string _cancelText;
        public string CancelText
        {
            get { return _cancelText; }
            set
            {
                _cancelText = value;
                OnPropertyChanged("CancelText");
            }
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            IsCanceled = true;
        }

        bool IsCanceled { get; set; }
    }
}