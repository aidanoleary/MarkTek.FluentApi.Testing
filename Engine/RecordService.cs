﻿using Marktek.Fluent.Testing.Engine;
using Marktek.Fluent.Testing.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarkTek.Fluent.Testing.RecordGeneration
{

/// <summary>
/// Fluent Record Service to track all changes during a testing session
/// </summary>
/// <typeparam name="TID"></typeparam>
    public class RecordService<TID> : IRecordService<TID>
    {
        /// <summary>
        /// Get the list of Id's created during the recordservice lifetime.
        /// </summary>
      //  private List<TID> CreatedIds { get; set; }

        public Dictionary<TID, object> CreatedRecords { get; private set; }


      /// <summary>
      /// The primary entity id, Used for cleanup and assertion classes
      /// </summary>
        public TID AggregateId { get; private set; }

        /// <summary>
        /// Every Graph must have a hierarchy
        /// </summary>
        /// <param name="aggregateId"></param>
        public RecordService(TID aggregateId)
        {
            CreatedRecords = new Dictionary<TID, object>();
            this.AggregateId = aggregateId;
        }

       /// <summary>
       /// Create a prarent record as hold the Id so it can be passed to the next command
       /// </summary>
       /// <typeparam name="TEntity"></typeparam>
       /// <param name="app"></param>
       /// <returns></returns>
        public virtual IRecordService<TID> CreateRecord<TEntity>(IRecordCreator<TEntity, TID> app)
        {
            var res = app.CreateRecord();
            this.CreatedRecords.Add(res.Id, res.Row);
            return this;
        }

        /// <summary>
        /// Create a record based on the record that was previously created, passes in the ID of the Previous record that was created
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public virtual IRecordService<TID> CreateRelatedRecord<TEntity>(IRelatedRecordCreator<TEntity, TID> app)
        {
            Guard.AgainstNull(this.CreatedRecords);
            var res = app.CreateRecord(CreatedRecords.Last().Key);
            this.CreatedRecords.Add(res.Id, res.Row);
            return this;
        }

        /// <summary>
        /// Creates a record based on the previous record created, passes in the entire object of the record that was created
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public virtual IRecordService<TID> CreatedRelatedRecord<TParent, TEntity>(IRelatedRecordCreator<TParent, TEntity, TID> app)
        {
            Guard.AgainstNull(this.CreatedRecords);
            var res = app.CreateRecord((TParent)CreatedRecords.Last().Value);
            this.CreatedRecords.Add(res.Id, res.Row);
            return this;
        }

        /// <summary>
        /// Assert against one or more assertions
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="spec"></param>
        /// <returns></returns>
        public IRecordService<TID> AssertAgainst<TType>(BaseValidator<TID, TType> spec) 
        {
            spec.Validate(AggregateId);
            return this;
        }
        
        /// <summary>
        /// Execute method based on Condition. Useful for Scenarios where you want to configure the behaviour
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IRecordService<TID> If(bool condition, Func<IRecordService<TID>, IRecordService<TID>> builder)
        {
            return condition ? builder(this) : this;
        }

        /// <summary>
        /// Cleanup Records
        /// </summary>
        /// <param name="Id"></param>
        public void Cleanup(IRecordCleanup<TID> Id)
        {
            Id.Cleanup(CreatedRecords, this.AggregateId);
        }

        /// <summary>
        /// Execute Action against the id of the last created record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public IRecordService<TID> ExecuteAction<T>(IExecutableAction<T, TID> implementation)
        {
            implementation.Execute(CreatedRecords.Last().Key);
            return this;
        }

        /// <summary>
        /// Execute Action against the aggregate record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public IRecordService<TID> ExecuteActionOnAggregate<T>(IExecutableAggregateAction<T, TID> implementation)
        {
            implementation.Execute(AggregateId);
            return this;
        }

      /// <summary>
      /// Delay execution. Time in ms (milliseconds)
      /// </summary>
      /// <param name="milliseconds"></param>
      /// <returns></returns>
        public IRecordService<TID>Delay(int milliseconds)
        {
            Thread.Sleep(milliseconds);
            return this;
        }

        /// <summary>
        /// Set the Aggregate id on the fly
        /// </summary>
        /// <returns></returns>
        public IRecordService<TID> AssignAggregateId()
        {
            this.AggregateId = this.CreatedRecords.Last().Key;
            return this;
        }

       
    }
}