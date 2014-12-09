using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Kesoft.Windows.Forms.Panels
{
	/// <summary>
	/// Used to group collections of controls. 
	/// </summary>
    public interface IPanel
    {
        /// <summary>
        /// Gets or sets the style of the panel.
        /// </summary>
        Kesoft.Windows.Forms.Panels.PanelStyle PanelStyle { get; set;}
        /// <summary>
        /// Gets or sets the color schema which is used for the panel.
        /// </summary>
        Kesoft.Windows.Forms.Panels.ColorScheme ColorScheme { get; set;}
        /// <summary>
        /// Gets or sets a value indicating whether the control shows a border
        /// </summary>
        bool ShowBorder { get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether the expand icon in the caption bar is visible.
		/// </summary>
		bool ShowExpandIcon { get; set;}
		/// <summary>
		/// Gets or sets a value indicating whether the close icon in the caption bar is visible.
		/// </summary>
		bool ShowCloseIcon  { get; set;}
        /// <summary>
        /// Expands the panel or xpanderpanel.
        /// </summary>
        bool Expand { get; set; }
    }
}
