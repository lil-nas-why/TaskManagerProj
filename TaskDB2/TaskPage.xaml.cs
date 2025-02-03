using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace TaskDB2
{
    public partial class TaskPage : Page
    {
        private ObservableCollection<Task> tasks;
        private ObservableCollection<Project> projects;

        public TaskPage()
        {
            InitializeComponent();
            this.Loaded += TaskPage_Loaded;
        }

        private void TaskPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProjects();
            LoadTasks();
        }

        private void LoadProjects()
        {
            projects = new ObservableCollection<Project>(TaskDBEntities.GetContext().Project.ToList());
            ProjectsListView.ItemsSource = projects;
        }

        private void LoadTasks()
        {
            tasks = new ObservableCollection<Task>(TaskDBEntities.GetContext().Task.ToList());
            ViewMainTusksLV.ItemsSource = tasks;
        }

        private void UpdateSource()
        {
            if (ViewMainTusksLV == null || SearchBar == null || PriorityFilterComboBox == null)
            {
                return;
            }

            var source = new ObservableCollection<Task>(tasks);

            if (PriorityFilterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() != "Все")
            {
                source = new ObservableCollection<Task>(source.Where(x => x.Priority.ToLower() == selectedItem.Content.ToString().ToLower()));
            }

            ViewMainTusksLV.ItemsSource = source;
        }

        private void AddMainTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsListView.SelectedItem as Project;
            if (selectedProject != null)
            {
                FrameManager.MainFrame.Navigate(new AddEditTaskPage(null, selectedProject.Id));
            }

            UpdateSource();
        }

        private void ParseToJson_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = ViewMainTusksLV.SelectedItem as Task;
            if (selectedTask != null)
            {
                string json = SerializeToJson(selectedTask);
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Save JSON File"
                };

                if (dialog.ShowDialog() == true)
                {
                    SaveToFile(json, dialog.FileName);
                }
            }
        }

        private void ParseToTxt_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = ViewMainTusksLV.SelectedItem as Task;
            if (selectedTask != null)
            {
                string txt = SerializeToTxt(selectedTask);
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Save Text File"
                };

                if (dialog.ShowDialog() == true)
                {
                    SaveToFile(txt, dialog.FileName);
                }
            }
        }

        private string SerializeToJson(Task task)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(task, settings);
        }

        private string SerializeToTxt(Task task)
        {
            return $"Name: {task.Name}\nDescription: {task.Description}\nDate: {task.Comment}";
        }

        private void SaveToFile(string content, string fileName)
        {
            // Определяем путь к директории для сохранения файлов
            string outputDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Определяем полный путь к файлу
            string filePath = System.IO.Path.Combine(outputDirectory, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(content);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var tasksForRemoving = ViewMainTusksLV.SelectedItems.Cast<Task>().ToList();
            foreach (var task in tasksForRemoving)
            {
                TaskDBEntities.GetContext().Task.Remove(task);
                tasks.Remove(task);
            }
            TaskDBEntities.GetContext().SaveChanges();
            TaskDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            UpdateSource();
        }

        private void ViewMainTusksLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTask = ViewMainTusksLV.SelectedItem as Task;
            if (selectedTask != null)
            {
                FillTaskDetails(selectedTask);
            }
        }

        private void FillTaskDetails(Task task)
        {
            // Заполните детали задачи здесь
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = ViewMainTusksLV.SelectedItem as Task;
            if (selectedTask != null)
            {
                selectedTask.Priority = "Завершено"; // Устанавливаем приоритет как "Завершено"
                TaskDBEntities.GetContext().SaveChanges();
                UpdateSource();
                FillTaskDetails(selectedTask);
            }
        }

        private void PriorityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSource();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new LogInPage());
        }

        private void LoadTasksForProject(int projectId)
        {
            var tasksForProject = TaskDBEntities.GetContext().Task.Where(t => t.idProject == projectId).ToList();
            ViewMainTusksLV.ItemsSource = new ObservableCollection<Task>(tasksForProject);
        }

        private void ProjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProject = ProjectsListView.SelectedItem as Project;
            if (selectedProject != null)
            {
                LoadTasksForProject(selectedProject.Id);
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddEditProjectPage(null));
        }

        private void SearchBar_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var source = TaskDBEntities.GetContext().Project.ToList();

            if (!String.IsNullOrWhiteSpace(SearchBar.Text))
            {
                source = source.Where(x => x.Name.ToLower().Contains(SearchBar.Text.ToLower())).ToList();
            }

            ProjectsListView.ItemsSource = new ObservableCollection<Project>(source);
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int projectId)
            {
                var projectToRemove = TaskDBEntities.GetContext().Project.FirstOrDefault(p => p.Id == projectId);
                if (projectToRemove != null)
                {
                    TaskDBEntities.GetContext().Project.Remove(projectToRemove);
                    TaskDBEntities.GetContext().SaveChanges();
                    projects.Remove(projectToRemove);
                }
            }
        }

        private void UserInfo_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                FrameManager.MainFrame.Navigate(new UserInfoPage(currentUser));
            }
            else
            {
                MessageBox.Show("No user is currently logged in.");
            }
        }

        private User GetCurrentUser()
        {
            return UserManager.currentUser; // Возвращаем текущего пользователя из статического свойства
        }

        private void ViewMainTusksLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedTask = ViewMainTusksLV.SelectedItem as Task;
            if (selectedTask != null)
            {
                FrameManager.MainFrame.Navigate(new ViewTaskPage(selectedTask));
            }
        }
    }

    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string priority)
            {
                switch (priority.ToLower())
                {
                    case "высокий":
                        return Brushes.Red;
                    case "средний":
                        return Brushes.Orange;
                    case "низкий":
                        return Brushes.Gold;
                    case "завершено":
                        return Brushes.Green; // Цвет для завершенных задач
                    default:
                        return Brushes.Gray;
                }
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
