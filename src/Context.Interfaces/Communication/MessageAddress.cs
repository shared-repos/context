using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Communication
{
    public class MessageAddress
    {
        public static char PathSeparator = '/';

        public MessageAddress()
        {
        }

        public MessageAddress(string str)
        {
            ParseTo(str, this);
        }

        public MessageAddress(string resourceAddress, string command)
        {
            ResourceAddress = resourceAddress;
            Command = command;
        }

        public MessageAddress(string resourceAddress, string newName, string command)
        {
            ResourceAddress = resourceAddress;
            NewName = newName;
            Command = command;
        }

        public MessageAddress(string prefix, string resourceAddress, string newName, string command)
        {
            Prefix = prefix;
            ResourceAddress = resourceAddress;
            NewName = newName;
            Command = command;
        }

        public static MessageAddress Parse(string str)
        {
            MessageAddress address = new MessageAddress();
            ParseTo(str, address);
            return address;
        }

        private static void ParseTo(string str, MessageAddress address)
        {
            address.Command = "Copy";

            int i = str.IndexOf(':');
            if (i > 1)
            {
                i++;
                if (i + 2 <= str.Length && str.Substring(i, 2) == "//")
                {
                    i += 2;
                }

                address.Prefix = str.Substring(0, i);
            }
            else
            {
                i = 0;
            }

            if (i >= str.Length)
            {
                return;
            }

            if (str[i] == '?')
            {
                try
                {
                    string queryString = str.Substring(i + 1);
                    ParseParameters(address, queryString);
                    if (address.Parameters.ContainsKey("Command"))
                    {
                        return;
                    }
                    address.Parameters = null;
                }
                catch
                {
                }

                address.Command = "Delete";
                i++;
            }

            if (i >= str.Length)
            {
                return;
            }

            int j = i;
            i = str.IndexOf('?', j);
            if (i >= 0)
            {
                string queryString = str.Substring(i + 1);
                ParseParameters(address, queryString);
                str = str.Substring(0, i);
            }

            i = str.IndexOf('|', j);
            if (i < 0)
            {
                address.ResourceAddress = str.Substring(j);
                return;
            }

            address.ResourceAddress = str.Substring(j, i - j);
            string newName = str.Substring(i + 1);
            if (newName.IndexOf(PathSeparator) < 0 && !address.ResourceAddress.EndsWith(PathSeparator + newName))
            {
                address.Command = "Rename";
            }
            else
            {
                address.Command = "Move";
            }

            address.NewName = newName;
        }

        private static void ParseParameters(MessageAddress address, string queryString)
        {
            var query = StringHelpers.ParseDictionary(queryString, "=", "&");
            string command;
            if (query.TryGetValue("Command", out command))
            {
                address.Command = command;
            }
            address.Parameters = query;
        }

        public string ResourceAddress { get; set; }

        public string NewResourceAddress
        {
            get
            {
                if (IsMove)
                {
                    return NewName;
                }
                else
                {
                    string parentAddress = GetParentAddress(ResourceAddress);
                    string newAddress = parentAddress + NewName;
                    if (IsFolder)
                    {
                        newAddress += PathSeparator;
                    }

                    return newAddress;
                }
            }
        }

        public string NewName { get; set; }

        public string Prefix { get; set; }

        public string Command { get; set; }

        public bool IsQuery
        {
            get
            {
                return Command != null && string.Compare(Command, "Query", true) == 0;
            }
        }

        public bool IsCopy
        {
            get
            {
                return string.IsNullOrEmpty(Command) || string.Compare(Command, "Copy", true) == 0;
            }
        }

        public bool IsDelete
        {
            get
            {
                return Command != null && string.Compare(Command, "Delete", true) == 0;
            }
        }

        public bool IsMove
        {
            get
            {
                return Command != null && string.Compare(Command, "Move", true) == 0;
            }
        }

        public bool IsRename
        {
            get
            {
                return Command != null && string.Compare(Command, "Rename", true) == 0;
            }
        }

        public bool IsFolder
        {
            get
            {
                if (ResourceAddress == null)
                {
                    return false;
                }

                int len = ResourceAddress.Length;
                if (len == 0)
                {
                    return false;
                }

                return ResourceAddress[len - 1] == PathSeparator;
            }
        }

        public bool IsMoveOrRename
        {
            get
            {
                return IsMove || IsRename;
            }
        }

        private IDictionary<string, string> parameters;

        public IDictionary<string, string> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new Dictionary<string, string>();
                }

                return parameters;
            }
            private set
            {
                parameters = value;
            }
        }

        public static string Combine(string folder, string fileName)
        {
            return folder + fileName.Trim(PathSeparator);
        }

        public static string GetFileName(string path)
        {
            if (path.EndsWith(MessageAddress.PathSeparator.ToString()))
            {
                return string.Empty;
            }

            int i = path.LastIndexOf(MessageAddress.PathSeparator);
            if (i < 0)
            {
                return path;
            }

            return path.Substring(i + 1);
        }

        public static string GetFolderName(string path)
        {
            if (path.EndsWith(PathSeparator.ToString()))
            {
                return path;
            }

            int i = path.LastIndexOf(PathSeparator);
            if (i < 0)
            {
                return string.Empty;
            }

            return path.Substring(0, i + 1);
        }

        public static string GetParentFolderName(string path)
        {
            return GetFolderName(path.TrimEnd(PathSeparator));
        }

        public static string GetParentAddress(string address)
        {
            if (address == null)
            {
                return null;
            }

            int len = address.Length;
            if (len == 0)
            {
                return string.Empty;
            }

            if (address[len - 1] == PathSeparator)
            {
                address = address.Substring(0, len - 1);
            }

            int i = address.LastIndexOf(PathSeparator);
            if (i < 0)
            {
                return string.Empty;
            }

            return address.Substring(0, i + 1);
        }

        public static string ChangeExtension(string address, string oldExtension, string newExtension)
        {
            if (address.EndsWith(oldExtension))
            {
                address = address.Substring(0, address.Length - oldExtension.Length) + newExtension;
            }

            return address;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Prefix != null)
            {
                sb.Append(Prefix);
            }

            if (IsDelete)
            {
                sb.Append('?');
            }

            sb.Append(ResourceAddress);

            if (IsQuery)
            {
                Parameters["Command"] = "Query";
            }
            else if (IsMoveOrRename)
            {
                sb.Append('|');
                sb.Append(NewName);
            }

            if (parameters != null)
            {
                char ch = '?';
                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    sb.Append(ch);
                    sb.Append(entry.Key);
                    sb.Append('=');
                    sb.Append(entry.Value);
                    ch = '&';
                }
            }

            return sb.ToString();
        }
    }
}
