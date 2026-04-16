namespace Warden.CLI.Domain.Enums
{
    public enum ExitCode
    {
        Success = 0,
        PartialSuccess = 1,
        InvalidPath = 2,
        FileNotFound = 3, 
        AccessDenied = 4, 
        FileInUse = 5, 
        WatcherFailed = 6,
        InvalidConfiguration = 7, 
        UserCancelled = 8,
        NothingToDo = 9,
        UnhandledError = 10
    }
}