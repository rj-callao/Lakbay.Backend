namespace Piloopinas.Domain;

public static class EventStatusConstants
{
    public const int Draft = 1;
    public const int Published = 2;
    public const int RegistrationOpen = 3;
    public const int RegistrationClosed = 4;
    public const int InProgress = 5;
    public const int Completed = 6;
    public const int Cancelled = 7;
}

public static class RoleConstants
{
    public const int Admin = 1;
    public const int Organizer = 2;
    public const int Rider = 3;
}
