using HotelReservations.Model;
using HotelReservations.Service;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelReservations.Windows
{
    public partial class ShowGuests : Window
    {
        private readonly GuestService guestService = new GuestService();
        public List<Guest> SelectedGuests = new List<Guest>();
        public List<Guest> emptyList = new List<Guest>();

        public ShowGuests()
        {
            InitializeComponent();
            LoadActiveGuests();
        }

        private void LoadActiveGuests()
        {
            var activeGuests = guestService.GetAllGuest();
            dgActiveGuests.ItemsSource = activeGuests;
        }

        private void LoadDetailedGuests(List<Guest> SelectedGuests)
        {
            dgDetailedGuests.ItemsSource = SelectedGuests;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dgActiveGuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            var selectedGuests = e.AddedItems.Cast<Guest>().ToList();

            foreach (var guest in selectedGuests)
            {
                foreach (var guestInArray in SelectedGuests)
                {
                    if (guestInArray.Id == guest.Id)
                    {
                        return;
                    }
                }
                SelectedGuests.Add(guest);
            }
            LoadDetailedGuests(emptyList);
            LoadDetailedGuests(SelectedGuests);
        }
    }
    
}
