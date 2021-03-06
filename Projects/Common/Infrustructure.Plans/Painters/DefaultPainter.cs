﻿using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		public DefaultPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			CalculateRectangle();
			return Rect.Size == Size.Empty ? DesignerCanvas.PainterCache.PointGeometry : base.CreateGeometry();
		}
		public override void Transform()
		{
			if (Geometry != DesignerCanvas.PainterCache.PointGeometry)
				base.Transform();
		}
		protected override Pen GetPen()
		{
			return null;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.BlackBrush;
		}
	}
}