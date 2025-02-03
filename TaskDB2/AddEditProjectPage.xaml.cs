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
    public partial class AddEditProjectPage : Page
    {
        private Project _currentProject = new Project();

        public AddEditProjectPage(Project project)
        {
            InitializeComponent();

            if (project != null)
            {
                this._currentProject = project;
            }
            DataContext = _currentProject;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;
                Project someProject = null;

                if (_currentProject.Id == 0)
                {
                    someProject = TaskDBEntities.GetContext().Project.Where(x => x.Name.ToLower().Equals(ProjectName.Text.ToLower())).FirstOrDefault();
                }

                if (someProject != null)
                {
                    MessageBox.Show("Проект уже существует", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (String.IsNullOrWhiteSpace(ProjectName.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(ProjectCommand.Text))
                {
                    count++;
                }
                if (String.IsNullOrWhiteSpace(ProjectDescription.Text))
                {
                    count++;
                }

                if (count > 0)
                {
                    MessageBox.Show("Не все поля заполнены", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Привязка значений к объекту проекта
                _currentProject.Name = ProjectName.Text;
                _currentProject.Team = ProjectCommand.Text;
                _currentProject.Description = ProjectDescription.Text;

                if (_currentProject.Id == 0)
                {
                    TaskDBEntities.GetContext().Project.Add(_currentProject);
                }

                TaskDBEntities.GetContext().SaveChanges();
                FrameManager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
