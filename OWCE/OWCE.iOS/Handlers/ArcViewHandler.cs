using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using OWCE.Views;
using UIKit;

namespace OWCE.iOS.Handlers
{
    public class ArcViewHandler : ViewHandler<ArcView, UIView>
    {
        public static IPropertyMapper<ArcView, ArcViewHandler> Mapper = new PropertyMapper<ArcView, ArcViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ArcView.ArcColor)] = MapArcColor,
            [nameof(ArcView.CircleColor)] = MapCircleColor,
            [nameof(ArcView.Current)] = MapCurrent,
            [nameof(ArcView.Minimum)] = MapMinimum,
            [nameof(ArcView.Maximum)] = MapMaximum,
        };

        public ArcViewHandler() : base(Mapper) { }

        protected override UIView CreatePlatformView() => new ArcDrawingView(VirtualView);

        static void MapArcColor(ArcViewHandler handler, ArcView view) => handler.Invalidate();
        static void MapCircleColor(ArcViewHandler handler, ArcView view) => handler.Invalidate();
        static void MapCurrent(ArcViewHandler handler, ArcView view) => handler.Invalidate();
        static void MapMinimum(ArcViewHandler handler, ArcView view) => handler.Invalidate();
        static void MapMaximum(ArcViewHandler handler, ArcView view) => handler.Invalidate();

        void Invalidate() => (PlatformView as ArcDrawingView)?.SetNeedsDisplay();
    }

    internal class ArcDrawingView : UIView
    {
        readonly ArcView _arcView;
        static readonly nfloat PiOnOneEighty = (nfloat)(Math.PI / 180.0);

        public ArcDrawingView(ArcView arcView)
        {
            _arcView = arcView;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (_arcView == null) return;

            float range = _arcView.Maximum - _arcView.Minimum;
            if (range == 0) return;

            nfloat centerPoint = (nfloat)(0.5f * rect.Width);
            nfloat centerX = centerPoint;
            nfloat centerY = centerPoint;
            nfloat radius = centerPoint;
            var circleRect = new CGRect(0, 0, rect.Width, rect.Width);

            float currentPercent = (_arcView.Current - _arcView.Minimum) / range;
            currentPercent = Math.Max(0f, Math.Min(1.215f, currentPercent));

            nfloat startAngle = (nfloat)((177 + (186f * currentPercent)) * Math.PI / 180.0);
            nfloat endAngle = 43 * PiOnOneEighty;

            using var context = UIGraphics.GetCurrentContext();
            if (context == null) return;

            var circleColor = _arcView.CircleColor.ToPlatform();
            circleColor.SetFill();
            context.FillEllipseInRect(circleRect);

            var arcColor = _arcView.ArcColor.ToPlatform();
            arcColor.SetFill();
            context.AddArc(centerX, centerY, radius, startAngle, endAngle, true);
            context.AddLineToPoint(centerX, centerY);
            context.FillPath();
        }
    }
}
