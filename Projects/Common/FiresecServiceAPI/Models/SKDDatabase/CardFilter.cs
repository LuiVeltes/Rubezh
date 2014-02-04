﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class CardFilter
    {
        [DataMember]
		public List<Guid> Uids { get; set; }
		
		public CardFilter()
        {
            Uids = new List<Guid>();
        }
    }
}