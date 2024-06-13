using Microsoft.AspNetCore.Mvc;

namespace StarSolitaireTournament.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthzController : ControllerBase
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region references and intialization

        private readonly ILogger<HealthzController> logger;

        public HealthzController(ILogger<HealthzController> _logger)
        {
            logger = _logger;
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region apis

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            logger.Log(LogLevel.Information, "Status Called");
            return Ok();
        }



        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
