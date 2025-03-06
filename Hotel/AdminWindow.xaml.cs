using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class AdminWindow : Window
    {
        private string connectionString = "Server=LAPTOP-V0AGQKUF\\SLAUUUIK;Database=Hotel;Trusted_Connection=True;";

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
                int bookingID = (int)button.Tag; // Получаем ID бронирования из свойства Tag

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
    }
}