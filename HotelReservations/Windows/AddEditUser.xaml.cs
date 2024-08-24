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
    /// Interaction logic for AddEditUser.xaml
    /// </summary>
    public partial class AddEditUser : Window
    {

        private UserService userService;
        private User contextUser;

        public AddEditUser(User? user = null)
        {
            if (user == null)
            {
                contextUser = new User();
            }
            else
            {
                contextUser = user.Clone();
            }

            InitializeComponent();
            userService = new UserService();

            AdjustWindow(user);

            DataContext = contextUser;
        }

        private void AdjustWindow(User user = null)
        {
            UserTypeCB.Items.Add(typeof(Administrator).Name);
            UserTypeCB.Items.Add(typeof(Receptionist).Name);

            if (user != null)
            {
                Title = "Edit user";
                UserTypeCB.SelectedItem = user.GetType().Name;
                UserTypeCB.IsEnabled = false;
            }
            else
            {
                Title = "Add user";
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(contextUser.Name) )
            {
                MessageBox.Show("Fill name field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(contextUser.Surname))
            {
                MessageBox.Show("Fill surname field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(contextUser.JMBG))
            {
                MessageBox.Show("Fill JMBG field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (userService.UserJMBGExists(contextUser.JMBG, contextUser.Id))
            {
                MessageBox.Show("JMBG already exists!");
                return;
            }

            if (string.IsNullOrEmpty(contextUser.Username))
            {
                MessageBox.Show("Fill Username field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (userService.UserUsernameExists(contextUser.Username, contextUser.Id))
            {
                MessageBox.Show("Username already exists!");
                return;
            }

            if (string.IsNullOrEmpty(contextUser.Password))
            {
                MessageBox.Show("Fill Password field.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (contextUser.UserType == null)
            {
                MessageBox.Show("Please select a user type! ");
                return;
            }
            userService.SaveUser(contextUser);

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
