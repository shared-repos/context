using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Context.Interfaces.Communication
{
    public class SimpleMessage
    {
        private List<MessageAttachment> attachments;
        private NameValueCollection headers;

        public SimpleMessage()
        {
        }

        public SimpleMessage(string body)
        {
            this.Body = body;
        }

        public SimpleMessage(string to, string body)
        {
            this.To = to;
            this.Body = body;
        }

        public static SimpleMessage ToSimpleMessage(object source)
        {
            SimpleMessage message = source as SimpleMessage;
            if (message != null)
            {
                return message;
            }

            MessageAttachment attach = source as MessageAttachment;
            if (attach != null)
            {
                message = new SimpleMessage();
                message.Attachments.Add(attach);
                return message;
            }

            string messageText = source as string;
            if (messageText != null)
            {
                return new SimpleMessage(messageText);
            }

            throw new ArgumentException("Can't convert source to the SimpleMessage object.");
        }

        public string MessageId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Command { get; set; }

        public List<MessageAttachment> Attachments
        {
            get
            {
                if (attachments == null)
                {
                    attachments = new List<MessageAttachment>();
                }

                return attachments;
            }
        }

        public NameValueCollection Headers
        {
            get
            {
                if (headers == null)
                {
                    headers = new NameValueCollection();
                }

                return headers;
            }
        }
    }
}
