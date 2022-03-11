using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                // View your current balance
                GetBalance();
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                ViewTransfers();
            }

            if (menuSelection == 3)
            {
                // View your pending requests
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                SendMoney();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }

        private void GetBalance()
        {
            decimal balance = tenmoApiService.GetBalance();

            Console.WriteLine($"Your current account balance is: {balance.ToString("C2")}\n");
            console.Pause();
        }

        public void ViewTransfers()
        {
            List<CompletedTransfer> transfers = tenmoApiService.GetTransfers();

            console.PrintGetTransfersMenu(transfers);
            int menuOption = console.PromptForInteger("Please enter transfer ID to view details (0 to cancel)");

            if (menuOption == 0) return;

            Transfer transfer = tenmoApiService.GetTansferById(menuOption);

            console.PrintTransferDetails(transfer);
            console.Pause();
        }


        public void SendMoney()
        {
            List<User> users = tenmoApiService.GetUsers();
            int toUserId;

            console.PrintSendMoneyMenu(tenmoApiService.UserId, users);


            while (true)
            {
                toUserId = console.PromptForInteger("Please enter id of the user you are sending to[0]", 1001, 1999);
                if (toUserId == tenmoApiService.UserId)
                {
                    Console.WriteLine("Please enter a valid ID. You cannot send money to yourself.");
                    continue;
                }
                foreach (User user in users)
                {
                    if (user.UserId == toUserId)
                    {

                    }
                }

            }

            decimal amount = console.PromptForDecimal("Enter amount to send");

            if (tenmoApiService.SendMoney(toUserId, amount))
            {
                Console.WriteLine("Transfer was successful.\n");
            }
            else
            {
                Console.WriteLine($"Transfer was not successful.\n");
            }
            console.Pause();
        }
    }
}
