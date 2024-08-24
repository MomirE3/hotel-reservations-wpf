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
using HotelReservations.Model;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for RoomTypes.xaml
    /// </summary>
    public partial class RoomTypes : Window
    {
        private ICollectionView view;
        public RoomTypes()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            var roomTypeService = new RoomTypeService();
            var roomTypes = roomTypeService.GetAllRoomTypes().Where(roomType => roomType.IsActive).ToList();

            view = CollectionViewSource.GetDefaultView(roomTypes);
            view.Filter = DoFilter;

            RoomTypesDG.ItemsSource = null;
            RoomTypesDG.ItemsSource = view;
            RoomTypesDG.IsSynchronizedWithCurrentItem = true;
            RoomTypesDG.SelectedIndex = -1;
            view.Refresh();
        }

        private bool DoFilter(object roomObject)
        {
            var roomType = roomObject as RoomType;

            var roomTypeSearchParam = RoomTypeSearchTB.Text;

            if (roomType.Name.Contains(roomTypeSearchParam))
            {
                return true;
            }

            return false;
        }

        private void RoomTypeSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void RoomTypesDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addRoomTypeWindow = new AddEditRoomType();

            Hide();
            if (addRoomTypeWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoomType = (RoomType)view.CurrentItem;

            if (selectedRoomType != null)
            {
                var roomService = new RoomService();
                var includedRoomTypes = roomService.GetIncludedRoomTypes(selectedRoomType.Id);

                if (includedRoomTypes.Any())
                {
                    MessageBox.Show("Cannot edit this room type because it is already used in rooms.");
                    return;
                }

                var editRoomTypeWindow = new AddEditRoomType(selectedRoomType);
                Hide();

                if (editRoomTypeWindow.ShowDialog() == true)
                {
                    FillData();

                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select a room type to edit.");
                return;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoomTypeToDelete = (RoomType)view.CurrentItem;

            if (selectedRoomTypeToDelete != null)
            {
                var roomService = new RoomService();
                var includedRoomTypes = roomService.GetIncludedRoomTypes(selectedRoomTypeToDelete.Id);

                if (includedRoomTypes.Any())
                {
                    MessageBox.Show("Cannot delete this room type because it is already used in rooms.");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this room type? ", "Delete Room Type", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    selectedRoomTypeToDelete.IsActive = false;

                    var roomTypeService = new RoomTypeService();
                    roomTypeService.SaveRoomType(selectedRoomTypeToDelete);
                    FillData();
                }
            }
            else
            {
                MessageBox.Show("Please select a room type to delete.");
                return;
            }
        }
    }
}
