using DesafioBlip.Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DesafioBlip.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RepositorioTakeController : ControllerBase
    {
        /// <summary>
        /// Retorna os 5 repositórios mais antigos da Take com a linguagem C#
        /// </summary>
        /// <returns>Retorna um array de RepositorioTake</returns>
        // GET: <RepositorioTakeController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<RepositorioTake> ret = new RepositorioTakeBusiness().GetRepositoriosAntigoCSharp();
                return Ok(ret);
                 
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);

            }


        }

        
    }
}
