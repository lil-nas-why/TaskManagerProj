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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskDB2
{
    public partial class UserInfoPage : Page
    {
        private User currentUser;

        public UserInfoPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            DataContext = user;
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewPasswordBox.Text))
            {
                currentUser.Password = NewPasswordBox.Text;
                TaskDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Password changed successfully!");
                NewPasswordBox.Visibility = Visibility.Collapsed;
                SavePassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please enter a new password.");
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NewPasswordBlock.Visibility = Visibility.Visible;
            NewPasswordBox.Visibility = Visibility.Visible;
            SavePassword.Visibility = Visibility.Visible;

        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
    }
}
