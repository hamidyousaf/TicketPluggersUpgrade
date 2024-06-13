using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Upgrade.Domain.Enums
{
    public enum PaymentStatus : byte
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Authorized
        /// </summary>
        Authorized = 2,

        /// <summary>
        /// Paid
        /// </summary>
        Paid = 3,

        /// <summary>
        /// Partially Refunded
        /// </summary>
        PartiallyRefunded = 4,

        /// <summary>
        /// Refunded
        /// </summary>
        Refunded = 5,

        /// <summary>
        /// Voided
        /// </summary>
        Voided = 6,

        /// <summary>
        /// SETTLED
        /// </summary>
        SETTLED = 7,

        /// <summary>
        /// PARTIALLY_SETTLED
        /// </summary>
        PARTIALLY_SETTLED = 8,

        /// <summary>
        /// DECLINED
        /// </summary>
        DECLINED = 9,

        /// <summary>
        /// FAILED
        /// </summary>
        FAILED = 10,

        /// <summary>
        /// CANCELLED
        /// </summary>
        CANCELLED = 11,

        /// <summary>
        /// SETTLING
        /// </summary>
        SETTLING = 12,
    }
}
