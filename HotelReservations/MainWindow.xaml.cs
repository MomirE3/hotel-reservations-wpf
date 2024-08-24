using HotelReservations.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelReservations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RoomsMI_Click(object sender, RoutedEventArgs e)
        {
            var roomsWindow = new Rooms();
            roomsWindow.Show();
        }

        private void UsersMI_Click(object sender, RoutedEventArgs e)
        {
            var usersWindow = new Users();
            usersWindow.Show();
        }

        private void GuestsMI_Click(object sender, RoutedEventArgs e)
        {
            var guestsWindow = new Guests();
            guestsWindow.Show();
        }

        private void RoomTypesMI_Click(object sender, RoutedEventArgs e)
        {
            var roomTipesWindow = new RoomTypes();
            roomTipesWindow.Show();
        }

        private void PricesMI_Click(object sender, RoutedEventArgs e)
        {
            var pricesWindow = new Prices();
            pricesWindow.Show();
        }

        private void ReservationsMI_Click(object sender, RoutedEventArgs e)
        {
            var reservationsWindow = new Reservations();
            reservationsWindow.Show();
        }

        public void AdministratorView()
        {
            ReservationsMI.Visibility = Visibility.Collapsed;

        }

        public void ReceptionistView()
        {
            RoomsMI.Visibility = Visibility.Collapsed;
            RoomTypessMI.Visibility = Visibility.Collapsed;
            UsersMI.Visibility = Visibility.Collapsed;
            PricesMI.Visibility = Visibility.Collapsed;
            GuestsMI.Visibility = Visibility.Collapsed;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}
