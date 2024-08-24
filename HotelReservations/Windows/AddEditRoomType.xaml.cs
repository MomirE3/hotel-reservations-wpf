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
using HotelReservations.Repostiory;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for AddEditRoomType.xaml
    /// </summary>
    public partial class AddEditRoomType : Window
    {
        private RoomTypeService roomTypeService;

        private RoomType contextRoomType;
        public AddEditRoomType(RoomType? roomType = null)
        {
            if (roomType == null)
            {
                contextRoomType = new RoomType();
            }
            else
            {
                contextRoomType = roomType.Clone();
            }

            InitializeComponent();
            roomTypeService = new RoomTypeService();

            AdjustWindow(roomType);

            this.DataContext = contextRoomType;
        }

        public void AdjustWindow(RoomType? roomType = null)
        {
            if (roomType != null)
            {
                Title = "Edit Room Type";
            }
            else
            {
                Title = "Add Room Type";
            }

            var roomTypeService = new RoomTypeService();

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var roomTypeService = new RoomTypeService();

            if (roomTypeService.RoomTypeExists(contextRoomType.Name, contextRoomType.Id))
            {
                MessageBox.Show("Room type already exists! Please pick a different room type name.");
                return;
            }

            if (string.IsNullOrEmpty(contextRoomType.Name))
            {
                MessageBox.Show("Fill Room Type field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (roomTypeService.RoomTypeExists(contextRoomType.Name, contextRoomType.Id))
            {
                MessageBox.Show("Room type already exists! Please pick a different room type name.");
                return;
            }

            roomTypeService.SaveRoomType(contextRoomType);
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
