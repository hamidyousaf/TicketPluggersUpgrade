namespace TP.Upgrade.Domain.Enums
{
    public enum TicketSpliting : int
    {
        DontSplitTickets = 0,
        DontLeaveSingleTicket = 1,
        AvoidLeaving1Or3Tickets = 2,
        AvoidOddNumbers = 3,
        AvoidEvenNumbers = 4,
        AvoidOneAndThree = 5,
        AvoidOne = 6
    }
}
