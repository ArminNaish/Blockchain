using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Domain.Model;
using BlockChain.Domain;
using BlockChain.Controllers.DTO;

namespace BlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainsController : ControllerBase
    {
        private readonly IBlockchainRepository repository;
        private readonly Node node;

        public BlockchainsController(IBlockchainRepository repository, Node node)
        {
            this.repository = repository;
            this.node = node;
        }

        // GET api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce
        [HttpGet("{id}")]
        public dynamic GetById(Guid id)
        {
            var blockchain = repository.GetBlockChainById(id);
            return new {
                Chain = blockchain.Chain,
                Length = blockchain.Chain.Count
            };
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/transactions
        [HttpPost("{id}/transactions")]
        public dynamic CreateTransaction(Guid id, CreateTransaction dto)
        {
            var blockchain = repository.GetBlockChainById(id);
            var index = blockchain.NewTransaction(dto.Sender, dto.Recepient, dto.Amount);
            // todo: implement save changes!!!
            // This method should return 201 - created with the url of the created resource
            return Ok(new {Message = $"Transaction will be added to Block {index}"});
        }

        // POST api/blockchains/a6407387-7a9c-4343-99f1-98be977252ce/mine
        [HttpPost("{id}/mine")]
        public dynamic Mine(Guid id)
        {
            var blockchain = repository.GetBlockChainById(id);
            var block = blockchain.Mine(node.Id);
            // todo: implement save changes!!!
            return Ok(new {
                Message = "New block forged",
                Index = block.Index,
                Transactions = block.Transactions,
                Proof = block.Proof,
                PreviousHash = block.PreviousHash
            });
        }
    }
}
