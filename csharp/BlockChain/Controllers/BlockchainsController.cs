using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Domain.Model;
using BlockChain.Domain;
using JsonFlatFileDataStore;

namespace BlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainsController : ControllerBase
    {
        private readonly IBlockchainRepository repository;
        private readonly IDataStore store;

        public BlockchainsController(IBlockchainRepository repository, IDataStore store)
        {
            this.repository = repository;
            this.store = store;
        }

        // GET api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce
        [HttpGet("{id}")]
        public dynamic GetById(Guid id)
        {
            var blockchain = store.GetItem<Blockchain>(id.ToString());
            if (blockchain == null)
                return NotFound($"Blockchain not found: {id}");

            return new
            {
                Chain = blockchain.Chain,
                Length = blockchain.Chain.Count
            };
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/transactions
        [HttpPost("{id}/transactions")]
        public async Task<dynamic> CreateTransaction(Guid id, dynamic data)
        {
            var blockchain = store.GetItem<Blockchain>(id.ToString());
            if (blockchain == null)
                return NotFound($"Blockchain not found: {id}");

            var index = blockchain.NewTransaction(data.Sender, data.Recepient, data.Amount);
            await store.ReplaceItemAsync(blockchain.Id.ToString(), blockchain, upsert: true);
            return Ok(new
            {
                Message = $"Transaction will be added to Block {index}"
            });
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/mine
        [HttpPost("{id}/mine")]
        public async Task<dynamic> Mine(Guid id)
        {
            var blockchain = store.GetItem<Blockchain>(id.ToString());
            if (blockchain == null)
                return NotFound($"Blockchain not found: {id}");

            var block = blockchain.Mine();
            await store.ReplaceItemAsync(blockchain.Id.ToString(), blockchain, upsert: true);

            return Ok(new
            {
                Message = "New block forged",
                Index = block.Index,
                Transactions = block.Transactions,
                Proof = block.Proof,
                PreviousHash = block.PreviousHash
            });
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/register
        [HttpPost("{id}/register")]
        public dynamic Register(Guid id, string address)
        {
            if (Uri.TryCreate(address, UriKind.Absolute, out var node))
                return BadRequest("Address must not be empty");

            var blockchain = store.GetItem<Blockchain>(id.ToString());
            if (blockchain == null)
                return NotFound($"Blockchain not found: {id}");

            blockchain.Register(node);

            return Ok(new {
                Message = "New nodes have been added",
                TotalNodes = blockchain.Nodes
            });
        }
    }
}
