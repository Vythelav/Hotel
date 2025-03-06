using Hotel;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=LAPTOP-V0AGQKUF\\SLAUUUIK;Database=Hotel;Trusted_Connection=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            string hashedPassword = HashPassword(password);

            string role = CheckCredentials(login, hashedPassword);

            if (role != null)
            {
                OpenRoleWindow(role);
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private string CheckCredentials(string login, string hashedPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Role FROM Employees WHERE Login = @Login AND PasswordHash = @PasswordHash";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                object result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        private void OpenRoleWindow(string role)
        {
            switch (role)
            {
                case "Администратор":
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    break;
                case "Уборщик":
                    CleanerWindow cleanerWindow = new CleanerWindow();
                    cleanerWindow.Show();
                    break;
                case "Руководитель":
                    ManagerWindow managerWindow = new ManagerWindow();
                    managerWindow.Show();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль");
                    break;
            }
            this.Close();
        }
    }
}