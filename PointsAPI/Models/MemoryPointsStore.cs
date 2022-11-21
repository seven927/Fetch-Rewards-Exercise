namespace PointsAPI.Models
{
    public class MemoryPointsStore : IPointsStore
    {
        /// <summary>
        /// A list of point transactions
        /// </summary>
        private IList<PointsTransaction> _transactions;

        public MemoryPointsStore() 
        {
            _transactions = new List<PointsTransaction>();  
        }

        /// <summary>
        /// Adds a point transaction to points store
        /// </summary>
        /// <param name="payer">The payer that this point comes from</param>
        /// <param name="points">Point number</param>
        /// <param name="transactionTimestamp">UTC timestamp for the point transaction</param>
        public void AddPoints(string payer, int points, DateTime transactionTimestamp)
        {
            PointsTransaction trans = new()
            {
                Payer = payer,
                Points = points,
                TransactionTimstamp = transactionTimestamp
            };

            _transactions.Add(trans);
        }

        /// <summary>
        /// Gets the point balance
        /// </summary>
        /// <returns>A dictionary that maps the payer and the corresponding point balance for this payer</returns>
        public Dictionary<string, int> GetPointsBalance()
        {
            Dictionary<string, int> balance = new();
            foreach (PointsTransaction trans in _transactions)
            {
                balance[trans.Payer] = balance.ContainsKey(trans.Payer) ?
                    balance[trans.Payer] + trans.Points : trans.Points;
            }
            return balance;
        }

        /// <summary>
        /// Spends a number of points
        /// </summary>
        /// <param name="points">The points that will be spent</param>
        /// <returns>A dictionary that maps the payer and the correspondings points from this payer</returns>
        public Dictionary<string, int>? SpendPoints(int points)
        {
            int sum = 0;
            Dictionary<string, int> consumption = new();
            foreach (PointsTransaction trans in _transactions)
            {
                if (sum + trans.Points < points)
                {
                    sum += trans.Points;
                    consumption[trans.Payer] = consumption.ContainsKey(trans.Payer) ?
                    consumption[trans.Payer] - trans.Points : -trans.Points;
                }
                else
                {
                    consumption[trans.Payer] = sum - points;
                    sum = points;
                    break;
                }
            }
            if (sum == points)
            {
                AddSpenceTransactions(consumption);
                return consumption;
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Add spence transactions to transaction list
        /// </summary>
        /// <param name="consumption">A dictionary that maps payer and the corresponding points spent from this payer</param>
        private void AddSpenceTransactions(Dictionary<string, int> consumption)
        {
            foreach (KeyValuePair<string, int> spence in consumption)
            {
                _transactions.Add(new PointsTransaction()
                {
                    Payer = spence.Key,
                    Points = spence.Value,
                    TransactionTimstamp = DateTime.UtcNow
                });
            }
        }
    }
}
