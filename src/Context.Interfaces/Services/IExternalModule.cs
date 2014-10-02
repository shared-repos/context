using System;
using System.Collections.Generic;

namespace Context.Interfaces.Services
{
    public interface IExternalModule
    {
        void Start();
        void Stop();
        void Keep();
    }
}
