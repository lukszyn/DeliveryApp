using System;

namespace DeliveryApp
{
    public enum MessageType
    {
        Error,
        Success
    }

    public interface IIoHelper
    {
        bool CheckIfNegative(decimal amount);
        void DisplayInfo(string message, MessageType color);
        decimal GetDecimalFromUser(string message);
        int GetIntFromUser(string message);
        string GetTextFromUser(string message);
        uint GetUintFromUser(string message);
        bool ValidateEmail(string email);
        bool ValidatePassword(string password);
        bool ValidatePhoneNumber(string phoneNumber);
    }

    public class IoHelper : IIoHelper
    {
        public void DisplayInfo(string message, MessageType color)
        {
            Console.Clear();

            if (color == MessageType.Error)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not na integer - try again.\n");
            }

            return number;
        }

        public uint GetUintFromUser(string message)
        {
            uint number;

            while (!uint.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not a positive integer - try again.\n");
            }

            return number;
        }

        public decimal GetDecimalFromUser(string message)
        {
            decimal number;

            while (!decimal.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not a floating point number - try again.\n");
            }

            return number;
        }

        public bool ValidateEmail(string email)
        {
            return email.Contains("@");
        }

        public bool ValidatePassword(string password)
        {
            return password.Length >= 6;
        }

        public bool ValidatePhoneNumber(string phoneNumber)
        {
            return phoneNumber.Length == 9 && int.TryParse(phoneNumber, out _);
        }

        public bool CheckIfNegative(decimal amount)
        {
            return amount <= 0;
        }

    }
}
