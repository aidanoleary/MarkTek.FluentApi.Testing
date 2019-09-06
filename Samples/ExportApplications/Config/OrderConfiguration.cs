﻿using MarkTek.Fluent.Testing.RecordGeneration;
using MarkTek.Fluent.Testing.ExportApplications.Model;
using System;
using System.IO;

namespace MarkTek.Fluent.Testing.Sample.Specifications.Config
{
    public class OrderConfiguration : IRecordCreator<Order, Guid>, IRelatedRecordCreator<Order, Guid>
    {
        public Record<Order, Guid> CreateRecord()
        {
            var c = new Order();
            Console.WriteLine($"Creating Order");

            File.AppendAllLines("C:\\Test\\test.txt",new[] { "Creating Order" });

            return new Record<Order, Guid>()
            {
                Row = c,
                Id = c.Id
            };
        }

        public Record<Order, Guid> CreateRecord(Guid id)
        {
            var c = new Order();
            Console.WriteLine($"Creating related Order with parent id {id}");


            File.AppendAllLines("C:\\Test\\test.txt", new[] { $"Creating related Order with parent id {id}" });

            return new Record<Order, Guid>()
            {
                Row = c,
                Id = c.Id
            };
        }

        
    }
}
