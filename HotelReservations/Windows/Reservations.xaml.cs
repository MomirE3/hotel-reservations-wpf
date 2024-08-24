using HotelReservations.Converter;
using HotelReservations.Model;
using HotelReservations.Service;
using Microsoft.VisualBasic;
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
    /// <summary>
    /// Interaction logic for Reservations.xaml
    /// </summary>
    public partial class Reservations : Window
    {
        private ICollectionView view;

        public Reservations()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            var reservationService = new ReservationService();
            var reservations = reservationService.GetAllReservations();

            view = CollectionViewSource.GetDefaultView(reservations);
            view.Filter = DoFilter;

            ReservationsDG.ItemsSource = null;
            ReservationsDG.ItemsSource = view;
            ReservationsDG.IsSynchronizedWithCurrentItem = true;
            ReservationsDG.SelectedIndex = -1;
            view.Refresh();
        }

        private void ReservationsDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            if (e.PropertyName.ToLower() == "Guests".ToLower())
            {
                var textColumn = e.Column as DataGridTextColumn;

                if (textColumn != null)
                {
                    textColumn.Binding = new Binding(e.PropertyName)
                    {
                        Converter = new ConverterGuest()
                    };
                }
            }
        }
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            var reservationService = new ReservationService();
            var reservations = reservationService.GetAllReservations();

            var startDateParam = StartDatePicker.SelectedDate;
            var endDateParam = EndDatePicker.SelectedDate;

            if (startDateParam != null && endDateParam != null)
            {

                if(startDateParam > endDateParam)
                {
                    MessageBox.Show("Please select date properly. ");
                    return;
                }
                var filteredReservations = reservations.Where(r =>
                    r.StartDateTime >= startDateParam && (r.EndDateTime <= endDateParam || r.EndDateTime == null)).ToList();

                view = CollectionViewSource.GetDefaultView(filteredReservations);
                view.Filter = DoFilter;

                ReservationsDG.ItemsSource = null;
                ReservationsDG.ItemsSource = view;
                ReservationsDG.IsSynchronizedWithCurrentItem = true;
                ReservationsDG.SelectedIndex = -1;
                view.Refresh();
            }
            else if (startDateParam == null && endDateParam != null)
            {
                var filteredReservations = reservations.Where(r => r.EndDateTime <= endDateParam || r.EndDateTime == null).ToList();
                view = CollectionViewSource.GetDefaultView(filteredReservations);
                view.Filter = DoFilter;

                ReservationsDG.ItemsSource = null;
                ReservationsDG.ItemsSource = view;
                ReservationsDG.IsSynchronizedWithCurrentItem = true;
                ReservationsDG.SelectedIndex = -1;
                view.Refresh();
            }
            else if (startDateParam != null && endDateParam == null)
            {
                var filteredReservations = reservations.Where(r => r.StartDateTime >= startDateParam).ToList();
                view = CollectionViewSource.GetDefaultView(filteredReservations);
                view.Filter = DoFilter;

                ReservationsDG.ItemsSource = null;
                ReservationsDG.ItemsSource = view;
                ReservationsDG.IsSynchronizedWithCurrentItem = true;
                ReservationsDG.SelectedIndex = -1;
                view.Refresh();
            }
        }

        private void ActiveReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            var reservationService = new ReservationService();
            var reservations = reservationService.GetAllReservations();

            var filteredReservations = reservations.Where(r => r.EndDateTime == null).ToList();
            view = CollectionViewSource.GetDefaultView(filteredReservations);
            view.Filter = DoFilter;

            ReservationsDG.ItemsSource = null;
            ReservationsDG.ItemsSource = view;
            ReservationsDG.IsSynchronizedWithCurrentItem = true;
            ReservationsDG.SelectedIndex = -1;
            view.Refresh();
        }

        private void AllReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            FillData();
        }

        private bool DoFilter(object roomObject)
        {
            var reservation = roomObject as Reservation;
            var roomNumberSearchParam = RoomNumberSearchTB.Text;
            

            if (!string.IsNullOrEmpty(roomNumberSearchParam) && !reservation.RoomNumber.RoomNumber.Contains(roomNumberSearchParam))
            {
                return false;
            }
            return true;
        }

        private void RoomNumberSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addReservationWindow = new AddEditReservation();

            Hide();
            if (addReservationWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedReservation = (Reservation)view.CurrentItem;

            if (selectedReservation != null)
            {
                var editReservationWindow = new AddEditReservation(selectedReservation);

                Hide();

                if (editReservationWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select reservation for edit. ");
                return;
            }
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedReservationToDelete = (Reservation)view.CurrentItem;

            if (selectedReservationToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this reservation? ", "Delete Reservation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    selectedReservationToDelete.IsActive = false;

                    var reservationService = new ReservationService();
                    reservationService.SaveReservation(selectedReservationToDelete);
                    FillData();
                }
            }
            else
            {
                MessageBox.Show("Please select a reservation to delete. ");
                return;
            }
        }
        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedReservation = (Reservation)view.CurrentItem;

            if (selectedReservation == null)
            {
                MessageBox.Show("Please select a reservation. ");
                return;
            }

            var reservationService = new ReservationService();
            var databaseReservation = reservationService.GetReservationById(selectedReservation.Id);

            if (databaseReservation == null)
            {
                MessageBox.Show("Error retrieving reservation from the database.");
                return;
            }

            if (databaseReservation.EndDateTime.HasValue)
            {
                MessageBox.Show("Reservation is already finished.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DateTime startDateTime = selectedReservation.StartDateTime;
            var endDateTime = DateTime.Now;

            TimeSpan timeDifference = endDateTime - startDateTime;

            int differenceInDays = timeDifference.Days;
            int differenceInHours = timeDifference.Hours;
            int differenceInMinutes = timeDifference.Minutes;

            if (differenceInDays < 0)
            {
                MessageBox.Show("Reservation did not start yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (differenceInDays == 0 && differenceInHours < 0)
            {
                MessageBox.Show("Reservation did not start yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (differenceInDays == 0 && differenceInHours == 0 && differenceInMinutes < 0)
            {
                MessageBox.Show("Reservation did not start yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to leave the room?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                selectedReservation.EndDateTime = DateTime.Now;

                PriceService priceService = new PriceService();
                List<Price> pricelist = priceService.GetAllPrices();

                if (differenceInDays > 0)
                {
                    selectedReservation.ReservationType = ReservationType.Night;

                    foreach (var price in pricelist)
                    {
                        if (price.RoomType.Name == selectedReservation.RoomNumber.RoomType.Name && price.ReservationType == selectedReservation.ReservationType)
                        {
                            selectedReservation.TotalPrice = price.PriceValue * differenceInDays * selectedReservation.Guests.Count;
                        }
                    }
                }
                else
                {
                    selectedReservation.ReservationType = ReservationType.Day;
                    foreach (var price in pricelist)
                    {
                        if (price.RoomType.Id == selectedReservation.RoomNumber.RoomType.Id && price.ReservationType == selectedReservation.ReservationType)
                        {
                            selectedReservation.TotalPrice = price.PriceValue * selectedReservation.Guests.Count;
                        }
                    }

                }

                FillData();
                reservationService.SaveReservation(selectedReservation);
            }
        }
    }
}