using HotelReservations.Model;
using HotelReservations.Service;
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

namespace HotelReservations.Windows
{
    public partial class Guests : Window
    {
        private ICollectionView view;

        public Guests()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            var guestService = new GuestService();
            var guests = guestService.GetAllGuest();

                view = CollectionViewSource.GetDefaultView(guests);
                view.Filter = DoFilter;

                GuestsDG.ItemsSource = null;
                GuestsDG.ItemsSource = view;
                GuestsDG.IsSynchronizedWithCurrentItem = true;
                GuestsDG.SelectedItem = null;
                view.Refresh();
        }

        private bool DoFilter(object guestObject)
        {
            var guest = guestObject as Guest;

            var guestIDNumberSearchParam = GuestIDNumberSearchTB.Text;

            if (guest.IDNumber.Contains(guestIDNumberSearchParam))
            {
                return true;
            }

            return false;
        }

        private void GuestsDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addGuestWindow = new AddEditGuest();

            Hide();
            if (addGuestWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedGuest = (Guest)view.CurrentItem;

            if (selectedGuest != null)
            {
                var editGuestWindow = new AddEditGuest(selectedGuest);

                Hide();

                if (editGuestWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select guest for edit. ");
                return;
            }
        }

        private void GuestIDNumberSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var guestService = new GuestService();
            var selectedGuestToDelete = (Guest)view.CurrentItem;

            if (selectedGuestToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this guest? ", "Delete Guest", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    guestService.DeleteGuest(selectedGuestToDelete.IDNumber);
                    FillData();
                }
            }
        }
    }
}
