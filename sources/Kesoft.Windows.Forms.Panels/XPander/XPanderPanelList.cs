using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Kesoft.Windows.Forms.Panels.Properties;

namespace Kesoft.Windows.Forms.Panels
{

    #region Class XPanderPanelList

    /// <summary>
    /// Manages a related set of xpanderpanels.
    /// </summary>
    /// <remarks>
    /// The XPanderPanelList contains XPanderPanels, which are represented by XPanderPanel
    /// objects that you can add through the XPanderPanels property.
    /// The order of XPanderPanel objects reflects the order the xpanderpanels appear
    /// in the XPanderPanelList control.
    /// </remarks>
    [Designer(typeof (XPanderPanelListDesigner)),
     DesignTimeVisible(true)]
    [ToolboxBitmap(typeof (System.Windows.Forms.Panel))]
    public partial class XPanderPanelList : ScrollableControl
    {
        #region Events

        /// <summary>
        /// The PanelStyleChanged event occurs when PanelStyle flags have been changed.
        /// </summary>
        [Description("The PanelStyleChanged event occurs when PanelStyle flags have been changed.")]
        public event EventHandler<PanelStyleChangeEventArgs> PanelStyleChanged;

        /// <summary>
        /// The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.
        /// </summary>
        [Description("The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.")]
        public event EventHandler<EventArgs> CaptionStyleChanged;

        /// <summary>
        /// The ColorSchemeChanged event occurs when ColorScheme flags have been changed.
        /// </summary>
        [Description("The ColorSchemeChanged event occurs when ColorScheme flags have been changed.")]
        public event EventHandler<ColorSchemeChangeEventArgs> ColorSchemeChanged;

        /// <summary>
        /// Occurs when the value of the CaptionHeight property changes.
        /// </summary>
        [Description("Occurs when the value of the CaptionHeight property changes.")]
        public event EventHandler<EventArgs> CaptionHeightChanged;

        #endregion

        #region FieldsPrivate

