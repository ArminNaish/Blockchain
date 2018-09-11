using System;

namespace BlockChain.Domain.Model
{
    public class Node : ValueObject
    {
        private readonly Uri address;

        public Node(string address)
         : this(new Uri(address))
        {
        }

        public static Node Localhost()
        {
            return new Node(@"http://127.0.0.1:5000/");
        }

        public Node(Uri address) 
        {
            this.address = address;
        }

        public Uri Address => address;
    }
}