using System;
using Xamarin.Forms;

namespace TimelyProjects.Helpers
{
    public class DefaultToolbarPage : ContentPage
    {
        public DefaultToolbarPage()
        {
            LoadToolbar();
        }

        private void LoadToolbar()
        {
            ToolbarItem profile = new ToolbarItem
            {
                Text = "My Profile",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0
            };
            ToolbarItem organization = new ToolbarItem
            {
                Text = "My Organization",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1
            };
            ToolbarItem signOut = new ToolbarItem
            {
                Text = "Sign Out",
                Order = ToolbarItemOrder.Secondary,
                Priority = 2
            };

            this.ToolbarItems.Add(profile);
            this.ToolbarItems.Add(organization);
            this.ToolbarItems.Add(signOut);
        }
    }
}
