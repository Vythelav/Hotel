using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class AddEmployeeWindow : Window
    {
        private string connectionString = "Server=LAPTOP-V0AGQKUF\\SLAUUUIK;Database=Hotel;Trusted_Connection=True;";

        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;

            string passwordHash = HashPassword(password);

            string query = @"
                INSERT INTO Employees (FIO, Login, Role, PhoneNumber, Email, PasswordHash)
                VALUES (@FIO, @Login, @Role, @PhoneNumber, @Email, @PasswordHash)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FIO", FIOTextBox.Text);
                command.Parameters.AddWithValue("@Login", LoginTextBox.Text);
                command.Parameters.AddWithValue("@Role", (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());
                command.Parameters.AddWithValue("@PhoneNumber", PhoneTextBox.Text);
                command.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Сотрудник добавлен!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message);
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); 
                }

                return builder.ToString();
            }
        }
    }
}