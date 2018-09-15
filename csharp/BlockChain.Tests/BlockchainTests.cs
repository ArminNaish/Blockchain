using System;
using System.Linq;
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
        public void Should_Create_Genesis_Block()
        {
            // arrange
            var expected = Block.Genesis();

            // act
            var blockchain = MakeBlockchain();
            var actual = blockchain.Blocks.First();

            // assert
            blockchain.Blocks.Should().HaveCount(1);
            actual.Should().NotBeNull();
            actual.Index.Should().Be(1);
            actual.Proof.Should().Be(new ProofOfWork(1));
            actual.PreviousHash.Should().Be(Sha256Hash.Of("Genesis"));
            actual.Transactions.Should().BeEmpty();
        }

        [Fact]
        public void Should_Create_New_Block()
        {
            // arrange
            var blockchain = MakeBlockchain();

            // act
            var actual = blockchain.NewBlock(new ProofOfWork(123));

            // assert
            blockchain.Blocks.Should().HaveCount(2);
            actual.Should().NotBeNull();
            actual.Index.Should().Be(2);
            actual.Proof.Should().Be(new ProofOfWork(123));
            actual.PreviousHash.Should().Be(Block.Genesis().Hash());
            actual.Transactions.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_Last_Block()
        {
            // arrange
            var blockchain = MakeBlockchain();
            blockchain.NewBlock(new ProofOfWork(123));

            // act
            var actual = blockchain.LastBlock;

            // assert
            blockchain.Blocks.Should().HaveCount(2);
            actual.Should().NotBeNull();
            actual.Index.Should().Be(2);
            actual.Proof.Should().Be(new ProofOfWork(123));
            actual.PreviousHash.Should().Be(Block.Genesis().Hash());
            actual.Transactions.Should().BeEmpty();
        }

        [Fact]
        public void Should_Throw_Exception_When_Proof_Is_Null()
        {
            var blockchain = MakeBlockchain();
            blockchain
                .Invoking(b => b.NewBlock(null))
                .Should()
                .Throw<ArgumentNullException>();
        }
        #endregion

        #region TestTransactionCreation
        [Fact]
        public void Should_Create_New_Transaction()
        {
            // arrange
            var blockchain = MakeBlockchain();
            var sender = Node.Default();
            var recipient = Node.Default();

            // act
            blockchain.NewTransaction(sender, recipient, 1);
            var actual = blockchain.CurrentTransactions.First();

            // assert
            blockchain.CurrentTransactions.Should().HaveCount(1);
            actual.Should().NotBeNull();
            actual.Sender.Should().Be(sender);
            actual.Recepient.Should().Be(recipient);
            actual.Amount.Should().Be(1);
        }

        [Fact]
        public void Should_Return_Index_Of_Next_Block()
        {
            // arrange
            var blockchain = MakeBlockchain();
            var sender = Node.Default();
            var recipient = Node.Default();

            // act
            var actual = blockchain.NewTransaction(sender, recipient, 1);

            // assert
            actual.Should().Be(2);
        }

        [Fact]
        public void Should_Throw_Exception_When_Recipient_Is_Null()
        {
            var blockchain = MakeBlockchain();
            blockchain
                .Invoking(b => b.NewTransaction(Node.Default(), null, 1))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Throw_Exception_When_Amount_Is_Negative()
        {
            var blockchain = MakeBlockchain();
            blockchain
                .Invoking(b => b.NewTransaction(Node.Default(), Node.Default(), -1))
                .Should()
                .Throw<ArgumentException>();
        }
        #endregion

        #region TestMining
        [Fact]
        public void Should_Calculate_Proof_Of_Work()
        {
            // arrange
            var blockchain = MakeBlockchain();
            var genesis = blockchain.LastBlock;
            var expected = new ProofOfWork(72608);

            // act
            var actual = blockchain.Mine();

            // assert
            actual.Proof.Should().Be(expected);
        }

        [Fact]
        public void Should_Reward_Miner_With_Transaction()
        {
            // arrange 
            var blockchain = MakeBlockchain();

            // act
            var block = blockchain.Mine();
            var actual = block.Transactions.Last();

            // assert
            actual.Should().NotBeNull();
            actual.Sender.Should().BeNull();
            actual.Recepient.Should().Be(blockchain.Self);
            actual.Amount.Should().Be(1);
        }

        [Fact]
        public void Should_Forge_New_Block()
        {
            // arrange 
            var blockchain = MakeBlockchain();

            // act
            var actual = blockchain.Mine();

            // assert
            actual.Should().NotBeNull();
            actual.Index.Should().Be(2);
            actual.Proof.Should().Be(new ProofOfWork(123));
            actual.PreviousHash.Should().Be(Block.Genesis().Hash());
            actual.Transactions.Should().HaveCount(1);
        }

        [Fact]
        public void Should_Reset_Transactions()
        {
            // arrange
            var blockchain = MakeBlockchain();

            // act
            blockchain.Mine();

            // assert
            blockchain.CurrentTransactions.Should().BeEmpty();
        }

        [Fact]
        public void Should_Add_Transactions_To_New_Block()
        {
            // arrange
            var blockchain = MakeBlockchain();
            var sender = Node.Default();
            var recipient = Node.Default();

            // act
            blockchain.NewTransaction(sender, recipient, 1);
            blockchain.NewTransaction(sender, recipient, 2);
            var actual = blockchain.Mine();

            // assert
            actual.Transactions.Should().HaveCount(3);
        }
        #endregion

        #region TestRegistration
        #endregion

        #region TestBlockchainValidation
        #endregion

        #region TestConsensusAlgorithm
        #endregion

        private static Blockchain MakeBlockchain()
        {
            var self = Node.Default();
            var blockchain = new Blockchain(self);
            return blockchain;
        }
    }
}
