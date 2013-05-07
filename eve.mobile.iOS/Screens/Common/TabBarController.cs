using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace eve.mobile.iOS.Screens.Common
{
	public class TabBarController : UITabBarController
	{
		UIViewController peopleScreen = null;
		UINavigationController peopleNav;

		public TabBarController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// home tab
			if (AppDelegate.IsPhone) {
					peopleScreen = new Screens.iPhone.HomeScreen();
				peopleScreen.Title = "iPhone Home";
			} else {
				peopleScreen = new Screens.iPad.HomeScreen();
				peopleScreen.Title = "iPad Home";
			}
			peopleNav = new UINavigationController();
			peopleNav.PushViewController(peopleScreen, false);
			peopleNav.Title = "res title";
			peopleNav.TabBarItem = new UITabBarItem("ciao", UIImage.FromBundle("Images/Tabs/schedule.png"), 0);

			UIViewController[] viewControllers;
			// create our array of controllers
			if (AppDelegate.IsPhone) {
				viewControllers = new UIViewController[]{
					peopleNav,
				};
			} else { // iPad
				viewControllers = new UIViewController[] {
 					peopleNav,
				};
			}

			// attach the view controllers
			ViewControllers = viewControllers;

			// tell the tab bar which controllers are allowed to customize. 
			// if we don't set  it assumes all controllers are customizable. 
			// if we set to empty array, NO controllers are customizable.
			CustomizableViewControllers = new UIViewController[] {};
			
			// set our selected item
			SelectedViewController = peopleNav;


		}
	}
}

