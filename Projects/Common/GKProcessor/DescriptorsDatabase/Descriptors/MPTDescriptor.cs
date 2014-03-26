﻿using System;
using System.Collections.Generic;
using Common;
using XFiresecAPI;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		public XPim HandAutomaticOffPim { get; private set; }
		public XPim DoorAutomaticOffPim { get; private set; }
		public XPim FailureAutomaticOffPim { get; private set; }

		public MPTDescriptor(GkDatabase gkDatabase, XMPT mpt)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.Hold = 10;
			MPT.DelayRegime = DelayRegime.On;

			HandAutomaticOffPim = new XPim();
			HandAutomaticOffPim.IsAutoGenerated = true;
			HandAutomaticOffPim.MPTUID = MPT.BaseUID;
			HandAutomaticOffPim.BaseUID = GuidHelper.CreateOn(MPT.BaseUID, 0);
			gkDatabase.AddPim(HandAutomaticOffPim);

			DoorAutomaticOffPim = new XPim();
			DoorAutomaticOffPim.IsAutoGenerated = true;
			DoorAutomaticOffPim.MPTUID = MPT.BaseUID;
			DoorAutomaticOffPim.BaseUID = GuidHelper.CreateOn(MPT.BaseUID, 1);
			gkDatabase.AddPim(DoorAutomaticOffPim);

			FailureAutomaticOffPim = new XPim();
			FailureAutomaticOffPim.IsAutoGenerated = true;
			FailureAutomaticOffPim.MPTUID = MPT.BaseUID;
			FailureAutomaticOffPim.BaseUID = GuidHelper.CreateOn(MPT.BaseUID, 2);
			gkDatabase.AddPim(FailureAutomaticOffPim);

			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			var hasOnExpression = false;
			if (MPT.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);
				hasOnExpression = true;
			}

			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						Formula.AddGetBit(XStateBit.TurningOn, MPT);
						Formula.Add(FormulaOperationType.AND);
						Formula.Add(FormulaOperationType.COM);
						if (hasOnExpression)
							Formula.Add(FormulaOperationType.AND);
						hasOnExpression = true;
					}
				}
			}
			if (hasOnExpression)
			{
				Formula.AddGetBit(XStateBit.Norm, MPT);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");

				Formula.AddGetBit(XStateBit.Norm, HandAutomaticOffPim);
				Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(XStateBit.Norm, DoorAutomaticOffPim);
				Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(XStateBit.Norm, FailureAutomaticOffPim);
				Formula.Add(FormulaOperationType.AND, comment: "");
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStart)
				{
					Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasOnExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOnExpression = true;
				}
			}

			if (hasOnExpression)
			{
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MPT);
			}

			var hasOffExpression = false;
			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						Formula.AddGetBit(XStateBit.TurningOn, MPT);
						Formula.Add(FormulaOperationType.AND);
						if (hasOffExpression)
							Formula.Add(FormulaOperationType.OR);
						hasOffExpression = true;
					}
				}
			}

			if (hasOffExpression)
			{
				Formula.AddGetBit(XStateBit.Norm, MPT);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");

				Formula.AddGetBit(XStateBit.Norm, HandAutomaticOffPim);
				Formula.Add(FormulaOperationType.AND, comment: "");

				//Formula.AddGetBit(XStateBit.Norm, DoorAutomaticOffPim);
				//Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(XStateBit.Norm, FailureAutomaticOffPim);
				Formula.Add(FormulaOperationType.AND, comment: "");
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStop)
				{
					Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasOffExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOffExpression = true;
				}
			}
			if (hasOffExpression)
			{
				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, MPT);
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
				Value = (ushort)MPT.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)MPT.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)MPT.DelayRegime
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