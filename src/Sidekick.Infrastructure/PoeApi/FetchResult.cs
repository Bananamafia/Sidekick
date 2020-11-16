using System;
using System.Collections.Generic;

namespace Sidekick.Infrastructure.PoeApi
{
    public class FetchResult<T>
    {
        public List<T> Result { get; set; }

        public string Id { get; set; }

        public int Total { get; set; }

        public Uri Uri { get; set; }
    }
}