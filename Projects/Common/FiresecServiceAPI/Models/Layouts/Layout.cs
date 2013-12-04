﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class Layout
	{
		public Layout()
		{
			UID = Guid.NewGuid();
			Users = new List<Guid>();
			Parts = new List<LayoutPart>();
			Caption = "Шаблон";
			SplitterSize = 4;
			SplitterColor = Colors.Transparent;
		}

		[DataMember]
		public List<Guid> Users { get; set; }
		[DataMember]
		public List<LayoutPart> Parts { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public int SplitterSize { get; set; }
		[DataMember]
		public Color SplitterColor { get; set; }
	}
}