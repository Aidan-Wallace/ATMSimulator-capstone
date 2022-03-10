using RestSharp;
using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

        public decimal GetBalance()
        {
            Account account = new Account();

            RestRequest request = new RestRequest($"{ApiUrl}/account/{UserId}");

            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - Unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - Received not success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public bool SendMoney(int toUserId, decimal amount)
        {
            TransferMoney transfer = new TransferMoney()
            {
                FromUserId = UserId,
                ToUserId = toUserId,
                TransferAmount = amount,
            };

            RestRequest request = new RestRequest($"{ApiUrl}/account/transfer");
            request.AddJsonBody(transfer);

            IRestResponse<bool> response = client.Post<bool>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - Unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - Received not success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public List<User> GetUsers()
        {
            RestRequest request = new RestRequest($"{ApiUrl}/user");

            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - Unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - Received not success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }
    }
}
