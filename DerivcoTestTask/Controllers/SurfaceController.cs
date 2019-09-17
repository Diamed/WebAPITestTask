using DerivcoTestTask.Model;
using Microsoft.AspNetCore.Mvc;

namespace DerivcoTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurfaceController : ControllerBase
    {
        /// <summary>
        /// Proceed the map and get all coordinates with water
        /// </summary>
        /// <param name="value">The map</param>
        /// <returns>Returns message with coordinates of water</returns>
        [HttpPost]
        public ActionResult<string> Post([FromBody] string value)
        {
            var map = new Map(value);
            var errorMessage = map.Proceed();
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return errorMessage;
            }
            return map.GetWaterCoordinates();
        }
    }
}
