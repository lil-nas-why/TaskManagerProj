using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TaskDB2
{
    public partial class AddEditTaskPage : Page
    {
        private Task _currentTask = new Task();
        private int _projectId;

        public AddEditTaskPage(Task task, int projectId)
        {
            InitializeComponent();

            if (task != null)
            {
                this._currentTask = task;
            }
            this._projectId = projectId;
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
                    MessageBox.Show("Задача уже существует", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (AddPriorityComboBoxx.SelectedItem == null)
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
                

                if (count > 0)
                {
                    MessageBox.Show("Не все поля заполнены", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Привязка значений к объекту задачи
                _currentTask.Name = TextName.Text;
                _currentTask.Description = Description.Text;
                _currentTask.Priority = (AddPriorityComboBoxx.SelectedItem as ComboBoxItem).Content.ToString();
                _currentTask.Comment = CommentBox.Text;
                _currentTask.Deadline = DateTime.Parse(DateTask.Text);
                _currentTask.idProject = _projectId;

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
            // Логика для добавления подзадач
        }
    }
}
