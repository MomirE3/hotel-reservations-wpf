using HotelReservations.Model;
using HotelReservations.Service;
using System.Security.RightsManagement;
using System.Windows;

namespace HotelReservations.Windows
{
    public partial class AddEditGuest : Window
    {
        private GuestService guestService;
        private Guest contextGuest;

        public AddEditGuest(Guest? guest = null)
        {
            if (guest == null)
            {
                contextGuest = new Guest();
            }
            else
            {
                contextGuest = guest.Clone();
            }

            InitializeComponent();
            guestService = new GuestService();

            AdjustWindow(guest);

            DataContext = contextGuest;
        }

        public void AdjustWindow(Guest? guest = null)
        {
            if (guest != null)
            {
                Title = "Edit Guest";
            }
            else
            {
                Title = "Add Guest";
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(contextGuest.Name) || string.IsNullOrEmpty(contextGuest.Surname) || string.IsNullOrEmpty(contextGuest.IDNumber))
            {
                MessageBox.Show("Fill required fields.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (guestService.GuestIDNumberExists(contextGuest.IDNumber, contextGuest.Id))
            {
                MessageBox.Show("IDNumber already exists!");
                return;
            }

            contextGuest.Name = GuestNameTB.Text;
            contextGuest.Surname = GuestSurnameTB.Text;
            contextGuest.IDNumber = GuestIDNumberTB.Text;
            contextGuest.IsActive = true; 

            guestService.SaveGuest(contextGuest);

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
