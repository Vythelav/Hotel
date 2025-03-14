using Hotel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=510EC15;Database=Hotel;Trusted_Connection=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        int numberOfVisit = 0;
        object blockedUser;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            string hashedPassword = HashPassword(password);
            string role = CheckCredentials(login, hashedPassword);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT isBlocked FROM Employees WHERE Login = @Login";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Login", login);

                    blockedUser = command.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка в блокировке: " + ex.Message);
                }
            }
        
            if (blockedUser.ToString() == "True")
            {
                MessageBox.Show("Ваш аккаунт заблокирован.");
                return;
            }

            if (role != null)
            {
                OpenRoleWindow(role);
            }
            else
            {
                numberOfVisit++;
                MessageBox.Show("Неверный логин или пароль осталось попыток: " + (3 - numberOfVisit));
                if (numberOfVisit >= 3)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "UPDATE Employees SET isBlocked = 1 WHERE Login = @Login";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@Login", login);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Вы заблокированы, ваш логин: " + login);
                            }
                            else
                            {
                                MessageBox.Show("Пользователь с таким логином не найден.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка в блокировке: " + ex.Message);
                        }
                    }
                }
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
                    MessageBox.Show("Окно уборщика");
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