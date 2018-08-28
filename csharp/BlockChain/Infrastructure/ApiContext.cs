using Microsoft.EntityFrameworkCore;
using BlockChain.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace BlockChain.Infrastructure
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
           : base(options)
        {
        }

        public DbSet<Blockchain> Blockchains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blockchain>().HasData(new Blockchain
            {
                Id = Guid.Parse("a6407387-7a9c-4343-99f1-98be977252ce"),
                Chain = new List<Block>(),
                CurrentTransactions = new List<Transaction>()
            });
        }
    }
}