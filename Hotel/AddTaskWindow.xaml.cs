using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class AddTaskWindow : Window
    {
        private string connectionString = "Server=LAPTOP-V0AGQKUF\\SLAUUUIK;Database=Hotel;Trusted_Connection=True;";

        public AddTaskWindow()
        {
            InitializeComponent();
            LoadEmployees(); 
        }

        private void LoadEmployees()
        {
            string query = "SELECT EmployeeID, FIO FROM Employees";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                EmployeeComboBox.ItemsSource = dataTable.DefaultView;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedValue == null || string.IsNullOrEmpty(TaskTextBox.Text) || TaskDatePicker.SelectedDate == null || StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int employeeID = (int)EmployeeComboBox.SelectedValue;
            string task = TaskTextBox.Text;
            DateTime date = TaskDatePicker.SelectedDate.Value;
            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string query = @"
                INSERT INTO Tasks (EmployeeID, Task, AssignedDate, Status)
                VALUES (@EmployeeID, @Task, @AssignedDate, @Status)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                command.Parameters.AddWithValue("@Task", task);
                command.Parameters.AddWithValue("@AssignedDate", date);
                command.Parameters.AddWithValue("@Status", status);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Задача успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении задачи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}