using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Hotel
{
    public partial class GuestWindow : Window
    {
        private string connectionString = "Server=LAPTOP-V0AGQKUF\\SLAUUUIK;Database=Hotel;Trusted_Connection=True;";

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

                string firstName = guestInfoWindow.FirstName;
                string lastName = guestInfoWindow.LastName;
                string email = guestInfoWindow.Email;
                string phoneNumber = guestInfoWindow.PhoneNumber;
                string passport = guestInfoWindow.Passport;
                DateTime dateOfBirth = guestInfoWindow.DateOfBirth;

                int guestID = AddGuest(firstName, lastName, phoneNumber, email, passport, dateOfBirth);

                if (guestID > 0)
                {
                    AddBooking(guestID, roomID, DateTime.Now, DateTime.Now.AddDays(1)); 
                    MessageBox.Show("Заявка на бронирование отправлена. Ожидайте подтверждения администратора.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadAvailableRooms();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении гостя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int AddGuest(string firstName, string lastName, string phoneNumber, string email, string passport, DateTime dateOfBirth)
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
                command.Parameters.AddWithValue("@PassportNumber", passport);
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

        private void AddBooking(int guestID, int roomID, DateTime checkInDate, DateTime checkOutDate)
        {
            string query = @"
                INSERT INTO Bookings (GuestID, RoomID, CheckInDate, CheckOutDate, BookingStatus)
                VALUES (@GuestID, @RoomID, @CheckInDate, @CheckOutDate, 'Ожидание')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GuestID", guestID);
                command.Parameters.AddWithValue("@RoomID", roomID);
                command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                command.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении бронирования: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoginAsEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}