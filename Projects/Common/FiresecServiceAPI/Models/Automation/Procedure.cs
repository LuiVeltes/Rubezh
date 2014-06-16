﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Procedure
	{
		public Procedure()
		{
			Name = "Новая процедура";
			InputObjects = new List<ProcedureInputObject>();
			Steps = new List<ProcedureStep>();
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<ProcedureInputObject> InputObjects { get; set; }

		[DataMember]
		public List<ProcedureStep> Steps { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		public void Start()
		{
			
		}
	}
}