using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Domain.Model;

namespace BlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainsController : ControllerBase
    {
        // GET api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce
        [HttpGet("{id}")]
        public dynamic GetById(Guid id)
        {
            var blockchain = new Blockchain();
            return new {
                Chain = blockchain.Chain,
                Length = blockchain.Chain.Count
            };
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/transactions
        [HttpPost("{id}/transactions")]
        public dynamic CreateTransaction()
        {
            return "We'll add a new transaction";
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/mine
        [HttpPost("{id}/mine")]
        public dynamic Mine()
        {
            return "We'll mine a new Block";
        }
    }
}
