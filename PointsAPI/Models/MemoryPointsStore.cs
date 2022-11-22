namespace PointsAPI.Models
{
    public class MemoryPointsStore : IPointsStore
    {
        /// <summary>
        /// Keeps track of the point transaction history
        /// </summary>
        private readonly IList<PointsTransaction> _transactionsHistory;

        /// <summary>
        /// Keeps tracks of transactions from spending perspective
        /// </summary>
        private readonly IList<PointsTransaction> _spenceList;

        /// <summary>
        /// A dictionary that maps the payer and its corresponding points balance
        /// </summary>
        private readonly Dictionary<string, int> _balance;

        /// <summary>
        /// Total points 
        /// </summary>
        private int _totalPoints;

        public MemoryPointsStore() 
        {
            _transactionsHistory = new List<PointsTransaction>();
            _spenceList = new List<PointsTransaction>();
            _balance = new Dictionary<string, int>();
            _totalPoints = 0;
        }

        /// <summary>
        /// Adds a point transaction to points store
        /// </summary>
        /// <param name="payer">The payer that this point comes from</param>
        /// <param name="points">Point number</param>
        /// <param name="transactionTimestamp">UTC timestamp for the point transaction</param>
        /// <returns>Whether the points are added successfully</returns>
        public bool AddPoints(string payer, int points, DateTime transactionTimestamp)
        {
            int curBalanceForPayer = CalculateBalanceUpToTimeStamp(_transactionsHistory, payer, transactionTimestamp);

            if (curBalanceForPayer + points < 0) 
            {
                // Do not allow negative points. Also considers the chronological sequence.
                return false;
            }

            PointsTransaction trans = new()
            {
                Payer = payer,
                Points = points,
                TransactionTimstamp = transactionTimestamp
            };

            AddBasedOnTimestamp(_transactionsHistory, trans);
            AddBasedOnTimestamp(_spenceList, new PointsTransaction() {
                Payer = payer,
                Points = points,
                TransactionTimstamp = transactionTimestamp
            });
            _balance[trans.Payer] = _balance.ContainsKey(trans.Payer) ?
                _balance[trans.Payer] + trans.Points : trans.Points;
            _totalPoints += points;
            return true;
        }

        /// <summary>
        /// Gets the point balance
        /// </summary>
        /// <returns>A dictionary that maps the payer and the corresponding point balance for this payer</returns>
        public Dictionary<string, int> GetPointsBalance()
        {
            return _balance;
        }

        /// <summary>
        /// Spends a number of points
        /// </summary>
        /// <param name="points">The points that will be spent</param>
        /// <returns>A dictionary that maps the payer and the correspondings points from this payer</returns>
        public Dictionary<string, int>? SpendPoints(int points)
        {
            if (points > _totalPoints) 
            {
                return null;
            }

            int sum = 0;
            Dictionary<string, int> consumption = new();
            foreach (PointsTransaction trans in _spenceList)
            {
                //If this is a spence transaction, and there is already positive points in consumption,
                //and the negative points do not exceed the existing positive points, then deduct it. 
                //Otherwise ignore this negative points
                if (trans.Points < 0 && consumption.ContainsKey(trans.Payer) && 
                    Math.Abs(consumption[trans.Payer]) > Math.Abs(trans.Points))
                {
                    sum += trans.Points;
                    consumption[trans.Payer] -= trans.Points;
                    _balance[trans.Payer] -= trans.Points;
                }
                else if(trans.Points > 0)
                {
                    //At most, we can add the total points from the current transaction or the remaining balance 
                    int delta = _balance[trans.Payer] < trans.Points ? _balance[trans.Payer] : trans.Points;

                    //Find out how many extra points we need to get to the required points
                    int diff = points - sum;

                    //Use the min value of the two values
                    int newDelta = Math.Min(delta, diff);

                    sum += newDelta;
                    consumption[trans.Payer] = consumption.ContainsKey(trans.Payer) ?
                    consumption[trans.Payer] - newDelta : -newDelta;

                    //Deduct points from current transaction so that we don't keep using the same transaction
                    trans.Points -= newDelta;

                    //Deduct points from balance
                    _balance[trans.Payer] -= newDelta;

                    if (sum == points) 
                    {
                        break;
                    }
                }
            }

            //Deduct points from total points
            _totalPoints -= points;
 
            //Add spence transactions to transaction list
            AddSpenceTransactions(_transactionsHistory, consumption);

            //Add spence transactions to spence list
            AddSpenceTransactions(_spenceList, consumption);
            return consumption;
        }

        /// <summary>
        /// Calculate the balance for a specific payer up to a specific timestamp
        /// </summary>
        /// <param name="list">Transaction list</param>
        /// <param name="payer">Payer name</param>
        /// <param name="limit">Timestamp limit</param>
        /// <returns>The point balance</returns>
        private static int CalculateBalanceUpToTimeStamp(IList<PointsTransaction> list, string payer, DateTime limit)  
        {
            int balance = 0;
            foreach (PointsTransaction trans in list) 
            {
                if (trans.TransactionTimstamp >= limit) 
                {
                    break;
                }

                if (trans.Payer == payer) 
                {
                    balance += trans.Points;
                }
            }
            return balance;
        }

        /// <summary>
        /// Add a transaction to a sorted transaction list based on timestamps
        /// </summary>
        /// <param name="list">Transaction list</param>
        /// <param name="cur">Current transaction</param>
        private static void AddBasedOnTimestamp(IList<PointsTransaction> list, PointsTransaction cur) 
        {
            int index = 0;
            for (; index < list.Count; index++) 
            {
                if (cur.TransactionTimstamp < list[index].TransactionTimstamp) 
                {
                    break;
                }
            }

            if (index == list.Count)
            {
                list.Add(cur);
            }
            else 
            {
                list.Insert(index, cur);
            }
        }

        /// <summary>
        /// Add spence transactions to transaction list
        /// </summary>
        /// <param name="list">A list of transactions</param>
        /// <param name="consumption">A dictionary that maps payer and the corresponding points spent from this payer</param>
        private static void AddSpenceTransactions(IList<PointsTransaction> list, Dictionary<string, int> consumption)
        {
            foreach (KeyValuePair<string, int> spence in consumption)
            {
                //We might have entries where value is 0 after deducting spence points
                if (spence.Value < 0) 
                {
                    list.Add(new PointsTransaction()
                    {
                        Payer = spence.Key,
                        Points = spence.Value,
                        TransactionTimstamp = DateTime.UtcNow
                    });
                }
            }
        }
    }
}
