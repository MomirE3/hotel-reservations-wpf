using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HotelReservations.Model;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for Prices.xaml
    /// </summary>
    public partial class Prices : Window
    {
        private ICollectionView view;

        public Prices()
        {

            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            var priceService = new PriceService();
            var prices = priceService.GetAllPrices();

            view = CollectionViewSource.GetDefaultView(prices);
            view.Filter = DoFilter;

            PricesDG.ItemsSource = null;
            PricesDG.ItemsSource = view;
            PricesDG.IsSynchronizedWithCurrentItem = true;
            PricesDG.SelectedIndex = -1;
            view.Refresh();

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addPriceWindow = new AddEditPrice();

            Hide();
            if (addPriceWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private bool DoFilter(object roomObject)
        {
            var price = roomObject as Price;

            var roomTypeSearchParam = RoomTypeSearchTB.Text;

            if (price.RoomType.ToString().Contains(roomTypeSearchParam))
            {
                return true;
            }

            return false;
        }

        private void RoomTypeSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void PricesDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPrice = (Price)view.CurrentItem;

            if (selectedPrice != null)
            {
                var editPriceWindow = new AddEditPrice(selectedPrice);

                Hide();

                if (editPriceWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select price for edit. ");
                return;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var priceService = new PriceService();
            var selectedPriceToDelete = (Price)view.CurrentItem;

            if (selectedPriceToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this price? ", "Delete Price", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    priceService.DeletePrice(selectedPriceToDelete.Id);
                    FillData();
                }
            }

        }
    }
}
