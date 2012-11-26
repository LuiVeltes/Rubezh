﻿using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonXZone : ElementBasePolygon, IElementZone, IPrimitive
	{
		[DataMember]
		public Guid ZoneUID { get; set; }

		public override ElementBase Clone()
		{
			ElementPolygonZone elementBase = new ElementPolygonZone()
			{
				ZoneUID = ZoneUID
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion

		private int _zlayer;
		public override int ZLayer
		{
			get { return _zlayer; }
		}
		public void SetZLayer(int zlayer)
		{
			_zlayer = zlayer;
		}
	}
}