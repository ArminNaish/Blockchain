using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BlockChain.Domain;
using BlockChain.Domain.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BlockChain
{
    public class BlockchainApiClient : IBlockchainApiClient
    {
        public IEnumerable<IList<Block>> FindChainsInNetwork(IEnumerable<Node> nodes)
        {
            var chains = new List<IList<Block>>();
            foreach (var node in nodes)
            {
                try
                {
                    chains.Add(GetChain(node));
                }
                catch (Exception) { }
            }
            return chains;
        }

        private IList<Block> GetChain(Node node)
        {
            var client = new RestClient(node.Address);

            var request = new RestRequest("blockchain/chain", Method.GET);
            request.AddHeader("accept", "application/json");

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Failed to grab blockchain from node: {node.Address.ToString()}");

            var jsonObject = JObject.Parse(response.Content);
            var length = jsonObject
                .SelectToken("length")
                .Value<long>();

            var chain = jsonObject
                .SelectToken("chain")
                .Value<List<dynamic>>()
                .Select(c => Block.Reconstitute(
                    (long)c.Index, 
                    (long)c.Timestamp,
                    (long)c.Proof, 
                    (string)c.PreviousHash, 
                    ((List<dynamic>)c.Transactions).Select(t => new Transaction(t.Sender, t.Recipient, t.Amount)).ToList())) 
                .ToList();

            if (chain.Count != length)
                throw new Exception("Received blockchain is an invalid length");

            return chain;
        }
    }
}