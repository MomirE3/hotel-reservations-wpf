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
using HotelReservations.Repostiory;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userRepository = new UserRepository();
                var userList = userRepository.GetAll();

                foreach (var user in userList)
                {
                    if (txtUsername.Text == user.Username && txtPassword.Password == user.Password)
                    {
                        var mainWindow = new MainWindow();

                        if (user.UserType.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
                        {
                            mainWindow.AdministratorView();
                            MessageBox.Show("Welcome to the Administrator view!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (user.UserType.Equals("Receptionist", StringComparison.OrdinalIgnoreCase))
                        {
                            mainWindow.ReceptionistView();
                            MessageBox.Show("Welcome to the Receptionist view!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        mainWindow.Show();
                        Close();
                        return;
                    }
                }

                txtUsername.Text = string.Empty;
                txtPassword.Password = string.Empty;
                MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
