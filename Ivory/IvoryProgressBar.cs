using Ivory.Enums;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ivory
{
    public class IvoryProgressBar : Control
    {
        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

        private int minimum = 0;
        private int maximum = 100;
        private int currentValue = 0;

        private Orientation orientation = Orientation.Horizontal;

        private Color barColor = Color.FromArgb(217, 201, 194);
        private Color borderColor = Color.Black;
        private int borderThickness = 2;

        private TextStyleType textStyle = TextStyleType.TextAndPercentage;
        private readonly TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis;
        private Color textColor = Color.White;

        private CompositingMode compositingMode = CompositingMode.SourceOver;
        private CompositingQuality compositingQuality = CompositingQuality.HighQuality;
        private InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;
        private PixelOffsetMode pixelOffsetMode = PixelOffsetMode.HighQuality;
        private SmoothingMode smoothingMode = SmoothingMode.HighQuality;

        private bool hasErrors = false;
        private string errorLog = null;

        public IvoryProgressBar()
        {

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            ForeColor = barColor;
            BackColor = Color.FromArgb(251, 56, 33);
            OnForeColorChanged(EventArgs.Empty);
            Size = new Size(400, 23);
        }

        [Category("Action"),
        Description("Occurs when the ProgressBar's Value changed")]
        public event ValueChangedEventHandler ValueChanged;

        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            barColor = ForeColor;
            base.OnForeColorChanged(e);
        }

        [Description("The maximum value."),
        DefaultValue(100)]
        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = value;
                Invalidate();
            }
        }

        [Description("The minimum value."),
        DefaultValue(0)]
        public int Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                Invalidate();
            }
        }

        [Description("The current value."),
        DefaultValue(0)]
        public int Value
        {
            get => currentValue;
            set
            {
                if (value >= minimum && value <= maximum)
                {
                    currentValue = value;
                }
                else if (value > maximum)
                {
                    currentValue = maximum;
                }
                else if (value < minimum)
                {
                    currentValue = minimum;
                }

                Invalidate();

                OnValueChanged(new ValueChangedEventArgs(currentValue));
            }
        }

        [Description("The border color."),
        DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Description("The border thickness"),
        DefaultValue(2)]
        public int BorderThickness
        {
            get => borderThickness;
            set
            {
                borderThickness = value;
                Invalidate();
            }
        }

        [Description("The ProgressBar oritentation."),
        DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;
                Invalidate();
            }
        }

        [Description("The color of the text drawn on the ProgressBar."),
        DefaultValue(typeof(Color), "White")]
        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                Invalidate();
            }
        }

        [Description("The way the text on the ProgressBar is drawn."),
        DefaultValue(typeof(TextStyleType), "TextAndPercentage")]
        public TextStyleType TextStyle
        {
            get => textStyle;
            set
            {
                textStyle = value;
                Invalidate();
            }
        }

        [Description("The ProgressBar's Graphic's CompositingMode."),
        DefaultValue(typeof(CompositingMode), "SourceOver")]
        public CompositingMode CompositingMode
        {
            get => compositingMode;
            set
            {
                compositingMode = value;
                Invalidate();
            }
        }
        [Description("The ProgressBar's Graphic's CompositingQuality."),
        DefaultValue(typeof(CompositingQuality), "HighQuality")]
        public CompositingQuality CompositingQuality
        {
            get => compositingQuality;
            set
            {
                if (value != CompositingQuality.Invalid)
                {
                    compositingQuality = value;
                    Invalidate();
                }
            }
        }

        [Description("The ProgressBar's Graphic's InterpolationMode."),
        DefaultValue(typeof(InterpolationMode), "HighQualityBicubic")]
        public InterpolationMode InterpolationMode
        {
            get => interpolationMode;
            set
            {
                if (value != InterpolationMode.Invalid)
                {
                    interpolationMode = value;
                    Invalidate();
                }
            }
        }

        [Description("The ProgressBar's Graphic's PixelOffsetMode."),
        DefaultValue(typeof(PixelOffsetMode), "HighQuality")]
        public PixelOffsetMode PixelOffsetMode
        {
            get => pixelOffsetMode;
            set
            {
                if (value != PixelOffsetMode.Invalid)
                {
                    pixelOffsetMode = value;
                    Invalidate();
                }
            }
        }

        [Description("The ProgressBar's Graphic's SmoothingMode."),
        DefaultValue(typeof(SmoothingMode), "HighQuality")]
        public SmoothingMode SmoothingMode
        {
            get => smoothingMode;
            set
            {
                if (value != SmoothingMode.Invalid)
                {
                    smoothingMode = value;
                    Invalidate();
                }
            }
        }

        [Description("If any errors occur, this will contain the errors information. HasErrors will be set to true if any errors have occured")]
        public string ErrorLog
        {
            get => errorLog;
        }

        [Description("If any errors have occured, this will be set to true")]
        public bool HasErrors
        {
            get => hasErrors;
        }

        public void ClearErrors()
        {
            errorLog = "";
            hasErrors = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                if (Width <= 0 || Height <= 0)
                {
                    return;
                }

                pe.Graphics.CompositingMode = compositingMode;
                pe.Graphics.CompositingQuality = compositingQuality;
                pe.Graphics.InterpolationMode = interpolationMode;
                pe.Graphics.PixelOffsetMode = pixelOffsetMode;
                pe.Graphics.SmoothingMode = smoothingMode;

                if (currentValue != 0)
                {
                    using (SolidBrush brush = new SolidBrush(barColor))
                    {
                        if (orientation == Orientation.Vertical)
                        {
                            int scaledHeight = Height - Convert.ToInt32(((double)Height / maximum) * currentValue);
                            pe.Graphics.FillRectangle(brush, 0, scaledHeight, Width, Height);
                        }
                        else
                        {
                            int scaledWidth = Convert.ToInt32(((double)Width / maximum) * currentValue);
                            pe.Graphics.FillRectangle(brush, 0, 0, scaledWidth, Height);
                        }
                    }
                }

                if (textStyle != TextStyleType.None)
                {
                    if (textColor != Color.Transparent)
                    {
                        using (Font font = GetFont(Properties.Resources.Roboto_Black, 10.25F))
                        {
                            string txt = null;

                            if (textStyle == TextStyleType.Value)
                            {
                                txt = currentValue.ToString();
                            }
                            else if (textStyle == TextStyleType.ValueOverMaximum)
                            {
                                txt = string.Format("{0}/{1}", currentValue, maximum);
                            }
                            else if (textStyle == TextStyleType.Percentage && maximum != 0)
                            {
                                double p = Convert.ToDouble((100d / maximum) * Value);
                                txt = string.Format("{0}%", p);
                            }
                            else if (textStyle == TextStyleType.Text && !string.IsNullOrWhiteSpace(Text))
                            {
                                txt = Text;
                            }
                            else if (textStyle == TextStyleType.TextAndPercentage && !string.IsNullOrWhiteSpace(Text) && maximum != 0)
                            {
                                double p = Convert.ToDouble((100d / maximum) * Value);
                                txt = string.Format("{0}: {1}%", Text, p);
                            }
                            else if (textStyle == TextStyleType.TextAndValueOverMaximum && !string.IsNullOrWhiteSpace(Text))
                            {
                                txt = string.Format("{0}: {1}/{2}", Text, currentValue, maximum);
                            }

                            if (txt != null)
                            {
                                TextRenderer.DrawText(pe.Graphics, txt, font, new Rectangle(0, 0, Width, Height), textColor, flags);
                            }

                        }
                    }
                }

                if (borderThickness > 0)
                {
                    if (borderColor != Color.Transparent)
                    {
                        using (Pen pen = new Pen(borderColor, borderThickness))
                        {
                            pe.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorLog += "Error in OnPaint event\n " + "Message: " + ex.Message + "\n" + "Type: " + ex.GetType().ToString() + "\n";

                hasErrors = true;
            }

        }
        public static Font GetFont(byte[] fontbyte, float size)
        {
            using (PrivateFontCollection privateFontCollection = new PrivateFontCollection())
            {
                byte[] fnt = fontbyte;
                IntPtr buffer = Marshal.AllocCoTaskMem(fnt.Length);
                Marshal.Copy(fnt, 0, buffer, fnt.Length);
                privateFontCollection.AddMemoryFont(buffer, fnt.Length);
                return new Font(privateFontCollection.Families[0].Name, size);
            }
        }

        public class ValueChangedEventArgs : EventArgs
        {
            public ValueChangedEventArgs(int currentValue)
            {
                Value = currentValue;
            }

            public int Value { get; set; }
        }

    }
}
