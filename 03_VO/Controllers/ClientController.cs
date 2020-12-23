using APIRest_ASPNET5.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIRest_ASPNET5.Data.VO;
using APIRest_ASPNET5.Hypermedia.Filters;
using APIRest_ASPNET5.Models;

namespace APIRest_ASPNET5.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/v{version:ApiVersion}/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private IClientBusiness _clientBusiness;

        public ClientController(ILogger<ClientController> logger, IClientBusiness clientBusiness)
        {
            _logger = logger;
            _clientBusiness = clientBusiness;
        }

        [HttpGet("{sortDiretion}/{pageSize}/{page}")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get([FromQuery] string name, string sortDiretion, int pageSize, int page)
        {
            return Ok(_clientBusiness.FindWithPagedSearch(name, sortDiretion, pageSize, page));
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get(long id)
        {
            var client = _clientBusiness.FindById(id);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [HttpGet("findClientByName")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get([FromQuery] string name)
        {
            var client = _clientBusiness.FindByName(name);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [HttpPost]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Post([FromBody] ClientVO client)
        {
            if (client == null) return BadRequest();
            return Ok(_clientBusiness.Create(client));
        }
        
        [HttpPut]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Put([FromBody] ClientVO client)
        {
            if (client == null) return BadRequest();
            return Ok(_clientBusiness.Update(client));
        }

        [HttpPatch("{id}")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Patch(long id)
        {
            var client = _clientBusiness.Disable(id);
            return Ok(client);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _clientBusiness.Delete(id);
            return NoContent();
        }

    }
}
