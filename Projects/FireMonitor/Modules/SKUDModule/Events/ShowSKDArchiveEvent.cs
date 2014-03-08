﻿using FiresecAPI;
using Microsoft.Practices.Prism.Events;

namespace SKDModule.Events
{
	public class ShowSKDArchiveEvent : CompositePresentationEvent<ShowSKDArchiveEventArgs>
	{
	}

	public class ShowSKDArchiveEventArgs
	{
		public SKDDevice Device { get; set; }
		public SKDZone Zone { get; set; }
	}
}