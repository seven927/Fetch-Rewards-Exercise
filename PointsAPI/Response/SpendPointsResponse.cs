namespace PointsAPI.Response
{
    /// <summary>
    /// Response type for spending points
    /// </summary>
    public class SpendPointsResponse
    {
        /// <summary>
        /// Payer name
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// Point number
        /// </summary>
        public int Points { get; set; }
    }
}
