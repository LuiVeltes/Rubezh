﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Entities.DeviceOriented;

namespace VideoModule.ViewModels
{
	public class ChannelViewModel : BaseViewModel
	{
		public ChannelViewModel(int no, Channel channel)
		{
			No = no;
			Channel = channel;
			Name = channel.Name;
		}

		public ChannelViewModel(int no)
		{
			No = no;
			Name = "Канал " + no;
		}

		public Channel Channel { get; private set; }
		public string Name { get; private set; }
		public int No { get; private set; }
	}
}