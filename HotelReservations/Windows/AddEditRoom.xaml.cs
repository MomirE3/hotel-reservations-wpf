using HotelReservations.Model;
using HotelReservations.Service;
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

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for AddEditRoom.xaml
    /// </summary>
    public partial class AddEditRoom : Window
    {
        private RoomService roomService;

        private Room contextRoom;
        public AddEditRoom(Room? room = null)
        {
            if (room == null)
            {
                contextRoom = new Room();
            }
            else
            {
                contextRoom = room.Clone();
            }

            InitializeComponent();
            roomService = new RoomService();

            AdjustWindow(room);

            this.DataContext = contextRoom;
        }

        public void AdjustWindow(Room? room = null)
        {
            var roomService = new RoomService();
            var roomTypes = roomService.GetAllRoomTypes();
            RoomTypesCB.ItemsSource = roomTypes;

            if (room != null)
            {           
                Title = "Edit Room";
                RoomTypesCB.ItemsSource = new List<RoomType> {room.RoomType};
                RoomTypesCB.SelectedItem = room.RoomType;
                RoomTypesCB.IsEnabled = false;
            }
            else
            {
                Title = "Add Room";              
            }
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(contextRoom.RoomNumber))
            {
                MessageBox.Show("Fill required fields.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (roomService.RoomNumberExists(contextRoom.RoomNumber, contextRoom.Id))
            {
                MessageBox.Show("Room number already exists! Please use a different room number.");
                return;
            }

            if (contextRoom.RoomType == null)
            {
                MessageBox.Show("Please select a room type! ");
                return;
            }

            roomService.SaveRoom(contextRoom);

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
