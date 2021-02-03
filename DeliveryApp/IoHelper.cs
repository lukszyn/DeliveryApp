using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp
{
    public enum MessageType
    {
        ERROR,
        SUCCESS
    }

    public class IoHelper
    {
        public void DisplayInfo(string message, MessageType color)
        {
            Console.Clear();

            if (color == MessageType.ERROR)
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
            int number;
            return phoneNumber.Length == 9 && int.TryParse(phoneNumber, out number);
        }

        public bool CheckIfNegative(decimal amount)
        {
            return amount <= 0;
        }

    }
}
