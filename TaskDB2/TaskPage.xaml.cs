using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Microsoft.Win32;
using Newtonsoft.Json;

namespace TaskDB2
{
    public partial class TaskPage : Page
    {
        public TaskPage()
        {
            InitializeComponent();
            this.Loaded += TaskPage_Loaded;
        }

        private void TaskPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSource();
        }

        private void UpdateSource()
        {
            if (ViewMainTusksLV == null || SearchBar == null || PriorityFilterComboBox == null)
            {
                return;
            }

            var source = TaskDBEntities.GetContext().Task.ToList();

            if (!String.IsNullOrWhiteSpace(SearchBar.Text))
            {
                source = source.Where(x => x.Name.ToLower().Contains(SearchBar.Text.ToLower())).ToList();
            }

            if (PriorityFilterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() != "Все")
            {
                source = source.Where(x => x.Priority.ToLower() == selectedItem.Content.ToString().ToLower()).ToList();
            }

            ViewMainTusksLV.ItemsSource = source;
        }

        private void AddMainTask_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddEditTaskPage(ViewMainTusksLV.SelectedItem as Task));
            ViewMainTusksLV.SelectedIndex = -1;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
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
            TaskDBEntities.GetContext().Task.RemoveRange(tasksForRemoving);
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
            TaskName.Text = task.Name;
            DescBox.Text = task.Description;
            CommentBox.Text = task.Comment;
            PriorityLabel.Content = task.Priority;
            Date.Text = $"Дата: {task.Deadline:dd/MM/yyyy @ HH:mm}";

            PriorityLabel.DataContext = task;
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
