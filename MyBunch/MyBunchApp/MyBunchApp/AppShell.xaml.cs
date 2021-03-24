using System;
using System.Collections.Generic;
using MyBunchApp.ViewModels;
using MyBunchApp.Views;
using Xamarin.Forms;

namespace MyBunchApp
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
			Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
		}

	}
}
