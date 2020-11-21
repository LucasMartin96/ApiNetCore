using System;
using System.Collections;
using System.Collections.Generic;

namespace FirstApi2xd.Contracts.v1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Tags { get; set; }

    }
}