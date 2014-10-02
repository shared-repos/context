using System;

namespace Context.Interfaces.Configuration
{
    public interface IConfigurationPage
    {
        void ApplyChanges();
        void DiscardChanges();
        void ValidateControls();
        bool Modified { get; }
        string[] FullPath { get; }
    }
}
