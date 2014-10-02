using System;
using System.Collections.Generic;

namespace Context.Interfaces.Services
{
    public interface IIdleService
    {
        void RequestRestart();

        void StartTask();

        void EndTask();
    }
}
