using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Hotel
{
    public partial class GuestWindow : Window
    {
        private string connectionString = "Server=510EC15;Database=Hotel;Trusted_Connection=True;";

        public GuestWindow()
        {
            InitializeComponent();
            LoadAvailableRooms();
        }

        private void LoadAvailableRooms()
        {
            string query = "SELECT RoomID, RoomNumber, RoomType, Capacity, Price, Status FROM Rooms WHERE Status = 'Свободен'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                RoomsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void BookRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите номер для бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GuestInfoWindow guestInfoWindow = new GuestInfoWindow();
            if (guestInfoWindow.ShowDialog() == true)
            {
                DataRowView selectedRow = (DataRowView)RoomsDataGrid.SelectedItem;
                int roomID = Convert.ToInt32(selectedRow["RoomID"]);
                decimal roomPrice = Convert.ToDecimal(selectedRow["Price"]); // Получаем цену номера

                string firstName = guestInfoWindow.FirstName;
                string lastName = guestInfoWindow.LastName;
                string email = guestInfoWindow.Email;
                string phoneNumber = guestInfoWindow.PhoneNumber;
                string passportNumber = guestInfoWindow.PassportNumber;
                DateTime dateOfBirth = guestInfoWindow.DateOfBirth;
                string paymentMethod = guestInfoWindow.PaymentMethod;

                int guestID = AddGuest(firstName, lastName, phoneNumber, email, passportNumber, dateOfBirth);

                if (guestID > 0)
                {
                    int bookingID = AddBooking(guestID, roomID, DateTime.Now, DateTime.Now.AddDays(1), "Ожидание");

                    if (bookingID > 0)
                    {
                        // Передаем цену номера в метод AddPayment
                        AddPayment(bookingID, paymentMethod, roomPrice);

                        UpdateRoomStatus(roomID, "Забронирован");

                        MessageBox.Show("Заявка на бронирование отправлена. Ожидайте подтверждения администратора.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadAvailableRooms();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении гостя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int AddGuest(string firstName, string lastName, string phoneNumber, string email, string passportNumber, DateTime dateOfBirth)
        {
            string query = @"
                INSERT INTO Guests (FirstName, LastName, PhoneNumber, Email, PassportNumber, DateOfBirth)
                VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @PassportNumber, @DateOfBirth);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                try
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении гостя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
        }

        private int AddBooking(int guestID, int roomID, DateTime checkInDate, DateTime checkOutDate, string bookingStatus)
        {
            string query = @"
                INSERT INTO Bookings (GuestID, RoomID, CheckInDate, CheckOutDate, BookingStatus)
                VALUES (@GuestID, @RoomID, @CheckInDate, @CheckOutDate, @BookingStatus);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GuestID", guestID);
                command.Parameters.AddWithValue("@RoomID", roomID);
                command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                command.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                command.Parameters.AddWithValue("@BookingStatus", bookingStatus);

                try
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении бронирования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
        }

        private void AddPayment(int bookingID, string paymentMethod, decimal amount)
        {
            string query = @"
                INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status)
                VALUES (@BookingID, @Amount, @PaymentMethod, @Status)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookingID", bookingID);
                command.Parameters.AddWithValue("@Amount", amount); // Используем переданную цену номера
                command.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                command.Parameters.AddWithValue("@Status", "Ожидание");

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении платежа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса комнаты: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoginAsEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow employeeLoginWindow = new MainWindow();
            employeeLoginWindow.Show();
            this.Close();
        }
    }
}