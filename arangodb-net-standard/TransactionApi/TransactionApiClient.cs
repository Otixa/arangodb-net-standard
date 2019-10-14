﻿using ArangoDBNetStandard.Transport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.TransactionApi
{
    public class TransactionApiClient: ApiClientBase
    {
        private IApiClientTransport _client;
        private readonly string _transactionApiPath = "_api/transaction";

        public TransactionApiClient(IApiClientTransport client)
        {
            _client = client;
        }

        public async Task<PostTransactionResponse<T>> PostTransactionAsync<T>(PostTransactionRequest request)
        {
            var content = GetStringContent(request, true, true);
            using (var response = await _client.PostAsync(_transactionApiPath, content))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    return DeserializeJsonFromStream<PostTransactionResponse<T>>(stream);
                }
                var error = DeserializeJsonFromStream<ApiErrorResponse>(stream);
                throw new ApiErrorException(error);
            }
        }
    }
}