        private bool m_bShowBorder;
        private bool m_bShowGradientBackground;
        private bool m_bShowExpandIcon;
        private bool m_bShowCloseIcon;
        private int m_iCaptionHeight;
        private LinearGradientMode m_linearGradientMode;
        private Color m_colorGradientBackground;
        private CaptionStyle m_captionStyle;
        private PanelStyle m_ePanelStyle;
        private ColorScheme m_eColorScheme;
        private readonly XPanderPanelCollection m_xpanderPanels;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of xpanderpanels in this xpanderpanellist.
        /// </summary>
        /// <example>The following code example creates a XPanderPanel and adds it to the XPanderPanels collection,
        /// <code>
        /// private void btnAddXPanderPanel_Click(object sender, EventArgs e)
        /// {
        ///     if (xPanderPanelList3 != null)
        ///     {
        ///         // Create and initialize a XPanderPanel.
        ///         Kesoft.Windows.Forms.Panels.XPanderPanel xpanderPanel = new Kesoft.Windows.Forms.Panels.XPanderPanel();
        ///         xpanderPanel.Text = "new XPanderPanel";
        ///         // and add it to the XPanderPanels collection
        ///         xPanderPanelList3.XPanderPanels.Add(xpanderPanel);
        ///     }
        /// }
        /// </code>
        /// </example>
        [RefreshProperties(RefreshProperties.Repaint),
         Category("Collections"),
         Browsable(true),
         Description("Collection containing all the XPanderPanels for the xpanderpanellist.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor(typeof (XPanderPanelCollectionEditor), typeof (UITypeEditor))]
        public XPanderPanelCollection XPanderPanels
        {
            get { return m_xpanderPanels; }
        }

        /// <summary>
        /// Specifies the style of the panels in this xpanderpanellist.
        /// </summary>
        [Description("Specifies the style of the xpanderpanels in this xpanderpanellist."),
         DefaultValue(PanelStyle.Default),
         Category("Appearance")]
        public PanelStyle PanelStyle
        {
            get { return m_ePanelStyle; }
            set
            {
                if (value != m_ePanelStyle)
                {
                    m_ePanelStyle = value;
                    OnPanelStyleChanged(this, new PanelStyleChangeEventArgs(m_ePanelStyle));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Panelcolors table.
        /// </summary>
        public PanelColors PanelColors { get; set; }

        /// <summary>
        /// Specifies the colorscheme of the xpanderpanels in the xpanderpanellist
        /// </summary>
        [Description("The colorscheme of the xpanderpanels in the xpanderpanellist")]
        [DefaultValue(ColorScheme.Professional)]
        [Category("Appearance")]
        public ColorScheme ColorScheme
        {
            get { return m_eColorScheme; }
            set
            {
                if (value != m_eColorScheme)
                {
                    m_eColorScheme = value;
                    OnColorSchemeChanged(this, new ColorSchemeChangeEventArgs(m_eColorScheme));
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the captionbar.
        /// </summary>
        [Description("The style of the captionbar.")]
        [Category("Appearance")]
        public CaptionStyle CaptionStyle
        {
            get { return m_captionStyle; }
            set
            {
                m_captionStyle = value;
                OnCaptionStyleChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// LinearGradientMode of the background in the xpanderpanellist 
        /// </summary>
        [Description("LinearGradientMode of the background in the xpanderpanellist"),
         DefaultValue(LinearGradientMode.Vertical),
         Category("Appearance")]
        public LinearGradientMode LinearGradientMode
        {
            get { return m_linearGradientMode; }
            set
            {
                if (value != m_linearGradientMode)
                {
                    m_linearGradientMode = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a xpanderpanellist's gradient background is shown. 
        /// </summary>
        [Description("Gets or sets a value indicating whether a xpanderpanellist's gradient background is shown."),
         DefaultValue(false),
         Category("Appearance")]
        public bool ShowGradientBackground
        {
            get { return m_bShowGradientBackground; }
            set
            {
                if (value != m_bShowGradientBackground)
                {
                    m_bShowGradientBackground = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a xpanderpanellist's border is shown
        /// </summary>
        [Description("Gets or sets a value indicating whether a xpanderpanellist's border is shown"),
         DefaultValue(true),
         Category("Appearance")]
        public bool ShowBorder
        {
            get { return m_bShowBorder; }
            set
            {
                if (value != m_bShowBorder)
                {
                    m_bShowBorder = value;
                    foreach (XPanderPanel xpanderPanel in XPanderPanels)
                    {
                        xpanderPanel.ShowBorder = m_bShowBorder;
                    }
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the expand icon of the xpanderpanels in this xpanderpanellist are visible.
        /// </summary>
        [Description("Gets or sets a value indicating whether the expand icon of the xpanderpanels in this xpanderpanellist are visible."),
         DefaultValue(false),
         Category("Appearance")]
        public bool ShowExpandIcon
        {
            get { return m_bShowExpandIcon; }
            set
            {
                if (value != m_bShowExpandIcon)
                {
                    m_bShowExpandIcon = value;
                    foreach (XPanderPanel xpanderPanel in XPanderPanels)
                    {
                        xpanderPanel.ShowExpandIcon = m_bShowExpandIcon;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the close icon of the xpanderpanels in this xpanderpanellist are visible.
        /// </summary>
        [Description("Gets or sets a value indicating whether the close icon of the xpanderpanels in this xpanderpanellist are visible."),
         DefaultValue(false),
         Category("Appearance")]
        public bool ShowCloseIcon
        {
            get { return m_bShowCloseIcon; }
            set
            {
                if (value != m_bShowCloseIcon)
                {
                    m_bShowCloseIcon = value;
                    foreach (XPanderPanel xpanderPanel in XPanderPanels)
                    {
                        xpanderPanel.ShowCloseIcon = m_bShowCloseIcon;
                    }
                }
            }
        }

        /// <summary>
        /// Gradientcolor background in this xpanderpanellist
        /// </summary>
        [Description("Gradientcolor background in this xpanderpanellist"),
         DefaultValue(false),
         Category("Appearance")]
        public Color GradientBackground
        {
            get { return m_colorGradientBackground; }
            set
            {
                if (value != m_colorGradientBackground)
                {
                    m_colorGradientBackground = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the XpanderPanels in this XPanderPanelList. 
        /// </summary>
        [Description("Gets or sets the height of the XpanderPanels in this XPanderPanelList. "),
         DefaultValue(25),
         Category("Appearance")]
        public int CaptionHeight
        {
            get { return m_iCaptionHeight; }
            set
            {
                if (value < Constants.CaptionMinHeight)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentUICulture,
                            Resources.IDS_InvalidOperationExceptionInteger, value, "CaptionHeight", Constants.CaptionMinHeight));
                }
                m_iCaptionHeight = value;
                OnCaptionHeightChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        /// Initializes a new instance of the XPanderPanelList class.
        /// </summary>
        public XPanderPanelList()
        {
            // Dieser Aufruf ist f�r den Windows Form-Designer erforderlich.
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            m_xpanderPanels = new XPanderPanelCollection(this);

            ShowBorder = true;
            PanelStyle = PanelStyle.Default;
            LinearGradientMode = LinearGradientMode.Vertical;
            CaptionHeight = 25;
        }

        /// <summary>
        /// Expands the specified XPanderPanel
        /// </summary>
        /// <param name="panel">The XPanderPanel to expand</param>
        /// <example>
        /// <code>
        /// private void btnExpandXPander_Click(object sender, EventArgs e)
        /// {
        ///    // xPanderPanel10 is not null
        ///    if (xPanderPanel10 != null)
        ///    {
        ///        Kesoft.Windows.Forms.Panels.XPanderPanelList panelList = xPanderPanel10.Parent as Kesoft.Windows.Forms.Panels.XPanderPanelList;
        ///        // and it's parent panelList is not null
        ///        if (panelList != null)
        ///        {
        ///            // expands xPanderPanel10 in it's panelList.
        ///            panelList.Expand(xPanderPanel10);
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        public void Expand(BasePanel panel)
        {
            if (panel == null)
            {
                throw new ArgumentNullException("panel",
                    string.Format(CultureInfo.InvariantCulture,
                        Resources.IDS_ArgumentException,
                        "panel"));
            }

            var xpanderPanel = panel as XPanderPanel;
            if (xpanderPanel != null)
            {
                foreach (XPanderPanel tmpXPanderPanel in m_xpanderPanels)
                {
                    if (tmpXPanderPanel.Equals(xpanderPanel) == false)
                    {
                        tmpXPanderPanel.Expand = false;
                    }
                }
                var propertyDescriptor = TypeDescriptor.GetProperties(xpanderPanel)["Expand"];
                if (propertyDescriptor != null)
                {
                    propertyDescriptor.SetValue(xpanderPanel, true);
                }
            }
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        /// Paints the background of the xpanderpanellist.
        /// </summary>
        /// <param name="pevent">A PaintEventArgs that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            if (m_bShowGradientBackground)
            {
                var rectangle = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
                using (var linearGradientBrush = new LinearGradientBrush(
                    rectangle,
                    BackColor,
                    GradientBackground,
                    LinearGradientMode))
                {
                    pevent.Graphics.FillRectangle(linearGradientBrush, rectangle);
                }
            }
        }

        /// <summary>
        /// Raises the ControlAdded event.
        /// </summary>
        /// <param name="e">A ControlEventArgs that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            var xpanderPanel = e.Control as XPanderPanel;
            if (xpanderPanel != null)
            {
                if (xpanderPanel.Expand)
                {
                    foreach (XPanderPanel tmpXPanderPanel in XPanderPanels)
                    {
                        if (tmpXPanderPanel != xpanderPanel)
                        {
                            tmpXPanderPanel.Expand = false;
                            tmpXPanderPanel.Height = xpanderPanel.CaptionHeight;
                        }
                    }
                }
                xpanderPanel.Parent = this;
                xpanderPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
                xpanderPanel.Left = Padding.Left;
                xpanderPanel.Width = ClientRectangle.Width
                                     - Padding.Left
                                     - Padding.Right;
                xpanderPanel.PanelStyle = PanelStyle;
                xpanderPanel.ColorScheme = ColorScheme;
                if (PanelColors != null)
                {
                    xpanderPanel.SetPanelProperties(PanelColors);
                }
                xpanderPanel.ShowBorder = ShowBorder;
                xpanderPanel.ShowCloseIcon = m_bShowCloseIcon;
                xpanderPanel.ShowExpandIcon = m_bShowExpandIcon;
                xpanderPanel.CaptionStyle = m_captionStyle;
                xpanderPanel.Top = GetTopPosition();
                xpanderPanel.PanelStyleChanged += XpanderPanelPanelStyleChanged;
                xpanderPanel.ExpandClick += XPanderPanelExpandClick;
                xpanderPanel.CloseClick += XPanderPanelCloseClick;
            }
            else
            {
                throw new InvalidOperationException("Can only add Kesoft.Windows.Forms.Panels.XPanderPanel");
            }
        }

        /// <summary>
        /// Raises the ControlRemoved event.
        /// </summary>
        /// <param name="e">A ControlEventArgs that contains the event data.</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            var xpanderPanel =
                e.Control as XPanderPanel;

            if (xpanderPanel != null)
            {
                xpanderPanel.PanelStyleChanged -= XpanderPanelPanelStyleChanged;
                xpanderPanel.ExpandClick -= XPanderPanelExpandClick;
                xpanderPanel.CloseClick -= XPanderPanelCloseClick;
            }
        }

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var iXPanderPanelCaptionHeight = 0;

            if (m_xpanderPanels != null)
            {
                foreach (XPanderPanel xpanderPanel in m_xpanderPanels)
                {
                    xpanderPanel.Width = ClientRectangle.Width
                                         - Padding.Left
                                         - Padding.Right;
                    if (xpanderPanel.Visible == false)
                    {
                        iXPanderPanelCaptionHeight -= xpanderPanel.CaptionHeight;
                    }
                    iXPanderPanelCaptionHeight += xpanderPanel.CaptionHeight;
                }

                foreach (XPanderPanel xpanderPanel in m_xpanderPanels)
                {
                    if (xpanderPanel.Expand)
                    {
                        xpanderPanel.Height =
                            Height
                            - iXPanderPanelCaptionHeight
                            - Padding.Top
                            - Padding.Bottom
                            + xpanderPanel.CaptionHeight;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the PanelStyle changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PanelStyleChangeEventArgs that contains the event data.</param>
        protected virtual void OnPanelStyleChanged(object sender, PanelStyleChangeEventArgs e)
        {
            var panelStyle = e.PanelStyle;
            Padding = new Padding(0);

            foreach (XPanderPanel xpanderPanel in XPanderPanels)
            {
                var propertyDescriptorCollection = TypeDescriptor.GetProperties(xpanderPanel);
                if (propertyDescriptorCollection.Count > 0)
                {
                    var propertyDescriptorPanelStyle = propertyDescriptorCollection["PanelStyle"];
                    if (propertyDescriptorPanelStyle != null)
                    {
                        propertyDescriptorPanelStyle.SetValue(xpanderPanel, panelStyle);
                    }
                    var propertyDescriptorLeft = propertyDescriptorCollection["Left"];
                    if (propertyDescriptorLeft != null)
                    {
                        propertyDescriptorLeft.SetValue(xpanderPanel, Padding.Left);
                    }
                    var propertyDescriptorWidth = propertyDescriptorCollection["Width"];
                    if (propertyDescriptorWidth != null)
                    {
                        propertyDescriptorWidth.SetValue(
                            xpanderPanel,
                            ClientRectangle.Width
                            - Padding.Left
                            - Padding.Right);
                    }
                }
            }
            if (PanelStyleChanged != null)
            {
                PanelStyleChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the ColorScheme changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnColorSchemeChanged(object sender, ColorSchemeChangeEventArgs e)
        {
            var eColorScheme = e.ColorSchema;
            foreach (XPanderPanel xpanderPanel in XPanderPanels)
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(xpanderPanel)["ColorScheme"];
                if (propertyDescriptor != null)
                {
                    propertyDescriptor.SetValue(xpanderPanel, eColorScheme);
                }
            }
            if (ColorSchemeChanged != null)
            {
                ColorSchemeChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the Caption Height changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnCaptionHeightChanged(object sender, EventArgs e)
        {
            foreach (XPanderPanel xpanderPanel in XPanderPanels)
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(xpanderPanel)["CaptionHeight"];
                if (propertyDescriptor != null)
                {
                    propertyDescriptor.SetValue(xpanderPanel, m_iCaptionHeight);
                }
            }
            if (CaptionHeightChanged != null)
            {
                CaptionHeightChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the CaptionStyleChanged changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnCaptionStyleChanged(object sender, EventArgs e)
        {
            foreach (XPanderPanel xpanderPanel in XPanderPanels)
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(xpanderPanel)["CaptionStyle"];
                if (propertyDescriptor != null)
                {
                    propertyDescriptor.SetValue(xpanderPanel, m_captionStyle);
                }
            }
            if (CaptionStyleChanged != null)
            {
                CaptionStyleChanged(sender, e);
            }
        }

        #endregion

        #region MethodsPrivate

        private void XPanderPanelExpandClick(object sender, EventArgs e)
        {
            var xpanderPanel = sender as XPanderPanel;
            if (xpanderPanel != null)
            {
                Expand(xpanderPanel);
            }
        }

        private void XPanderPanelCloseClick(object sender, EventArgs e)
        {
            var xpanderPanel = sender as XPanderPanel;
            if (xpanderPanel != null)
            {
                Controls.Remove(xpanderPanel);
            }
        }

        private void XpanderPanelPanelStyleChanged(object sender, PanelStyleChangeEventArgs e)
        {
            var panelStyle = e.PanelStyle;
            if (panelStyle != m_ePanelStyle)
            {
                PanelStyle = panelStyle;
            }
        }

        private int GetTopPosition()
        {
            var iTopPosition = Padding.Top;
            var iNextTopPosition = 0;

            //The next top position is the highest top value + that controls height, with a
            //little vertical spacing thrown in for good measure
            var enumerator = XPanderPanels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var xpanderPanel = (XPanderPanel) enumerator.Current;
                if (xpanderPanel != null && xpanderPanel.Visible)
                {
                    iTopPosition = iNextTopPosition == Padding.Top ? Padding.Top : iNextTopPosition;
                    iNextTopPosition = iTopPosition + xpanderPanel.Height;
                }
            }
            return iTopPosition;
        }

        #endregion
    }

    #endregion

    #region Class XPanderPanelListDesigner

    /// <summary>
    /// Extends the design mode behavior of a XPanderPanelList control that supports nested controls.
    /// </summary>
    internal class XPanderPanelListDesigner : ParentControlDesigner
    {
        #region FieldsPrivate

        private readonly Pen m_borderPen = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
        private XPanderPanelList m_xpanderPanelList;

        #endregion

        #region MethodsPublic

        /// <summary>
        /// nitializes a new instance of the XPanderPanelListDesigner class.
        /// </summary>
        public XPanderPanelListDesigner()
        {
            m_borderPen.DashStyle = DashStyle.Dash;
        }

        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The IComponent to associate with the designer.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            m_xpanderPanelList = (XPanderPanelList) Control;
            //Disable the autoscroll feature for the control during design time.  The control
            //itself sets this property to true when it initializes at run time.  Trying to position
            //controls in this control with the autoscroll property set to True is problematic.
            m_xpanderPanelList.AutoScroll = false;
        }

        /// <summary>
        /// This member overrides ParentControlDesigner.ActionLists
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                // Create action list collection
                var actionLists = new DesignerActionListCollection();

                // Add custom action list
                actionLists.Add(new XPanderPanelListDesignerActionList(Component));

                // Return to the designer action service
                return actionLists;
            }
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        /// Releases the unmanaged resources used by the XPanderPanelDesigner,
        /// and optionally releases the managed resources. 
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (m_borderPen != null)
                    {
                        m_borderPen.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Receives a call when the control that the designer is managing has painted its surface so the designer can
        /// paint any additional adornments on top of the xpanderpanel.
        /// </summary>
        /// <param name="e">A PaintEventArgs the designer can use to draw on the xpanderpanel.</param>
        protected override void OnPaintAdornments(PaintEventArgs e)
        {
            base.OnPaintAdornments(e);
            e.Graphics.DrawRectangle(m_borderPen, 0, 0, m_xpanderPanelList.Width - 2, m_xpanderPanelList.Height - 2);
        }

        #endregion
    }

    #endregion

    #region Class XPanderPanelListDesignerActionList

    /// <summary>
    /// Provides the class for types that define a list of items used to create a smart tag panel for the XPanderPanelList.
    /// </summary>
    public class XPanderPanelListDesignerActionList : DesignerActionList
    {
        #region Properties

        /// <summary>
        /// Gets a collecion of XPanderPanel objects
        /// </summary>
        [Editor(typeof (XPanderPanelCollectionEditor), typeof (UITypeEditor))]
        public XPanderPanelCollection XPanderPanels
        {
            get { return XPanderPanelList.XPanderPanels; }
        }

        /// <summary>
        /// Gets or sets the style of the panel.
        /// </summary>
        public PanelStyle PanelStyle
        {
            get { return XPanderPanelList.PanelStyle; }
            set { SetProperty("PanelStyle", value); }
        }

        /// <summary>
        /// Gets or sets the color schema which is used for the panel.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get { return XPanderPanelList.ColorScheme; }
            set { SetProperty("ColorScheme", value); }
        }

        /// <summary>
        /// Gets or sets the style of the caption (not for PanelStyle.Aqua).
        /// </summary>
        public CaptionStyle CaptionStyle
        {
            get { return XPanderPanelList.CaptionStyle; }
            set { SetProperty("CaptionStyle", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control shows a border.
        /// </summary>
        public bool ShowBorder
        {
            get { return XPanderPanelList.ShowBorder; }
            set { SetProperty("ShowBorder", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the expand icon is visible
        /// </summary>
        public bool ShowExpandIcon
        {
            get { return XPanderPanelList.ShowExpandIcon; }
            set { SetProperty("ShowExpandIcon", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the close icon is visible
        /// </summary>
        public bool ShowCloseIcon
        {
            get { return XPanderPanelList.ShowCloseIcon; }
            set { SetProperty("ShowCloseIcon", value); }
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        /// Initializes a new instance of the XPanderPanelListDesignerActionList class.
        /// </summary>
        /// <param name="component">A component related to the DesignerActionList.</param>
        public XPanderPanelListDesignerActionList(IComponent component)
            : base(component)
        {
            // Automatically display smart tag panel when
            // design-time component is dropped onto the
            // Windows Forms Designer
            base.AutoShow = true;
        }

        /// <summary>
        /// Returns the collection of DesignerActionItem objects contained in the list.
        /// </summary>
        /// <returns> A DesignerActionItem array that contains the items in this list.</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            // Create list to store designer action items
            var actionItems = new DesignerActionItemCollection();

            actionItems.Add(
                new DesignerActionMethodItem(
                    this,
                    "ToggleDockStyle",
                    GetDockStyleText(),
                    "Design",
                    "Dock or undock this control in it's parent container.",
                    true));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "ShowBorder",
                    "Show Border",
                    GetCategory(XPanderPanelList, "ShowBorder")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "ShowExpandIcon",
                    "Show ExpandIcon",
                    GetCategory(XPanderPanelList, "ShowExpandIcon")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "ShowCloseIcon",
                    "Show CloseIcon",
                    GetCategory(XPanderPanelList, "ShowCloseIcon")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "PanelStyle",
                    "Select PanelStyle",
                    GetCategory(XPanderPanelList, "PanelStyle")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "ColorScheme",
                    "Select ColorScheme",
                    GetCategory(XPanderPanelList, "ColorScheme")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "CaptionStyle",
                    "Select CaptionStyle",
                    GetCategory(XPanderPanelList, "CaptionStyle")));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "XPanderPanels",
                    "Edit XPanderPanels",
                    GetCategory(XPanderPanelList, "XPanderPanels")));

            return actionItems;
        }

        /// <summary>
        /// Dock/Undock designer action method implementation
        /// </summary>
        public void ToggleDockStyle()
        {
            // Toggle ClockControl's Dock property
            SetProperty("Dock", XPanderPanelList.Dock != DockStyle.Fill ? DockStyle.Fill : DockStyle.None);
        }

        #endregion

        #region MethodsPrivate

        /// <summary>
        /// Helper method that returns an appropriate display name for the Dock/Undock property,
        /// based on the ClockControl's current Dock property value
        /// </summary>
        /// <returns>the string to display</returns>
        private string GetDockStyleText()
        {
            if (XPanderPanelList.Dock == DockStyle.Fill)
            {
                return "Undock in parent container";
            }
            return "Dock in parent container";
        }

        private XPanderPanelList XPanderPanelList
        {
            get { return (XPanderPanelList) Component; }
        }

        // Helper method to safely set a component�s property
        private void SetProperty(string propertyName, object value)
        {
            // Get property
            var property
                = TypeDescriptor.GetProperties(XPanderPanelList)[propertyName];
            // Set property value
            property.SetValue(XPanderPanelList, value);
        }

        // Helper method to return the Category string from a
        // CategoryAttribute assigned to a property exposed by 
        //the specified object
        private static string GetCategory(object source, string propertyName)
        {
            var property = source.GetType().GetProperty(propertyName);
            var attribute = (CategoryAttribute) property.GetCustomAttributes(typeof (CategoryAttribute), false)[0];
            if (attribute == null)
            {
                return null;
            }
            return attribute.Category;
        }

        #endregion
    }

    #endregion

    #region Class XPanderPanelCollection

    /// <summary>
    /// Contains a collection of XPanderPanel objects.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public sealed class XPanderPanelCollection : IList
    {
        #region FieldsPrivate

        private readonly XPanderPanelList xpanderPanelList;
        private readonly Control.ControlCollection controlCollection;

        #endregion

        #region Constructor

        internal XPanderPanelCollection(XPanderPanelList xpanderPanelList)
        {
            this.xpanderPanelList = xpanderPanelList;
            controlCollection = xpanderPanelList.Controls;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a XPanderPanel in the collection. 
        /// </summary>
        /// <param name="index">The zero-based index of the XPanderPanel to get or set.</param>
        /// <returns>The xPanderPanel at the specified index.</returns>
        public XPanderPanel this[int index]
        {
            get { return (XPanderPanel) controlCollection[index]; }
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        /// Determines whether the XPanderPanelCollection contains a specific XPanderPanel
        /// </summary>
        /// <param name="xpanderPanel">The XPanderPanel to locate in the XPanderPanelCollection</param>
        /// <returns>true if the XPanderPanelCollection contains the specified value; otherwise, false.</returns>
        public bool Contains(XPanderPanel xpanderPanel)
        {
            return controlCollection.Contains(xpanderPanel);
        }

        /// <summary>
        /// Adds a XPanderPanel to the collection.  
        /// </summary>
        /// <param name="xpanderPanel">The XPanderPanel to add.</param>
        public void Add(XPanderPanel xpanderPanel)
        {
            controlCollection.Add(xpanderPanel);
            xpanderPanelList.Invalidate();
        }

        /// <summary>
        /// Removes the first occurrence of a specific XPanderPanel from the XPanderPanelCollection
        /// </summary>
        /// <param name="xpanderPanel">The XPanderPanel to remove from the XPanderPanelCollection</param>
        public void Remove(XPanderPanel xpanderPanel)
        {
            controlCollection.Remove(xpanderPanel);
        }

        /// <summary>
        /// Removes all the XPanderPanels from the collection. 
        /// </summary>
        public void Clear()
        {
            controlCollection.Clear();
        }

        /// <summary>
        /// Gets the number of XPanderPanels in the collection. 
        /// </summary>
        public int Count
        {
            get { return controlCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only. 
        /// </summary>
        public bool IsReadOnly
        {
            get { return controlCollection.IsReadOnly; }
        }

        /// <summary>
        /// Returns an enumeration of all the XPanderPanels in the collection.  
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return controlCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the specified XPanderPanel in the collection. 
        /// </summary>
        /// <param name="xpanderPanel">The xpanderPanel to find the index of.</param>
        /// <returns>The index of the xpanderPanel, or -1 if the xpanderPanel is not in the <see ref="ControlCollection">ControlCollection</see> instance.</returns>
        public int IndexOf(XPanderPanel xpanderPanel)
        {
            return controlCollection.IndexOf(xpanderPanel);
        }

        /// <summary>
        /// Removes the XPanderPanel at the specified index from the collection. 
        /// </summary>
        /// <param name="index">The zero-based index of the xpanderPanel to remove from the ControlCollection instance.</param>
        public void RemoveAt(int index)
        {
            controlCollection.RemoveAt(index);
        }

        /// <summary>
        /// Inserts an XPanderPanel to the collection at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted. </param>
        /// <param name="xpanderPanel">The XPanderPanel to insert into the Collection.</param>
        public void Insert(int index, XPanderPanel xpanderPanel)
        {
            ((IList) this).Insert(index, xpanderPanel);
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="xpanderPanels">The one-dimensional Array that is the destination of the elements copied from ICollection.
        /// The Array must have zero-based indexing.
        ///</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(XPanderPanel[] xpanderPanels, int index)
        {
            controlCollection.CopyTo(xpanderPanels, index);
        }

        #endregion

        #region Interface ICollection

        /// <summary>
        /// Gets the number of elements contained in the ICollection.
        /// </summary>
        int ICollection.Count
        {
            get { return Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) controlCollection).IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return ((ICollection) controlCollection).SyncRoot; }
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) controlCollection).CopyTo(array, index);
        }

        #endregion

        #region Interface IList

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns> The element at the specified index.</returns>
        object IList.this[int index]
        {
            get { return controlCollection[index]; }
            set { }
        }

        /// <summary>
        /// Adds an item to the IList.
        /// </summary>
        /// <param name="value">The Object to add to the IList.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        int IList.Add(object value)
        {
            var xpanderPanel = value as XPanderPanel;
            if (xpanderPanel == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture,
                    Resources.IDS_ArgumentException,
                    typeof (XPanderPanel).Name));
            }
            Add(xpanderPanel);
            return IndexOf(xpanderPanel);
        }

        /// <summary>
        /// Determines whether the IList contains a specific value.
        /// </summary>
        /// <param name="value">The Object to locate in the IList.</param>
        /// <returns>true if the Object is found in the IList; otherwise, false.</returns>
        bool IList.Contains(object value)
        {
            return Contains(value as XPanderPanel);
        }

        /// <summary>
        /// Determines the index of a specific item in the IList.
        /// </summary>
        /// <param name="value">The Object to locate in the IList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        int IList.IndexOf(object value)
        {
            return IndexOf(value as XPanderPanel);
        }

        /// <summary>
        /// Inserts an item to the IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to insert.</param>
        /// <param name="value">The Object to insert into the IList.</param>
        void IList.Insert(int index, object value)
        {
            if ((value is XPanderPanel) == false)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture,
                        Resources.IDS_ArgumentException,
                        typeof (XPanderPanel).Name));
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the IList.
        /// </summary>
        /// <param name="value">The Object to remove from the IList.</param>
        void IList.Remove(object value)
        {
            Remove(value as XPanderPanel);
        }

        /// <summary>
        /// Removes the IList item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        /// Gets a value indicating whether the IList is read-only.
        /// </summary>
        bool IList.IsReadOnly
        {
            get { return IsReadOnly; }
        }

        /// <summary>
        /// Gets a value indicating whether the IList has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get { return ((IList) controlCollection).IsFixedSize; }
        }

        #endregion
    }

    #endregion

    #region Class XPanderPanelCollectionEditor

    /// <summary>
    /// Provides a user interface that can edit most types of collections at design time.
    /// </summary>
    internal class XPanderPanelCollectionEditor : CollectionEditor
    {
        #region FieldsPrivate

        private CollectionForm m_collectionForm;

        #endregion

        #region MethodsPublic

        /// <summary>
        /// Initializes a new instance of the XPanderPanelCollectionEditor class
        /// using the specified collection type.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public XPanderPanelCollectionEditor(Type type)
            : base(type)
        {
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns> A CollectionEditor.CollectionForm to provide as the user interface for editing the collection.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            m_collectionForm = base.CreateCollectionForm();
            return m_collectionForm;
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="ItemType">The type of item to create.</param>
        /// <returns> A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type ItemType)
        {
            /* you can create the new instance yourself 
                 * ComplexItem ci=new ComplexItem(2,"ComplexItem",null);
                 * we know for sure that the itemType it will always be ComplexItem
                 *but this time let it to do the job... 
                 */

            var xpanderPanel =
                (XPanderPanel) base.CreateInstance(ItemType);

            if (Context.Instance != null)
            {
                xpanderPanel.Expand = true;
            }
            return xpanderPanel;
        }

        #endregion
    }

    #endregion
}