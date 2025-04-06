using System;

namespace VRCEntryBoard.HMI.Exception
{
    internal interface IExceptionNotifier
    {
        void NotifyFatalError(string title, string message, System.Exception ex);
        bool NotifyRecoverableError(string title, string message, System.Exception ex);
    }
}