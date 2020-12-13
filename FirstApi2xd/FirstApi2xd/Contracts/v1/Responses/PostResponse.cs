using System;
using System.Collections;
using System.Collections.Generic;

namespace FirstApi2xd.Contracts.v1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public IEnumerable<TagResponse> Tags { get; set; }
    }
}