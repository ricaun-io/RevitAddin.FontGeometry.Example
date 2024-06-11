using System;
using System.Globalization;
using System.Windows.Media;

namespace RevitAddin.FontGeometry.Example.Services
{
    public class FontPathGeometryService
    {
        public FontFamily FontFamily { get; private set; }
        public System.Windows.FlowDirection FlowDirection { get; set; } = System.Windows.FlowDirection.LeftToRight;
        public System.Windows.TextAlignment TextAlignment { get; set; } = System.Windows.TextAlignment.Left;
        public System.Windows.FontWeight FontWeight { get; set; } = System.Windows.FontWeights.Normal;
        public System.Windows.FontStyle FontStyle { get; set; } = System.Windows.FontStyles.Normal;
        public System.Windows.FontStretch FontStretch { get; set; } = System.Windows.FontStretches.Normal;
        public double Tolerance { get; set; } = 0;

        public FontPathGeometryService(string fontName = "Arial")
        {
            FontFamily = new FontFamily(fontName);
        }

        public PathGeometry CreateText(string text, double fontSize = 100, System.Windows.Point point = default)
        {
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var foreground = System.Windows.Media.Brushes.Black;
            var formattedText = new System.Windows.Media.FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection,
                typeface, fontSize, foreground
#if NET47_OR_GREATER || NET
                , 1.25
#endif
                );

            formattedText.TextAlignment = TextAlignment;

            // Build the geometry object that represents the text.
            var textGeometry = formattedText.BuildGeometry(point);

            if (Tolerance == 0)
                return textGeometry.GetFlattenedPathGeometry();

            return textGeometry.GetFlattenedPathGeometry(Tolerance, ToleranceType.Absolute);
        }
    }
}
