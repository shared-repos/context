using System;

namespace Context.Interfaces.UI.CommandBars
{
    public struct ShortcutEntry
    {
        private string m_FirstKey;
        private string m_SecondKey;

        public ShortcutEntry(string firstKey, string secondKey)
        {
            this.m_FirstKey = firstKey;
            this.m_SecondKey = secondKey;
        }

        public string FirstKey
        {
            get
            {
                return m_FirstKey;
            }
        }

        public string SecondKey
        {
            get
            {
                return m_SecondKey;
            }
        }
    }
}
