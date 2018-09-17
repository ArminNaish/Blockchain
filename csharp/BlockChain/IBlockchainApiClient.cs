using System.Collections.Generic;
using BlockChain.Domain.Model;

namespace BlockChain
{
    public interface IBlockchainApiClient
    {
        ICollection<Blockchain> FindBlockchains(IEnumerable<Node> nodes);
    }
}