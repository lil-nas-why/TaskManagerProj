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
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private TaskDBEntities dbe;
        public LogInPage()
        {
            InitializeComponent();
            dbe = TaskDBEntities.GetContext();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string log = LogInLogBox.Text;
                string pass = LogInPassBox.Password;

                var user = dbe.User.FirstOrDefault(u => u.Login == log && u.Password == pass);

                if (user != null)
                {
                    UserManager.currentUser = user;
                  
                    FrameManager.MainFrame.Navigate(new TaskPage());
                }
                else
                {
                    MessageBox.Show("Login failed: Invalid login or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message);
            }


        }

        private void GoToReg_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new RegPage());
        }
    }
}
