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
    public partial class AddEditTaskPage : Page
    {

        private Task _currentTask = new Task();

        public AddEditTaskPage(Task task)
        {
            InitializeComponent();

            if (task != null)
            {
                this._currentTask = task;
            }
            DataContext = _currentTask;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;
                Task someTask = null;

                if (_currentTask.Id == 0)
                {
                    someTask = TaskDBEntities.GetContext().Task.Where(x => x.Name.ToLower().Equals(TextName.Text.ToLower())).FirstOrDefault();
                }

                if (someTask != null)
                {
                    MessageBox.Show("Сотрудник уже существует", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (String.IsNullOrWhiteSpace(TextName.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(Description.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(Priority.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(CommentBox.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(DateTask.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(IdStatusBox.Text))
                {
                    count++;
                }

                if (count > 0)
                {
                    MessageBox.Show("Не все поля заполнены", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Привязка значений к объекту задачи
                _currentTask.Name = TextName.Text;
                _currentTask.Description = Description.Text;
                _currentTask.Priority = Priority.Text;
                _currentTask.Comment = CommentBox.Text;
                _currentTask.Deadline = DateTime.Parse(DateTask.Text);
                _currentTask.idStatus = int.Parse(IdStatusBox.Text);

                if (_currentTask.Id == 0)
                {
                    TaskDBEntities.GetContext().Task.Add(_currentTask);
                }

                TaskDBEntities.GetContext().SaveChanges();
                FrameManager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void AddSubtasks_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
