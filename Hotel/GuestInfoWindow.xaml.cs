using System;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class GuestInfoWindow : Window
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string PassportNumber { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string PaymentMethod { get; private set; } 

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
                DateOfBirthPicker.SelectedDate == null ||
                PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FirstName = FirstNameTextBox.Text;
            LastName = LastNameTextBox.Text;
            Email = EmailTextBox.Text;
            PhoneNumber = PhoneNumberTextBox.Text;
            PassportNumber = PassportTextBox.Text;
            DateOfBirth = DateOfBirthPicker.SelectedDate.Value;

            PaymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            this.DialogResult = true;
            this.Close();
        }
    }
}