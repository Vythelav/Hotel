using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class AdminWindow : Window
    {
        private string connectionString = "Server=510EC15;Database=Hotel;Trusted_Connection=True;";

        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            LoadBookings();
            LoadRooms();
            LoadGuests();
            LoadPayments();
            LoadEmployees();
        }

        private void LoadBookings()
        {
            string query = @"
                SELECT b.BookingID, g.FirstName + ' ' + g.LastName AS GuestName, r.RoomNumber, 
                       b.CheckInDate, b.CheckOutDate, b.BookingStatus
                FROM Bookings b
                JOIN Guests g ON b.GuestID = g.GuestID
                JOIN Rooms r ON b.RoomID = r.RoomID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                BookingsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadRooms()
        {
            string query = "SELECT * FROM Rooms";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                RoomsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadGuests()
        {
            string query = "SELECT * FROM Guests";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                GuestsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadPayments()
        {
            string query = "SELECT * FROM Payments";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                PaymentsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadEmployees()
        {
            string query = "SELECT * FROM Employees";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                EmployeesDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ConfirmBookingButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                int bookingID = (int)button.Tag; // Получаем ID бронирования из свойства 

                // Обновляем статус бронирования
                UpdateBookingStatus(bookingID, "Подтверждено");

                // Получаем RoomID для этого бронирования
                int roomID = GetRoomIDByBookingID(bookingID);

                if (roomID > 0)
                {
                    // Обновляем статус комнаты на "Занят"
                    UpdateRoomStatus(roomID, "Занят");
                }

                // Обновляем данные в DataGrid
                LoadBookings();
                LoadRooms();
            }
        }

        private void BlockedEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button button)
            {

                if (button.Tag != null && int.TryParse(button.Tag.ToString(), out int employeeID))
                {

                    
                    bool isCurrentlyBlocked = CheckIfEmployeeIsBlocked(employeeID);


                    string newBlockedStatus = isCurrentlyBlocked ? "False" : "True";
                    string newBlockedContent = isCurrentlyBlocked ? "Заблакировать" : "Разблокировать";
                    button.Content = newBlockedContent;
                    UpdateBlockedEmployees(employeeID, newBlockedStatus);

                    
                    LoadPayments();
                }
                else
                {
                    MessageBox.Show("Ошибка: неверный идентификатор сотрудника.");
                }
            }
        }
        private bool CheckIfEmployeeIsBlocked(int employeeID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT isBlocked FROM Employees WHERE EmployeeID = @EmployeeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                object result = command.ExecuteScalar();
                return result.ToString() == "True" ? true : false;
            }
        }

        private void UpdateBlockedEmployees(int employeeId, string status)
        {
            if (employeeId != 1 && employeeId != 2)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE Employees SET isBlocked = @Status WHERE EmployeeID = @EmployeeID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        command.Parameters.AddWithValue("@Status", status);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Статус блокировки успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Пользователя с указанным ID не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка в блокировке: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("нельзя заблокировать главных!");
            }
        }


        private void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                int paymentID = (int)button.Tag; // Получаем ID платежа из свойства Tag

                // Обновляем статус платежа на "Оплачено"
                UpdatePaymentStatus(paymentID, "Оплачено");

                // Обновляем данные в DataGrid
                LoadPayments();
            }
        }

        private void UpdateBookingStatus(int bookingID, string status)
        {
            string query = @"
                UPDATE Bookings
                SET BookingStatus = @Status
                WHERE BookingID = @BookingID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@BookingID", bookingID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Статус бронирования успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Бронирование с указанным ID не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса бронирования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePaymentStatus(int paymentID, string status)
        {
            string query = @"
                UPDATE Payments
                SET Status = @Status
                WHERE PaymentID = @PaymentID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@PaymentID", paymentID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Статус платежа успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Платеж с указанным ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса платежа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int GetRoomIDByBookingID(int bookingID)
        {
            string query = "SELECT RoomID FROM Bookings WHERE BookingID = @BookingID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookingID", bookingID);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении RoomID: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
        }

        private void UpdateRoomStatus(int roomID, string status)
        {
            string query = @"
                UPDATE Rooms
                SET Status = @Status
                WHERE RoomID = @RoomID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@RoomID", roomID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Статус комнаты успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Комната с указанным ID не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса комнаты: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            GuestWindow guestWindow = new GuestWindow();
            guestWindow.Show();
            this.Close();
        }
    }
}