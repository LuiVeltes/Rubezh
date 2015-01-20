using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using SKDModule.Events;
using Infrastructure.Common;

namespace SKDModule.Validation
{
	public class SlideWeeklyIntervalValidationError : ObjectValidationError<SKDSlideWeeklyInterval, ShowSKDSlideWeekIntervalsEvent, int>
	{
		public SlideWeeklyIntervalValidationError(SKDSlideWeeklyInterval interval, string error, ValidationErrorLevel level)
			: base(interval, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.SKD; }
		}
		protected override int Key
		{
			get { return Object.ID; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string Address
		{
			get { return Object.ID.ToString(); }
		}
		public override string ImageSource
		{
			get { return "Shedule"; }
		}
	}
}