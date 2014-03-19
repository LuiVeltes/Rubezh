﻿using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganizationFilterBase : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> OrganizationUIDs { get; set; }

		public OrganizationFilterBase()
			: base()
		{
			OrganizationUIDs = new List<Guid>();
		}
	}
}