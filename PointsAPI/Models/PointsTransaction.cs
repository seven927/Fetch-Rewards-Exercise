namespace PointsAPI.Models
{
    /// <summary>
    /// Represents a piece of points transaction
    /// </summary>
    public class PointsTransaction
    {
        /// <summary>
        /// Payer name
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// Point number
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// UTC timestamp for the transaction
        /// </summary>
        public DateTime TransactionTimstamp { get; set; }
    }
}
