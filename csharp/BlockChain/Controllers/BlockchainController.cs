using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Domain.Model;
using BlockChain.Domain;

namespace BlockChain.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlockchainsController : ControllerBase
    {
        private readonly Blockchain blockchain;
        private readonly IBlockchainApiClient apiClient;

        public BlockchainsController(Blockchain blockchain, IBlockchainApiClient apiClient)
        {
            if (blockchain == null)
                throw new ArgumentNullException("Blockchain must not be null");
            if (apiClient == null)
                throw new ArgumentNullException("ApiClient must not be null");
            this.blockchain = blockchain;
            this.apiClient = apiClient;
        }

        // GET blockchain/chain
        [HttpGet("chain")]
        public dynamic Get()
        {
            return new
            {
                chain = blockchain.Blocks,
                length = blockchain.Blocks.Count
            };
        }

        // POST blockchain/transaction/new
        [HttpPost("transactions/new")]
        public dynamic CreateTransaction(dynamic data)
        {
            if (!data.HasProperties("Sender", "Recipient", "Amout"))
                BadRequest("Missing values");

            var index = blockchain.NewTransaction(data.Sender, data.Recepient, data.Amount);
            return Ok(new
            {
                message = $"Transaction will be added to block {index}"
            });
        }

        // POST blockchain/mine
        [HttpPost("{id}/mine")]
        public dynamic Mine()
        {
            var block = blockchain.Mine();
            return Ok(new
            {
                message = "New block forged",
                index = block.Index,
                transactions = block.Transactions,
                proof = block.Proof,
                previous_hash = block.PreviousHash
            });
        }

        // POST blockchain/nodes/register
        [HttpPost("nodes/register")]
        public dynamic Register(string address)
        {
            if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
                return BadRequest("Address must not be empty");

            blockchain.Register(uri);

            return Ok(new
            {
                message = "New node has been added",
                total_nodes = blockchain.Nodes
            });
        }

        // POST blockchain/nodes/resolve
        [HttpPost("nodes/resolve")]
        public dynamic Resolve()
        {

            var otherChains = apiClient.FindChainsInNetwork(blockchain.Nodes);
            var replaced = blockchain.ResolveConflicts(otherChains);
            if (replaced)
            {
                return new
                {
                    message = "Our chain was replaced",
                    new_chain = blockchain.Blocks
                };
            }

            return new
            {
                message = "Our chain is authoritative",
                chain = blockchain.Blocks
            };
        }
    }
}
