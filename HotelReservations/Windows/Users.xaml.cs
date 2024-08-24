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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        private ICollectionView view;

        public Users()
        {
            var userService = new UserService();

            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            var userService = new UserService();
            var users = userService.GetAllUsers();

            view = CollectionViewSource.GetDefaultView(users);
            view.Filter = DoFilter;

            UsersDG.ItemsSource = null;
            UsersDG.ItemsSource = view;
            UsersDG.IsSynchronizedWithCurrentItem = true;
            UsersDG.SelectedItem = null;
            view.Refresh();           
        }

        private bool DoFilter(object userObject)
        {
            var user = userObject as User;

            var userUsernameSearchParam = UserUsernameSearchTB.Text;

            if (user.Username.Contains(userUsernameSearchParam))
            {
                return true;
            }

            return false;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
  
            var addUserWindow = new AddEditUser();

            Hide();
            if (addUserWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();

        }

        private void UserUsernameSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void UsersDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = view.CurrentItem as User;

            if (selectedUser != null)
            {
                var editUsersWindow = new AddEditUser(selectedUser);
                Hide();

                if (editUsersWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("Please select a user to edit. ");
                return;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedUserToDelete = (User)view.CurrentItem;

            if (selectedUserToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this user? ", "Delete User", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    selectedUserToDelete.isActive = false;

                    var userService = new UserService();
                    userService.SaveUser(selectedUserToDelete);
                    FillData();
                }
            }
            else
            {
                MessageBox.Show("Please select an user to delete. ");
                return;
            }
        }
    }
}
