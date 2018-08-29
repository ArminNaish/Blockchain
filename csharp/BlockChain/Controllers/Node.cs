using System;

namespace BlockChain.Controllers
{
    public class Node
    {
        public Node(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}