using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimelyProjects.Helpers;
using TimelyProjects.ViewModels;
using Xamarin.Forms;

namespace TimelyProjects.Views
{
    public partial class MyHome : DefaultToolbarPage
    {
        public MyHome()
        {
            BindingContext = new MyHomeViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = (BindingContext as MyHomeViewModel).LoadData();
        }
    }
}
