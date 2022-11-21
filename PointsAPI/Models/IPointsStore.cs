namespace PointsAPI.Models
{
    /// <summary>
    /// Interface for a component that can store points
    /// </summary>
    public interface IPointsStore
    {
        /// <summary>
        /// Adds a point transaction to points store
        /// </summary>
        /// <param name="payer">The payer that this point comes from</param>
        /// <param name="points">Point number</param>
        /// <param name="transactionTimestamp">UTC timestamp for the point transaction</param>
        void AddPoints(string payer, int points, DateTime transactionTimestamp);

        /// <summary>
        /// Spends a number of points
        /// </summary>
        /// <param name="points">The points that will be spent</param>
        /// <returns>A dictionary that maps the payer and the correspondings points from this payer</returns>
        Dictionary<string, int>? SpendPoints(int points);

        /// <summary>
        /// Gets the point balance
        /// </summary>
        /// <returns>A dictionary that maps the payer and the corresponding point balance for this payer</returns>
        Dictionary<string, int> GetPointsBalance();
    }
}
