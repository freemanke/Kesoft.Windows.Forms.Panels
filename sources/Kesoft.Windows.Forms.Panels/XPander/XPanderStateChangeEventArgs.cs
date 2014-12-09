using System;

namespace Kesoft.Windows.Forms.Panels
{
    /// <summary>
    /// Provides data for the XPanderStateChange event.
    /// </summary>
	public class XPanderStateChangeEventArgs : EventArgs
	{
		#region FieldsPrivate
		
		private bool m_bExpand;
		
		#endregion
		
		#region Properties
		
		/// <summary>
        /// Gets a value indicating whether the panel expands.
		/// </summary>
        public bool Expand
		{
			get {return m_bExpand;}
		}
		
		#endregion

		#region MethodsPublic
        /// <summary>
        /// arguments used when a XPanderStateChange event occurs.
        /// </summary>
        /// <param name="bExpand">Gets a value indicating whether the panel expands.</param>
		public XPanderStateChangeEventArgs(bool bExpand)
		{
			this.m_bExpand = bExpand;
		}

		#endregion
	}
}
