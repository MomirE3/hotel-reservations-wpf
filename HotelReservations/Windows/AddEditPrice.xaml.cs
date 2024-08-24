using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddEditPrice.xaml
    /// </summary>
    public partial class AddEditPrice : Window
    {
        private PriceService priceService;

        private Price contextPrice;
        public AddEditPrice(Price? price = null)
        {
            if (price == null)
            {
                contextPrice = new Price();
            }
            else
            {
                contextPrice = price.Clone();
            }

            InitializeComponent();
            priceService = new PriceService();

            AdjustWindow(price);

            this.DataContext = contextPrice;
        }

        public void AdjustWindow(Price? price = null)
        {
            priceService = new PriceService();
            var roomTypes = priceService.GetAllRoomTypes();
            RoomTypesCB.ItemsSource = roomTypes;

            var reservationTypes = Enum.GetValues(typeof(ReservationType));
            ReservationTypeCB.ItemsSource = reservationTypes;

            if (price != null)
            {
                Title = "Edit Price";
                RoomTypesCB.ItemsSource = new List<RoomType>{price.RoomType};
                RoomTypesCB.SelectedItem = price.RoomType;
                RoomTypesCB.IsEnabled = false;
                ReservationTypeCB.IsEnabled = false;
            }
            else
            {
                Title = "Add Price";

            }
            
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            var priceService = new PriceService();
            var prices = priceService.GetAllPrices().ToList();

            var existingPrice = prices.FirstOrDefault(
            p => p.Id != contextPrice.Id &&
            p.RoomType.Id == contextPrice.RoomType.Id &&
            p.ReservationType.ToString() == contextPrice.ReservationType.ToString());

            if (contextPrice.PriceValue < 1 || String.IsNullOrEmpty(contextPrice.PriceValue.ToString()))
            {
                MessageBox.Show("Please select a value for price.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (contextPrice.RoomType == null)
            {
                MessageBox.Show("Please select a room type! ");
                return;
            }

            foreach (var price in prices)
            {
                if(price.Id == contextPrice.Id)
                {
                    continue;
                }
                if (existingPrice != null)
                {
                    MessageBox.Show($"A price with {existingPrice.RoomType} and {existingPrice.ReservationType} already exists.",
                                    "Duplicate Price Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }    
                   
            priceService.SavePrice(contextPrice);

            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
