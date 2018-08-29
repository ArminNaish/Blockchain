namespace BlockChain.Infrastructure.Entities
{
    public class ProofOfWork
    {
        public long Proof { get; set; }
        public long? LastProof { get; set; }
    }
}