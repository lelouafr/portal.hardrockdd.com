namespace DB
{

    public enum AccessLevelEnum
    {
        Denied,
        Read,
        Write,
        Full
    }
    public enum WebUserStatusEnum
    {
        Active,
        Disabled
    }

    public enum FormAccessLevelEnum
    {
        Denied,
        Read,
        Write,
        Full
    }

    public enum ErrorLogStatusEnum
    {
        Error,
        Critical,
        Dismiss,
        Fixed
    }

    public enum SessionAccess
    {
        Denied,
        View,
        Edit
    }

    public enum NotificationLevelEnum
    {
        Info = 1,
        Warning = 2,
        Danger = 3
    }

}