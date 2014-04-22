﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class OrganisationFilterBase : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> OrganisationUIDs { get; set; }

		public OrganisationFilterBase()
			: base()
		{
			OrganisationUIDs = new List<Guid>();
		}
	}
}