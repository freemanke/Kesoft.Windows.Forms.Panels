using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Kesoft.Windows.Forms.Panels
{
    /// <summary>
    /// Represents a splitter control that enables the user to resize docked controls.
    /// </summary>
    /// <remarks>
    /// The splitter control supports in difference to the <see cref="System.Windows.Forms.Splitter"/> the using
    /// of a transparent backcolor.
    /// </remarks>
    [DesignTimeVisibleAttribute(true)]
	[ToolboxBitmap(typeof(System.Windows.Forms.Splitter))]
	public partial class Splitter : System.Windows.Forms.Splitter
	{
		#region MethodsPublic
		/// <summary>
        /// Initializes a new instance of the Splitter class.
		/// </summary>
		public Splitter()
		{
            //The System.Windows.Forms.Splitter doesn't suports a transparent backcolor
            //With this, the using of a transparent backcolor is possible
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			InitializeComponent();
            this.BackColor = Color.Transparent;
		}

		#endregion
	}
}
