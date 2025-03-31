using System;

namespace VRCEntryBoard.HMI.Exception
{
    public interface IExceptionNotifier
    {
        void NotifyFatalError(string title, string message, System.Exception ex);
        void NotifyRecoverableError(string title, string message, System.Exception ex);
    }
}