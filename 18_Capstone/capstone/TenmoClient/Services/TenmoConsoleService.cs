﻿using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...
        public void PrintSendMoneyMenu(List<User> users)
        {
            Console.Clear();
            Console.WriteLine("|-------------- Users --------------|");
            Console.WriteLine("|    Id | Username                  |");
            Console.WriteLine("|-------+---------------------------|");

            // Loop to display users goes here:
            foreach (User user in users)
            {
                string template = $"|  {user.UserId} | {user.Username.PadRight(26)}|";
                Console.WriteLine(template);
            }
            Console.WriteLine("|-----------------------------------|");
        }

        public void PrintGetTransfersMenu(List<CompletedTransfer> transfers)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------------");
            Console.WriteLine(" Transfers");
            Console.WriteLine(" ID             From/To              Amount ");
            Console.WriteLine(" -------------------------------------------");

            //Loop to display users goes here:
            foreach (CompletedTransfer transfer in transfers)
            {
                // transfer id
                // transfer type
                // amount
                Console.WriteLine($" {transfer.TransferId}          {transfer.Type.PadRight(6)}{transfer.Username.PadRight(15)}{transfer.Amount.ToString("C2").PadLeft(6)}");
            }
            Console.WriteLine("---------");
        }

        internal void PrintTransferDetails(int menuOption)
        {
            throw new NotImplementedException();
        }
    }
}
