using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Codout.Apis.Asaas.Core.Response.Base;

namespace Codout.Apis.Asaas.Core.Response
{
    public class ResponseList<T> : BaseResponse
    {
        public bool HasMore { get; }

        public int TotalCount { get; }

        public int Limit { get; }

        public int Offset { get; }

        public List<T> Data { get; }

        public ResponseList(HttpStatusCode httpStatusCode, string content) : base(httpStatusCode, content)
        {
            if (httpStatusCode != HttpStatusCode.OK) return;

            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            HasMore = root.GetProperty("hasMore").GetBoolean();
            TotalCount = root.GetProperty("totalCount").GetInt32();
            Limit = root.GetProperty("limit").GetInt32();
            Offset = root.GetProperty("offset").GetInt32();
            Data = JsonSerializer.Deserialize<List<T>>(root.GetProperty("data").GetRawText(), JsonSerializerConfiguration.Options);
        }
    }
}
