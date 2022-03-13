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

        public List<AllTransfers> GetTransfers()
        {
            RestRequest request = new RestRequest($"{ApiUrl}/account/{UserId}/transfers");

            IRestResponse<List<AllTransfers>> response = client.Get<List<AllTransfers>>(request);

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

        public List<PendingTransfer> GetPendingTransfers()
        {
            RestRequest request = new RestRequest($"{ApiUrl}/account/{UserId}/transfers/pending");

            IRestResponse<List<PendingTransfer>> response = client.Get<List<PendingTransfer>>(request);

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
        public Transfer GetTransferById(int transferId, int userId)
        {
            RestRequest request = new RestRequest($"{ApiUrl}/account/{userId}/transfer/{transferId}/");

            IRestResponse<Transfer> response = client.Get<Transfer>(request);

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

        public bool RequestMoney(int fromUserId, decimal amount)
        {
            TransferMoney transfer = new TransferMoney()
            {
                FromUserId = fromUserId,
                ToUserId = UserId,
                TransferAmount = amount,
            };

            RestRequest request = new RestRequest($"{ApiUrl}/account/transfer/request");
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

        internal bool ApproveRequest(int transferId)
        {
            TransferIdModel newApproval = new TransferIdModel()
            {
                TransferId = transferId,
            };

                RestRequest request = new RestRequest($"{ApiUrl}/account/transfer/request/apporove");
                request.AddJsonBody(newApproval);

                IRestResponse<bool> response = client.Put<bool>(request);

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

        internal bool RejectRequest(int transferId)
        {
            TransferIdModel reject = new TransferIdModel()
            {
                TransferId = transferId,
            };

            RestRequest request = new RestRequest($"{ApiUrl}/account/transfer/request/reject");
            request.AddJsonBody(reject);

            IRestResponse<bool> response = client.Put<bool>(request);

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
