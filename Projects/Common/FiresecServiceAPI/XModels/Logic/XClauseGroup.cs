﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class XClauseGroup
	{
		public XClauseGroup()
		{
			ClauseGroups = new List<XClauseGroup>();
			Clauses = new List<XClause>();
			ClauseJounOperationType = ClauseJounOperationType.Or;
		}

		public List<XClauseGroup> ClauseGroups { get; set; }
		public List<XClause> Clauses { get; set; }
		public ClauseJounOperationType ClauseJounOperationType { get; set; }

		public XClauseGroup Clone()
		{
			var result = new XClauseGroup();
			result.ClauseJounOperationType = ClauseJounOperationType;

			foreach (var clause in Clauses)
			{
				var clonedClause = new XClause()
				{
					ClauseConditionType = clause.ClauseConditionType,
					ClauseOperationType = clause.ClauseOperationType,
					StateType = clause.StateType,
					DeviceUIDs = clause.DeviceUIDs,
					ZoneUIDs = clause.ZoneUIDs,
					GuardZoneUIDs = clause.GuardZoneUIDs,
					DirectionUIDs = clause.DirectionUIDs,
					Devices = clause.Devices,
					Zones = clause.Zones,
					GuardZones = clause.GuardZones,
					Directions = clause.Directions,
				};
				result.Clauses.Add(clonedClause);
			}

			foreach (var clauseGroup in ClauseGroups)
			{
				result.ClauseGroups.Add(clauseGroup.Clone());
			}

			return result;
		}
	}
}