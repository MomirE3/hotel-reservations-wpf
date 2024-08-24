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
using Microsoft.VisualBasic;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for AddEditReservation.xaml
    /// </summary>
    public partial class AddEditReservation : Window
    {

        private ReservationService reservationService;

        private Reservation contextReservation;
        public AddEditReservation(Reservation? reservation = null)
        {
            if (reservation == null)
            {
                contextReservation = new Reservation();
                
            }
            else
            {
                contextReservation = reservation.Clone();
            }

            contextReservation.StartDateTime = DateTime.Now;

            InitializeComponent();
            reservationService = new ReservationService();
            EndDateTimePicker.IsEnabled = false;
            ReservationTypeTextBox.IsEnabled = false;
            TotalPriceTextBox.IsEnabled = false;

            AdjustWindow(reservation);
            this.DataContext = contextReservation;
        }
        
        public void AdjustWindow(Reservation? reservation = null)
        {
            if (reservation != null)
            {
                Title = "Edit Reservation";
            }
            else
            {
                Title = "Add Reservation";
            }

            reservationService = new ReservationService();
            var roomNumbers = reservationService.GetAllRoomNumbers();
            RoomNumberCB.ItemsSource = roomNumbers;
            StartDateTimePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;

            SetReservationType();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetReservationType();
        }

        private void SetReservationType()
        {
            if (StartDateTimePicker.SelectedDate.HasValue && EndDateTimePicker.SelectedDate.HasValue)
            {
                if (StartDateTimePicker.SelectedDate == EndDateTimePicker.SelectedDate)
                {
                    contextReservation.ReservationType = ReservationType.Day;
                }
                else
                {
                    contextReservation.ReservationType = ReservationType.Night;
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RoomNumberCB.SelectedItem == null)
            {
                MessageBox.Show("Please select a room number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(GuestsTextBox.Text))
            {
                MessageBox.Show("Please select guests.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Guest> guestList = new List<Guest>();
            GuestService guestService = new GuestService();
            string inputString = GuestsTextBox.Text;
            
            string[] stringArray = inputString.Split(',');

            int[] intArray = new int[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++)
            {
                intArray[i] = int.Parse(stringArray[i]);
                Guest guest = guestService.GetAllGuestById(intArray[i]);
                guestList.Add(guest);
            }

            contextReservation.Guests = guestList;

            reservationService = new ReservationService();
            var reservationList = reservationService.GetAllReservations().Where(r => r.EndDateTime == null && r.IsActive).ToList();
            foreach (var reservation in reservationList)
            {
                if(reservation.RoomNumber.Id == contextReservation.RoomNumber.Id)
                {
                    MessageBox.Show("There is already reservation for that room.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            reservationService.SaveReservation(contextReservation);

            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectGuestsBtn_Click(object sender, RoutedEventArgs e)
        {
            var showGuestsWindow = new ShowGuests();
            showGuestsWindow.ShowDialog();

            if (showGuestsWindow.DialogResult == true)
            {
                var selectedGuests = showGuestsWindow.SelectedGuests;
                GuestsTextBox.Text = string.Join(",", selectedGuests.Select(g => $"{g.Id}"));
            }
        }


    }
}
