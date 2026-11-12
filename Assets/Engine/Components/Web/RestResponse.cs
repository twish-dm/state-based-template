using UnityEngine;

namespace StateEngine.Components.Web
{
    public readonly struct RestResponse
    {
        public RestResponse(long statusCode, bool isSuccess, string body, string error)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Body = body;
            Error = error;
        }

        public long StatusCode { get; }

        public bool IsSuccess { get; }

        public string Body { get; }

        public string Error { get; }

        public T ReadJson<T>() => JsonUtility.FromJson<T>(Body);

        public override string ToString() => IsSuccess ? $"{StatusCode}" : $"{StatusCode}: {Error}";
    }
}
