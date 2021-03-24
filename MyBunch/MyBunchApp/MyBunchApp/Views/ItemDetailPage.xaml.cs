using System.ComponentModel;
using Xamarin.Forms;
using MyBunchApp.ViewModels;

namespace MyBunchApp.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		public ItemDetailPage()
		{
			InitializeComponent();
			BindingContext = new ItemDetailViewModel();
		}
	}
}