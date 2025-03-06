using System;
using System.Windows;

namespace Hotel
{
    public partial class GuestInfoWindow : Window
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Passport { get; private set; }
        public DateTime DateOfBirth { get; private set; }

        public GuestInfoWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text) ||
                string.IsNullOrEmpty(PhoneNumberTextBox.Text) ||
                string.IsNullOrEmpty(PassportTextBox.Text) ||
                DateOfBirthPicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FirstName = FirstNameTextBox.Text;
            LastName = LastNameTextBox.Text;
            Email = EmailTextBox.Text;
            PhoneNumber = PhoneNumberTextBox.Text;
            Passport = PassportTextBox.Text;
            DateOfBirth = DateOfBirthPicker.SelectedDate.Value;

            this.DialogResult = true;
            this.Close();
        }
    }
}