using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PointsAPI.Models;
using PointsAPI.Response;

namespace PointsAPI.Controllers
{
    /// <summary>
    /// Handles request for points
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PointsController : ControllerBase
    {
        /// <summary>
        /// The component that stores the points
        /// </summary>
        private readonly IPointsStore _pointsStore;

        public PointsController(IPointsStore pointsStore)
        {
            _pointsStore = pointsStore;
        }

        /// <summary>
        /// Gets the point balance
        /// </summary>
        /// <returns>A dictionary that maps the payer and the corresponding point balance for this payer</returns>
        [HttpGet]
        public IActionResult GetPointsBalance()
        {
            Dictionary<string, int> balance = _pointsStore.GetPointsBalance();
            if (balance.Count == 0) 
            {
                return NotFound(new NotFoundResponse() 
                {
                    Error = "There is no point"
                });
            }
            return Ok(balance);
        }

        /// <summary>
        /// Adds a point transaction to points store
        /// </summary>
        /// <param name="payer">The payer that this point comes from</param>
        /// <param name="points">Point number</param>
        /// <param name="timeStamp">UTC timestamp for the point transaction</param>
        /// <returns>Response result</returns>
        [HttpPut]
        public IActionResult AddPoints(string payer, int? points, DateTime? timeStamp)
        {
            ModelStateDictionary modelMap = new ();
            if (string.IsNullOrEmpty(payer))
            {
                modelMap.AddModelError(nameof(payer), $"{nameof(payer)} is not valid");
                return BadRequest(modelMap);
            }

            if (points == null) 
            {
                modelMap.AddModelError(nameof(points), $"{nameof(points)} is not valid");
                return BadRequest(modelMap);
            }

            if (timeStamp == null) 
            {
                modelMap.AddModelError(nameof(timeStamp), $"{nameof(timeStamp)} is not valid");
                return BadRequest(modelMap);
            }

            _pointsStore.AddPoints(payer, points.Value, timeStamp.Value);
            return Ok();
        }

        /// <summary>
        /// Spends a number of points
        /// </summary>
        /// <param name="points">The points that will be spent</param>
        /// <returns>An array of payers and their corresponding points spent on them</returns>
        [HttpPost]
        public IActionResult SpendPoints(int? points)
        {
            ModelStateDictionary modelMap = new();
            if (points == null || points.Value <= 0)
            {
                modelMap.AddModelError(nameof(points), $"{nameof(points)} is not valid");
                return BadRequest(modelMap);
            }

            Dictionary<string, int>? consumption = _pointsStore.SpendPoints(points.Value);
            if (consumption == null) 
            {
                return NotFound(new NotFoundResponse()
                {
                    Error = "There are not enough points to spend"
                });
            }
            IList<SpendPointsResponse> response = new List<SpendPointsResponse>();

            foreach (KeyValuePair<string, int> spence in consumption)
            {
                response.Add(new SpendPointsResponse()
                {
                    Payer = spence.Key,
                    Points = spence.Value
                });
            }
            return Ok(response);
        }
    }
}
