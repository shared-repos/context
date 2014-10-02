using System;
using System.Collections.Generic;
using System.IO;

namespace Context.Interfaces.Communication
{
    public class MessageAttachment
    {
        public string FileName { get; set; }

        public string Body { get; set; }

        public byte[] Data { get; set; }

        public Stream Content { get; set; }

        public string MediaType { get; set; }
    }
}
