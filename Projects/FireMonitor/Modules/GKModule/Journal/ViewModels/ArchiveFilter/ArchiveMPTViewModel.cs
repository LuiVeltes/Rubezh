﻿using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchiveMPTViewModel : CheckBoxItemViewModel
	{
		public ArchiveMPTViewModel(XMPT mpt)
		{
			MPT = mpt;
			Name = mpt.PresentationName;
		}

		public XMPT MPT { get; private set; }
		public string Name { get; private set; }
	}
}