using System;
using System.Net;
using System.Runtime.Serialization;

namespace ChatService.Client
{
    [Serializable]
    public class ChatServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ChatServiceException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public ChatServiceException(string message, Exception e, HttpStatusCode statusCode) : base(message, e)
        {
            StatusCode = statusCode;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(StatusCode)}: {StatusCode}";
        }
    }
}