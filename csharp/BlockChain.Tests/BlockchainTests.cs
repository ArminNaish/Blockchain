using System;
using BlockChain.Domain;
using BlockChain.Domain.Model;
using FluentAssertions;
using Xunit;

namespace BlockChain.Tests
{
    public class BlockchainTests
    {
        #region TestRegisterNodes
        [Fact]
        public void Should_Register_Address_In_Blockchain()
        {
            // arrange
            var address = "http://127.0.0.1:9999/";
            var expected = new Node(address);
            var blockchain = MakeBlockchain();

            // act
            blockchain.Register(new Uri(address));

            // assert
            blockchain.Nodes.Should().HaveCount(1);
            blockchain.Nodes.Should().Contain(expected);
        }

        [Fact]
        public void Should_Register_Address_Idempotently()
        {
            // arrange
            var address = "http://127.0.0.1:9999/";
            var expected = new Node(address);
            var blockchain = MakeBlockchain();

            // act
            blockchain.Register(new Uri(address));
            blockchain.Register(new Uri(address));

            // assert
            blockchain.Nodes.Should().HaveCount(1);
            blockchain.Nodes.Should().Contain(expected);
        }

        [Fact]
        public void Should_Throw_Exception_When_Address_Is_Null()
        {
            var blockchain = MakeBlockchain();
            blockchain
                .Invoking(b => b.Register(null))
                .Should()
                .Throw<ArgumentNullException>();
        }
        #endregion

        #region TestBlockCreation
        [Fact]
        public void Should_Create_Genesis_At_Initialization()
        {
            // arrange
            var expected = Block.Genesis();
            var blockchain = MakeBlockchain(); 

            // act
            var actual = blockchain.LastBlock;

            // assert
            blockchain.Blocks.Should().HaveCount(1);
            actual.Index.Should().Be(1);
            actual.Proof.Should().Be(new ProofOfWork(1));
            actual.PreviousHash.Should().Be(  Sha256Hash.Of("Genesis"));
            actual.Transactions.Should().BeEmpty();
        }

        [Fact]
        public void Should_Create_New_Block()
        {
            // arrange
            var genesis = Block.Genesis();
            var blockchain = MakeBlockchain(); 

            // act
            blockchain.NewBlock(new ProofOfWork(123));
            var actual = blockchain.LastBlock;

            // assert
            blockchain.Blocks.Should().HaveCount(2);
            actual.Index.Should().Be(2);
            actual.Proof.Should().Be(new ProofOfWork(123));
            actual.PreviousHash.Should().Be(genesis.Hash());
            actual.Transactions.Should().BeEmpty();
        }

        // todo: test argumentnullex
        // todo: test clear transaction list
        // todo: test last block property
        #endregion

        private static Blockchain MakeBlockchain()
        {
            var self = Node.Localhost();
            var blockchain = new Blockchain(self);
            return blockchain;
        }
    }
}
