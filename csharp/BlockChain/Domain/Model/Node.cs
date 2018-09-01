using System;

namespace BlockChain.Domain.Model
{
    public class Node : ValueObject
    {
        private readonly Uri address;

        public Node(Uri address) 
        {
            this.address = address;
        }

        public Uri Address => address;
    }
}