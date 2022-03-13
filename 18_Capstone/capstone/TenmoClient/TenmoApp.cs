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
                ViewPendingTransfers();
            }

            if (menuSelection == 4)
            {
                // Send TE bucks

                SendMoney();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                RequestMoney();
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
                    console.PrintError(" Login failed.");
                }
                else
                {
                    console.PrintSuccess(" You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError(" Login failed.");
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
                    console.PrintSuccess(" Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError(" Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError(" Registration was unsuccessful.");
            }
            console.Pause();
        }

        private void GetBalance()
        {
            decimal balance = tenmoApiService.GetBalance();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Your current account balance is: {balance.ToString("C2")}\n");
            Console.ResetColor();
            console.Pause();
        }

        public void ViewTransfers()
        {
            List<AllTransfers> transfers = tenmoApiService.GetTransfers();

            console.PrintGetTransfersMenu(tenmoApiService.Username, transfers);
            int menuOption = console.PromptForInteger("Please enter transfer ID to view details (Enter to cancel)", 3001, 3099, 0);

            if (menuOption == 0) return;

            Transfer transfer = tenmoApiService.GetTransferById(menuOption, tenmoApiService.UserId);

            if (transfer.TransferId == 0)
            {
                console.PrintError(" There was an error retrieving the information.");
            }
            else
            {
                console.PrintTransferDetails(transfer);
            }
            console.Pause();
        }

        public void ViewPendingTransfers()
        {
            List<PendingTransfer> transfers = tenmoApiService.GetPendingTransfers();

            console.PrintPendingTransfersMenu(transfers);
            int menuOption = console.PromptForInteger("Please enter transfer ID to approve/reject(0 to cancel)", 0);

            if (menuOption == 0) return;

            HandlePendingTransfers(menuOption);

            /*
            Transfer transfer = tenmoApiService.GetTransferById(menuOption, tenmoApiService.UserId);

            if (transfer.TransferId == 0)
            {
                console.PrintError(" There was an error retrieving the information.");
            }
            else
            {
                console.PrintTransferDetails(transfer);
            }
            console.Pause();
            */
        }

        public void HandlePendingTransfers(int transferId)
        {
            console.PrintApproveRejectMenu(transferId);
            
            int menuOption = console.PromptForInteger("Please choose an option", 0, 2, 0);
            if (menuOption == 0) return; 

            if (menuOption == 1)
            {
                if (tenmoApiService.ApproveRequest(transferId))
                {
                    console.PrintSuccess(" Transfer was completed.");
                 
                }
                else
                {
                    console.PrintError(" An error occured.");
                }

            }
            else
            {
                if (tenmoApiService.RejectRequest(transferId))
                {
                    console.PrintSuccess(" Transfer was rejected.");
                }
                else
                {
                    console.PrintError(" An error occurred.");
                }
            }
            console.Pause();
        }

        public void SendMoney()
        {
            List<User> users = tenmoApiService.GetUsers();
            int toUserId = 0;

            console.PrintMoneyMenu(tenmoApiService.UserId, users);

            //to prevent user from entering their own ID or one that does not exist
            bool isIdFound = false;
            while (!isIdFound)
            {
                toUserId = console.PromptForInteger(" Please enter the ID of the user you are sending to", 1001, 1999, 0);
                if (toUserId == tenmoApiService.UserId)
                {

                    console.PrintError(" Please enter a valid ID. You cannot send money to yourself.");
                    continue;
                }

                foreach (User user in users)
                {
                    if (user.UserId == toUserId)
                    {
                        isIdFound = true;
                        continue;
                    }
                }

                if (!isIdFound) console.PrintError("Please enter a valid ID.");
            }

            //Asks for amount to send to selected user
            decimal amount = console.PromptForDecimal(" Enter amount to send");

            if (tenmoApiService.SendMoney(toUserId, amount))
            {
                console.PrintSuccess(" Transfer was successful.\n");
            }
            else
            {
                console.PrintError(" Unable to complete transfer.");
            }
            console.Pause();
        }

        public void RequestMoney()
        {
            List<User> users = tenmoApiService.GetUsers();
            int fromUserId = 0;

            console.PrintMoneyMenu(tenmoApiService.UserId, users);

            //to prevent user from entering their own ID or one that does not exist
            bool isIdFound = false;
            while (!isIdFound)
            {
                fromUserId = console.PromptForInteger(" Please enter the ID of the user you are requesting from", 1001, 1999, 0);
                if (fromUserId == tenmoApiService.UserId)
                {

                    console.PrintError(" Please enter a valid ID. You cannot request money from yourself.");
                    continue;
                }

                foreach (User user in users)
                {
                    if (user.UserId == fromUserId)
                    {
                        isIdFound = true;
                        continue;
                    }
                }

                if (!isIdFound) console.PrintError("Please enter a valid ID.");
            }

            //Asks for amount to send to selected user
            decimal amount = console.PromptForDecimal(" Enter amount you are requesting");

            if (tenmoApiService.RequestMoney(fromUserId, amount))
            {
                console.PrintSuccess(" Transfer request was successful, pending approval.\n");
            }
            else
            {
                console.PrintError(" Unable to complete request.");
            }
            console.Pause();
        }
    }
}
