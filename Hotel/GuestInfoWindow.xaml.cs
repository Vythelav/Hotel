using System;
using System.Text.RegularExpressions;
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

            if (string.IsNullOrEmpty(FirstNameTextBox.Text) || FirstNameTextBox.Text.Length < 2)
            {
                MessageBox.Show("Имя должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (string.IsNullOrEmpty(LastNameTextBox.Text) || LastNameTextBox.Text.Length < 2)
            {
                MessageBox.Show("Фамилия должна содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (string.IsNullOrEmpty(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (string.IsNullOrEmpty(PhoneNumberTextBox.Text) || !IsValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("Введите корректный номер телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

 
            if (string.IsNullOrEmpty(PassportTextBox.Text) || PassportTextBox.Text.Length < 5)
            {
                MessageBox.Show("Паспортные данные должны содержать не менее 5 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (DateOfBirthPicker.SelectedDate == null || DateOfBirthPicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Выберите корректную дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

   
            if (PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите способ оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        private bool IsValidPhoneNumber(string phoneNumber)
        {
          
            return Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }
    }
}