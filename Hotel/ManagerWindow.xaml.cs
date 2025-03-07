using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace Hotel
{
    public partial class ManagerWindow : Window
    {
        private string connectionString = "Server=510EC15;Database=Hotel;Trusted_Connection=True;";

        public SeriesCollection BarSeries { get; set; }
        public string[] Labels { get; set; }

        public ManagerWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = this; 
        }

        private void LoadData()
        {
            LoadStatistics();
            LoadEmployees();
            LoadSchedule();
            LoadTasks();
        }

        private void LoadStatistics()
        {
            string query = @"
                SELECT 'Загруженность отеля' AS Metric, COUNT(*) AS Value FROM Bookings WHERE BookingStatus = 'Подтверждено'
                UNION ALL
                SELECT 'Выручка', SUM(Amount) FROM Payments WHERE Status = 'Оплачено'
                UNION ALL
                SELECT 'Наполненность номеров', COUNT(*) FROM Rooms WHERE Status = 'Занят'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                BarSeries = new SeriesCollection();
                Labels = new string[dataTable.Rows.Count];
                var values = new double[dataTable.Rows.Count];

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Labels[i] = dataTable.Rows[i]["Metric"].ToString();
                    values[i] = Convert.ToDouble(dataTable.Rows[i]["Value"]);
                }

                var gradientBrush = new LinearGradientBrush
                {
                    StartPoint = new System.Windows.Point(0, 0),
                    EndPoint = new System.Windows.Point(0, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Colors.DodgerBlue, 0),
                        new GradientStop(Colors.DeepSkyBlue, 1)
                    }
                };

                BarSeries.Add(new ColumnSeries
                {
                    Title = "Статистика",
                    Values = new ChartValues<double>(values),
                    Fill = gradientBrush,
                    Stroke = Brushes.White, 
                    StrokeThickness = 1,
                    DataLabels = true, 
                    LabelPoint = point => $"{point.Y}" 
                });
            }
        }

        private void LoadEmployees()
        {
            string query = "SELECT FIO, Role, PhoneNumber, Email FROM Employees";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                EmployeesDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadSchedule()
        {
            string query = @"
                SELECT e.FIO AS EmployeeName, c.Task, c.ScheduledDate AS Date, c.Status 
                FROM CleaningSchedule c
                JOIN Employees e ON c.EmployeeID = e.EmployeeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                ScheduleDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadTasks()
        {
            string query = @"
                SELECT Task, e.FIO AS EmployeeName, Status, CompletionDate 
                FROM Tasks t
                JOIN Employees e ON t.EmployeeID = e.EmployeeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                TasksDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.ShowDialog();
            LoadEmployees(); 
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow();
            addTaskWindow.ShowDialog();
            LoadSchedule();
            LoadTasks();
        }
    }
}