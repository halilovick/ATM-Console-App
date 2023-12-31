﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading;

namespace ATMConsoleApp
{
    public static class Utility
    {
        private static long transactionCounter;
        public static long GetTransactionId()
        {
            return ++transactionCounter;
        }

        public static string GetPINInput(string prompt, List<ConsoleKeyInfo> consoleKeys = null)
        {
            bool isPrompt = true;
            string asterics = "";
            int index = 0;

            StringBuilder input = new StringBuilder();

            while (true)
            {
                if (isPrompt)
                    Console.WriteLine(prompt);
                isPrompt = false;

                ConsoleKeyInfo inputKey = consoleKeys != null ? consoleKeys[index] : Console.ReadKey(true);
                index = index + 1;

                if (inputKey.Key == ConsoleKey.Enter)
                {
                    if (input.Length == 4)
                    {
                        break;
                    }
                    else
                    {
                        PrintMessage("\nPlease enter 4 digits.", false);
                        input.Clear();
                        isPrompt = true;
                        if (consoleKeys != null) return "Please enter 4 digits.";
                        continue;
                    }
                }
                if (inputKey.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                }
                else if (inputKey.Key != ConsoleKey.Backspace)
                {
                    input.Append(inputKey.KeyChar);
                    Console.Write(asterics + "*");
                }

            }
            return input.ToString();
        }

        public static void PrintMessage(string msg, bool success = true, bool pressEnter = true)
        {
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
            if (pressEnter)
            {
                PressEnterToContinue();
            }
        }
        public static string GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        public static void PrintDotAnimation()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
        }
        public static void PressEnterToContinue()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
        public static T Convert<T>(string prompt)
        {
            bool valid = false;
            string userInput;

            while (!valid)
            {
                userInput = GetUserInput(prompt);

                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(userInput);
                    }
                    else
                    {
                        return default;
                    }
                }
                catch
                {
                   
                    PrintMessage("Invalid input. Try again.", false, false);
                    return default;
                }
            }
            return default;
        }
    }
}

