namespace EmployeeServiceDesk.Domain.Enums;

public enum NotificationType
{
    TicketCreated = 1,
    TicketAssigned = 2,
    StatusChanged = 3,
    PriorityChanged = 4,
    SLARisk = 5,
    SLABreach = 6,
    TicketResolved = 7,
    TicketClosed = 8
}