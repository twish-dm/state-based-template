using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace StateEngine.Components.Web
{
    /// <summary>
    /// Thin async wrapper over <see cref="UnityWebRequest"/> for JSON APIs. Must be used from the main thread.
    /// </summary>
    public sealed class RestClient
    {
        private const string k_JsonContentType = "application/json; charset=utf-8";

        private readonly string m_BaseUrl;
        private readonly Dictionary<string, string> m_Headers = new Dictionary<string, string>();

        public RestClient(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.", nameof(baseUrl));

            m_BaseUrl = baseUrl.TrimEnd('/');
        }

        public int TimeoutSeconds { get; set; } = 30;

        public void SetHeader(string name, string value) => m_Headers[name] = value;

        public void RemoveHeader(string name) => m_Headers.Remove(name);

        public void SetBearerToken(string token) => SetHeader("Authorization", $"Bearer {token}");

        public Task<RestResponse> GetAsync(string endpoint, CancellationToken cancellationToken = default) =>
            SendAsync(UnityWebRequest.kHttpVerbGET, endpoint, null, cancellationToken);

        public Task<RestResponse> PostAsync(string endpoint, string json, CancellationToken cancellationToken = default) =>
            SendAsync(UnityWebRequest.kHttpVerbPOST, endpoint, json, cancellationToken);

        public Task<RestResponse> PutAsync(string endpoint, string json, CancellationToken cancellationToken = default) =>
            SendAsync(UnityWebRequest.kHttpVerbPUT, endpoint, json, cancellationToken);

        public Task<RestResponse> DeleteAsync(string endpoint, CancellationToken cancellationToken = default) =>
            SendAsync(UnityWebRequest.kHttpVerbDELETE, endpoint, null, cancellationToken);

        public async Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var request = new UnityWebRequest(BuildUrl(endpoint), method)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = TimeoutSeconds
            };

            if (json != null)
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                request.SetRequestHeader("Content-Type", k_JsonContentType);
            }

            request.SetRequestHeader("Accept", "application/json");
            foreach (KeyValuePair<string, string> header in m_Headers)
                request.SetRequestHeader(header.Key, header.Value);

            await SendWebRequestAsync(request, cancellationToken);

            return new RestResponse(
                request.responseCode,
                request.result == UnityWebRequest.Result.Success,
                request.downloadHandler.text,
                request.error);
        }

        private string BuildUrl(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return m_BaseUrl;

            return endpoint[0] == '/' ? m_BaseUrl + endpoint : $"{m_BaseUrl}/{endpoint}";
        }

        private static async Task SendWebRequestAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            var completion = new TaskCompletionSource<bool>();
            request.SendWebRequest().completed += _ => completion.TrySetResult(true);

            using (cancellationToken.Register(() =>
                   {
                       request.Abort();
                       completion.TrySetCanceled(cancellationToken);
                   }))
            {
                await completion.Task;
            }
        }
    }
}
