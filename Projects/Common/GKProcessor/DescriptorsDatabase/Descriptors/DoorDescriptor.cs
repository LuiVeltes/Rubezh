﻿using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DoorDescriptor : BaseDescriptor
	{
		public DoorDescriptor(GKDoor door)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Door;
			Door = door;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x104);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (Door.EnterDevice != null || Door.ExitDevice != null)
			{
				if (Door.EnterDevice != null)
				{
					Formula.AddGetBit(GKStateBit.Attention, Door.EnterDevice);
					Formula.Add(FormulaOperationType.ACS, (byte)Door.EnterLevel, (ushort)Door.GKDescriptorNo);
					Formula.Add(FormulaOperationType.AND);
				}
				if (Door.ExitDevice != null)
				{
					if (Door.ExitDevice.DriverType == GKDriverType.RSR2_CodeReader)
					{
						Formula.AddGetBit(GKStateBit.Attention, Door.ExitDevice);
						Formula.Add(FormulaOperationType.ACS, (byte)Door.EnterLevel, (ushort)Door.GKDescriptorNo);
						Formula.Add(FormulaOperationType.AND);
					}
					else
					{
						Formula.AddGetBit(GKStateBit.Fire1, Door.ExitDevice);
					}
				}
				if (Door.EnterDevice != null && Door.ExitDevice != null)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.TurningOn, Door);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = (ushort)(Door.Delay)
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)(Door.Hold)
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}