using System.ComponentModel;

namespace TP.Upgrade.Domain.Enums
{
    public enum OrderStatus : byte
    {
        /// <summary>
        /// Pending
        /// </summary>
        [Description("Waiting for confirmation")]
        Pending = 1,
        /// <summary>
        /// Confirmed
        /// </summary>
        [Description("Confirmed")]
        VendorConfirmed = 2,
        /// <summary>
        /// Ticket Transfered 
        /// </summary>
        [Description("Ticket Transfered")]
        TicketUploaded = 3,
        /// <summary>
        /// Cancelled 
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 4,
        /// <summary>
        /// Refunded 
        /// </summary>
        [Description("Refunded")]
        Refunded = 5,
        /// <summary>
        /// In Problem 
        /// </summary>
        [Description("In Problem")]
        InProblem = 6,
        /// <summary>
        /// Payment Initiated
        /// </summary>
        [Description("Payment Initiated")]
        GetPayRequest = 7,
        /// <summary>
        /// Completed 
        /// </summary>
        [Description("Completed")]
        PaymentCompleted = 8,
        /// <summary>
        /// Penality Applied 
        /// </summary>
        [Description("Penality Applied")]
        Penalities = 9,
        /// <summary>
        /// Complete Order
        /// </summary>
        [Description("Complete Order")]
        Complete = 10
    }
}
