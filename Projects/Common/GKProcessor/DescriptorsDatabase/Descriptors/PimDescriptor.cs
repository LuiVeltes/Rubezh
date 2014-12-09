﻿using FiresecAPI.GK;

namespace GKProcessor
{
	public class PimDescriptor : BaseDescriptor
	{
		public PimDescriptor(GKPim pim)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Pim;
			Pim = pim;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x107);
			SetAddress((ushort)0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			if (Formula == null)
			{
				Formula = new FormulaBuilder();
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
			}
		}
	}
}