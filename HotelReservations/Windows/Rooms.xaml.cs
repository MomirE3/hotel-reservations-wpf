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
    /// <summary>
    /// Interaction logic for Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        private ICollectionView view;
        public Rooms()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            var roomService = new RoomService();
            var rooms = roomService.GetAllRooms().Where(room => room.IsActive).ToList();

            view = CollectionViewSource.GetDefaultView(rooms);
            view.Filter = DoFilter;

            RoomsDG.ItemsSource = null;
            RoomsDG.ItemsSource = view;
            RoomsDG.IsSynchronizedWithCurrentItem = true;
            RoomsDG.SelectedIndex = -1;
            view.Refresh();
        }

        public void FillInactiveRooms()
        {
            var roomService = new RoomService();
            var inactiveRooms = roomService.GetAllRooms().Where(room => !room.IsActive).ToList();

            view = CollectionViewSource.GetDefaultView(inactiveRooms);
            view.Filter = DoFilter;

            RoomsDG.ItemsSource = null;
            RoomsDG.ItemsSource = view;
            RoomsDG.IsSynchronizedWithCurrentItem = true;
        }

        private bool DoFilter(object roomObject)
        {
            var room = roomObject as Room;

            var roomTypeSearchParam = RoomTypeSearchTB.Text;

            if (room.RoomType.Name.Contains(roomTypeSearchParam))
            {
                return true;
            }

            return false;
        }

        private void RoomsDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addRoomWindow = new AddEditRoom();

            Hide();
            if (addRoomWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = (Room)view.CurrentItem;

            if (selectedRoom != null)
            {
                var editRoomWindow = new AddEditRoom(selectedRoom);

                Hide();

                if (editRoomWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select a room to edit. ");
                return;
            }
        }

        private void RoomTypeSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoomToDelete = (Room)view.CurrentItem;

            if (selectedRoomToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this room? ", "Delete Room", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    selectedRoomToDelete.IsActive = false;

                    var roomService = new RoomService();
                    roomService.SaveRoom(selectedRoomToDelete);
                    FillData();
                }
            }
            else
            {
                MessageBox.Show("Please select a room to delete. ");
                return;
            }
        }
        private void ShowInactiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FillInactiveRooms();
        }

        private void ShowInactiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        public List<Room> FillTakenRooms()
        {
            var reservationService = new ReservationService();
            var activeReservations = reservationService.GetAllReservations().Where(r => r.EndDateTime == null).ToList();

            var takenRooms = new List<Room>();

            foreach (var reservation in activeReservations)
            {
                var roomService = new RoomService();
                var rooms = roomService.GetAllRooms().Where(room => room.IsActive && room.RoomNumber == reservation.RoomNumber.RoomNumber).ToList();

                takenRooms.AddRange(rooms);
            }

            view = CollectionViewSource.GetDefaultView(takenRooms);
            view.Filter = DoFilter;

            RoomsDG.ItemsSource = null;
            RoomsDG.ItemsSource = view;
            RoomsDG.IsSynchronizedWithCurrentItem = true;
            RoomsDG.SelectedIndex = -1;
            view.Refresh();

            return takenRooms;
        }


        private void ShowTakenCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FillTakenRooms();
        }

        private void ShowTakenCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FillData();
        }
    }
}
