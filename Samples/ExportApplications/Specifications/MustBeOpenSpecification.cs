﻿using Marktek.Fluent.Testing.Engine;
using MarkTek.Fluent.Testing.ExportApplications.Model;
using MarkTek.Fluent.Testing.RecordGeneration;
using System;
using System.Collections.Generic;

namespace MarkTek.Fluent.Testing.Sample.Specifications
{
    public class MustBe : BaseValidator<Guid, Order>
    {
        public override Order GetRecord(Guid id)
        {
            return new Order { };
        }

        public override List<ISpecificationValidator<Order>> GetValidators()
        {
            return new List<ISpecificationValidator<Order>>
           {
               new MustDoA(),
                new MustDoB()
           };
        }
    }   

}
