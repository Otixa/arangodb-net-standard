﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.Transport
{
    /// <summary>
    /// A transport layer for communicating with an ArangoDB host.
    /// </summary>
    public interface IApiClientTransport : IDisposable
    {
        /// <summary>
        /// Send a POST request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<IApiClientResponse> PostAsync(string requestUri, byte[] content);

        /// <summary>
        /// Send a DELETE request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        Task<IApiClientResponse> DeleteAsync(string requestUri);

        /// <summary>
        /// Send a DELETE request with body content.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<IApiClientResponse> DeleteAsync(string requestUri, byte[] content);

        /// <summary>
        /// Send a PUT request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<IApiClientResponse> PutAsync(string requestUri, byte[] content);

        /// <summary>
        /// Send a GET request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        Task<IApiClientResponse> GetAsync(string requestUri);

        /// <summary>
        /// Send a PATCH request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<IApiClientResponse> PatchAsync(string requestUri, byte[] content);

        /// <summary>
        /// Send a HEAD Request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="httpRequestHeaders"></param>
        /// <returns></returns>
        Task<IApiClientResponse> HeadAsync(string requestUri, WebHeaderCollection httpRequestHeaders);
    }
}
