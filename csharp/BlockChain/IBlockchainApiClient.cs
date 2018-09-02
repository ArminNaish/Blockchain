using System.Collections.Generic;
using BlockChain.Domain.Model;

namespace BlockChain
{
    public interface IBlockchainApiClient
    {
        IEnumerable<IList<Block>> FindChainsInNetwork(IEnumerable<Node> nodes);
    }
}