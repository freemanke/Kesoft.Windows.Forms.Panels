﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;
using Kesoft.Windows.Forms.Panels.Properties;

namespace Kesoft.Windows.Forms.Panels
{
    /// <summary>
    /// Represents a Windows progress bar control. 
    /// </summary>
    [ToolboxBitmap(typeof (System.Windows.Forms.ProgressBar))]
    public partial class ProgressBar : Control
    {
        #region Events

        /// <summary>
        /// Occurs when the value of the BorderColor property changes.
        /// </summary>
        [Description("Occurs when the value of the BorderColor property is changed on the control.")]
        public event EventHandler<EventArgs> BorderColorChanged;

        /// <summary>
        /// Occurs when the value of the BackgroundColor property changes.
        /// </summary>
        [Description("Occurs when the value of the BackgroundColor property is changed on the control.")]
        public event EventHandler<EventArgs> BackgroundColorChanged;

        /// <summary>
        /// Occurs when the value of the ValueColor property changes.
        /// </summary>
        [Description("Occurs when the value of the ValueColor property is changed on the control.")]
        public event EventHandler<EventArgs> ValueColorChanged;

        #endregion

        #region FieldsPrivate

        private Color m_backgroundColor;
        private Color m_valueColor;
        private Color m_borderColor;
        private int m_iMinimum;
        private int m_iMaximum;
        private int m_iValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color used for the background rectangle of this control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color"/>
        /// A Color used for the background rectangle of this control.
        /// </value>
        [Browsable(true)]
        [Description("The color used for the background rectangle of this control.")]
        public Color BackgroundColor
        {
            get { return m_backgroundColor; }
            set
            {
                if (m_backgroundColor != value)
                {
                    m_backgroundColor = value;
                    OnBackgroundColorChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used for the value rectangle of this control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color"/>
        /// A Color used for the value rectangle of this control.
        /// </value>
        [Browsable(true)]
        [Description("The color used for the value rectangle of this control.")]
        public Color ValueColor
        {
            get { return m_valueColor; }
            set
            {
                if (m_valueColor != value)
                {
                    m_valueColor = value;
                    OnValueColorChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Color "/>
        /// A Color that represents the border color of the control.
        /// </value>
        public Color BorderColor
        {
            get { return m_borderColor; }
            set
            {
                if (m_borderColor != value)
                {
                    m_borderColor = value;
                    OnBorderColorChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// The maximum value of the range. The default is 100.
        /// </value>
        [Browsable(true)]
        [Description("The upper bound of range this ProgressBar is working with.")]
        public int Maximum
        {
            get { return m_iMaximum; }
            set
            {
                if (m_iMaximum != value)
                {
                    if (value < 0)
                    {
                        var args = new object[] {"Maximum", value.ToString(CultureInfo.CurrentCulture), "Maximum"};
                        throw new ArgumentOutOfRangeException("Maximum", string.Format(CultureInfo.InvariantCulture, Resources.IDS_InvalidLowBoundArgument, args));
                    }
                    if (m_iMinimum > value)
                    {
                        m_iMinimum = value;
                    }
                    m_iMaximum = value;
                    if (m_iValue > m_iMaximum)
                    {
                        m_iValue = m_iMaximum;
                    }
                    UpdatePos();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// The minimum value of the range. The default is 0.
        /// </value>
        [Browsable(true)]
        [Description("The lower bound of range this ProgressBar is working with.")]
        public int Minimum
        {
            get { return m_iMinimum; }
            set
            {
                if (m_iMinimum != value)
                {
                    if (value < 0)
                    {
                        var args = new object[] {"Minimum", value.ToString(CultureInfo.CurrentCulture), "Minimum"};
                        throw new ArgumentOutOfRangeException("Minimum", string.Format(CultureInfo.InvariantCulture, Resources.IDS_InvalidLowBoundArgument, args));
                    }
                    if (m_iMaximum < value)
                    {
                        m_iMaximum = value;
                    }
                    m_iMinimum = value;
                    if (m_iValue < m_iMinimum)
                    {
                        m_iValue = m_iMinimum;
                    }
                    UpdatePos();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current position of the progress bar.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Int32"/>
        /// The position within the range of the progress bar. The default is 0.
        /// </value>
        [Browsable(true)]
        [Description("The current value for the ProgressBar, in the range specified by the minimum and maximum properties.")]
        public int Value
        {
            get { return m_iValue; }
            set
            {
                if (m_iValue != value)
                {
                    if ((value < m_iMinimum) || (value > m_iMaximum))
                    {
                        throw new ArgumentOutOfRangeException("Value", string.Format(CultureInfo.InvariantCulture, Resources.IDS_InvalidBoundArgument, new object[] {"Value", value.ToString(CultureInfo.CurrentCulture), "'minimum'", "'maximum'"}));
                    }
                    m_iValue = value;
                    UpdatePos();
                }
            }
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        public ProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();

            m_iMaximum = 100;
            m_backgroundColor = Color.FromArgb(20, 20, 255);
            m_valueColor = Color.FromArgb(255, 0, 255);
            m_borderColor = SystemColors.ActiveBorder;
            BackColor = Color.Transparent;
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            using (var antiAlias = new UseAntiAlias(e.Graphics))
            {
                var graphics = e.Graphics;
                DrawProgressBar(
                    graphics,
                    ClientRectangle,
                    m_backgroundColor,
                    m_valueColor,
                    m_borderColor,
                    RightToLeft,
                    Minimum,
                    Maximum,
                    Value);

                if (string.IsNullOrEmpty(Text) == false)
                {
                    using (var useClearTypeGridFit = new UseClearTypeGridFit(graphics))
                    {
                        using (var textBrush = new SolidBrush(ForeColor))
                        {
                            using (var stringFormat = new StringFormat())
                            {
                                stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                                if (RightToLeft == RightToLeft.Yes)
                                {
                                    stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                                }
                                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                                stringFormat.LineAlignment = StringAlignment.Center;
                                stringFormat.Alignment = StringAlignment.Center;

                                var stringRectangle = ClientRectangle;
                                graphics.DrawString(Text, Font, textBrush, stringRectangle, stringFormat);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the BorderColor changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnBorderColorChanged(object sender, EventArgs e)
        {
            Invalidate(true);
            if (BorderColorChanged != null)
            {
                BorderColorChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the BackgroundColor changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnBackgroundColorChanged(object sender, EventArgs e)
        {
            Invalidate();
            if (BackgroundColorChanged != null)
            {
                BackgroundColorChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the ValueColor changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnValueColorChanged(object sender, EventArgs e)
        {
            Invalidate(true);
            if (ValueColorChanged != null)
            {
                ValueColorChanged(sender, e);
            }
        }

        #endregion

        #region MethodsPrivate

        private void UpdatePos()
        {
            Invalidate(true);
        }

        private static void DrawProgressBar(
            Graphics graphics,
            Rectangle clientRectangle,
            Color colorBackgroundEnd,
            Color colorValueEnd,
            Color borderColor,
            RightToLeft rightToLeft,
            int iMinimum,
            int iMaximum,
            int iValue)
        {
            var outerRectangle = GetRectangleBackground(clientRectangle);

            using (var outerRectangleGraphicsPath = GetBackgroundPath(outerRectangle, 4))
            {
                if (outerRectangleGraphicsPath != null)
                {
                    using (var gradientBrush = GetGradientBackBrush(outerRectangle, colorBackgroundEnd))
                    {
                        if (gradientBrush != null)
                        {
                            graphics.FillPath(gradientBrush, outerRectangleGraphicsPath);
                        }
                    }

                    // Draws the value rectangle
                    if (iValue > 0)
                    {
                        var valueRectangle = GetRectangleValue(outerRectangle, rightToLeft, iMinimum, iMaximum, iValue);
                        using (var valueGraphicsPath = GetValuePath(valueRectangle, rightToLeft, 5))
                        {
                            using (var gradientBrush = GetGradientBackBrush(valueRectangle, colorValueEnd))
                            {
                                if (gradientBrush != null)
                                {
                                    graphics.FillPath(gradientBrush, valueGraphicsPath);
                                }
                            }
                        }
                    }
                    using (var borderPen = new Pen(borderColor))
                    {
                        graphics.DrawPath(borderPen, outerRectangleGraphicsPath);
                    }
                }
            }
        }

        private static Rectangle GetRectangleBackground(Rectangle clientRectangle)
        {
            var rectangleBackground = clientRectangle;
            rectangleBackground.Inflate(-1, -1);
            return rectangleBackground;
        }

        private static Rectangle GetRectangleValue(Rectangle backgroundRectangle, RightToLeft rightToLeft, int iMinimum, int iMaximum, int iValue)
        {
            var valueRectangle = backgroundRectangle;
            var iProgressRange = iMaximum - iMinimum;
            var iValueRange = iValue - iMinimum;
            var iRange = (int) ((float) iValueRange/(float) iProgressRange*backgroundRectangle.Width);
            valueRectangle.Width = iRange;
            if (rightToLeft == RightToLeft.Yes)
            {
                valueRectangle.X = backgroundRectangle.Width - valueRectangle.Width;
            }
            return valueRectangle;
        }

        private static GraphicsPath GetBackgroundPath(Rectangle bounds, int radius)
        {
            var x = bounds.X;
            var y = bounds.Y;
            var width = bounds.Width;
            var height = bounds.Height;
            var graphicsPath = new GraphicsPath();
            graphicsPath.AddArc(x, y, radius, radius, 180, 90); //Upper left corner
            graphicsPath.AddArc(x + width - radius, y, radius, radius, 270, 90); //Upper right corner
            graphicsPath.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90); //Lower right corner
            graphicsPath.AddArc(x, y + height - radius, radius, radius, 90, 90); //Lower left corner
            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        private static GraphicsPath GetValuePath(Rectangle bounds, RightToLeft rightToLeft, int radius)
        {
            var x = bounds.X;
            var y = bounds.Y;
            var width = bounds.Width;
            var height = bounds.Height;
            var graphicsPath = new GraphicsPath();
            if (rightToLeft == RightToLeft.No)
            {
                graphicsPath.AddArc(x, y, radius, radius, 180, 90); //Upper left corner
                graphicsPath.AddLine(x + radius, y, x + width, y); //Upper line
                graphicsPath.AddLine(x + width, y, x + width, y + height); //Right line
                graphicsPath.AddArc(x, y + height - radius, radius, radius, 90, 90); //Lower left corner
            }
            else
            {
                graphicsPath.AddLine(x, y, width - radius, y); //Upper Line
                graphicsPath.AddArc(x + width - radius, y, radius, radius, 270, 90); // Upper right corner
                graphicsPath.AddLine(x + width, y + radius, x + width, y + radius + height - (2*radius)); // right line
                graphicsPath.AddArc(x + width - radius, y + radius + height - (2*radius), radius, radius, 360, 90); // Lower right corner
                graphicsPath.AddLine(x + width - radius, y + height, x, y + height); // Lower line
            }
            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        private static LinearGradientBrush GetGradientBackBrush(Rectangle bounds, Color backColor)
        {
            if (IsZeroWidthOrHeight(bounds))
            {
                return null;
            }
            var linearGradientBrush = new LinearGradientBrush(bounds, Color.White, backColor, LinearGradientMode.Vertical);
            if (linearGradientBrush != null)
            {
                var blend = new Blend();
                blend.Positions = new float[] {0.0F, 0.2F, 0.3F, 0.5F, 0.6F, 0.8F, 1.0F};
                blend.Factors = new float[] {0.3F, 0.4F, 0.5F, 0.8F, 1.0F, 1.0F, 0.9F};
                linearGradientBrush.Blend = blend;
            }
            return linearGradientBrush;
        }

        /// <summary>
        /// Checks if the rectangle width or height is equal to 0.
        /// </summary>
        /// <param name="rectangle">the rectangle to check</param>
        /// <returns>true if the with or height of the rectangle is 0 else false</returns>
        private static bool IsZeroWidthOrHeight(Rectangle rectangle)
        {
            if (rectangle.Width != 0)
            {
                return (rectangle.Height == 0);
            }
            return true;
        }

        #endregion
    }
}