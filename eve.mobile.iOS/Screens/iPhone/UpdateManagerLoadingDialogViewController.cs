using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using eve.mobile.Core;
//using eve.mobile.BL;

namespace eve.mobile.iOS.Screens.iPhone {
	/// <summary>
	/// Base class for loading screens
	/// </summary>
	/// <remarks>
	/// This ViewController implements the data loading via a virtual
	/// method LoadData(), which must call StopLoadingScreen()
	/// </remarks>
	public partial class UpdateManagerLoadingDialogViewController : DialogViewController {
		UI.Controls.LoadingOverlay loadingOverlay;

		/// <summary>
		/// Set pushing=true so that the UINavCtrl 'back' button is enabled
		/// </summary>
		public UpdateManagerLoadingDialogViewController () : base (UITableViewStyle.Plain, null, true)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
//			BL.Managers.UpdateManager.UpdateFinished += HandleUpdateFinished;
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

//			if(BL.Managers.UpdateManager.IsUpdating) {
//				if (loadingOverlay == null) {
//					var bounds = new RectangleF(0,0,768,1004);
//					if (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft
//					|| InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
//						bounds = new RectangleF(0,0,1024,748);	
//					} 
//
//					loadingOverlay = new eve.mobile.iOS.UI.Controls.LoadingOverlay(bounds);
//					// because DialogViewController is a UITableViewController,
//					// we need to step OVER the UITableView, otherwise the loadingOverlay
//					// sits *in* the scrolling area of the table
//					View.Superview.Add (loadingOverlay); 
//					View.Superview.BringSubviewToFront (loadingOverlay);
//				}
//				ConsoleD.WriteLine("Waiting for updates to finish before displaying table.");
//			} else {
//				loadingOverlay = null;
//				if (AlwaysRefresh || Root == null || Root.Count == 0) {
//					ConsoleD.WriteLine("Not updating, populating table.");
//					PopulateTable();
//				} else ConsoleD.WriteLine("Data already populated.");
//			}
		}
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
//			BL.Managers.UpdateManager.UpdateFinished -= HandleUpdateFinished; 
		}
		void HandleUpdateFinished(object sender, EventArgs e)
		{
			ConsoleD.WriteLine("Updates finished, going to populate table.");
//			InvokeOnMainThread ( () => {
//				PopulateTable ();
//				if (loadingOverlay != null)
//					loadingOverlay.Hide ();
//				loadingOverlay = null;
//			});
		}
		
		/// <summary>
		/// Your implementation should get data from the UpdateManager 
		/// and set the Root for the DialogViewController
		/// </summary>
		protected virtual void PopulateTable()
		{
		}

		/// <summary>
		/// Whether the table will be reloaded on ViewWillAppear.
		/// </summary>
		protected bool AlwaysRefresh { get; set; }
	}
}