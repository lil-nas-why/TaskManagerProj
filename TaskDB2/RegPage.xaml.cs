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
    public partial class RegPage : Page
    {
        private TaskDBEntities dbe;
        public RegPage()
        {
            InitializeComponent();
            dbe = TaskDBEntities.GetContext();
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User userReg = new User();
                {
                    userReg.Login = LogBox.Text;
                    userReg.Password = PassBox.Password;
                    userReg.Email = EmailBox.Text;
                }

                dbe.User.Add(userReg);
                dbe.SaveChanges();
                FrameManager.MainFrame.Navigate(new TaskPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new LogInPage());
        }
    }
}
