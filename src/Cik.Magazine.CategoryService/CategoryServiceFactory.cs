﻿using System;
using Akka.Actor;
using Akka.Routing;
using Cik.Magazine.CategoryService.Denomalizer.Projections;
using Cik.Magazine.CategoryService.Domain;
using Cik.Magazine.CategoryService.Query;
using Cik.Magazine.Shared;
using Cik.Magazine.Shared.Domain;

namespace Cik.Magazine.CategoryService
{
    public static class CategoryServiceFactory
    {
        public static IActorRef CategoryCommanderAggregate(this IActorRefFactory system, Guid id,
            int snapshotThreshold = 10)
        {
            var nameOfCommanderActor = SystemData.CategoryCommanderActor.Name;
            var nameofProjectionActor = SystemData.CategoryProjectionsActor.Name;
            // build up the category actor
            var projectionsProps = new ConsistentHashingPool(10).Props(Props.Create<ReadModelProjections>());
            var projections = system.ActorOf(projectionsProps, $"{nameofProjectionActor}-{nameOfCommanderActor}");
            var creationParams = new AggregateRootCreationParameters(id, projections, snapshotThreshold);
            return system.ActorOf(Props.Create<Category>(creationParams), nameOfCommanderActor);
        }

        public static IActorRef CategoryQueryAggregate(this IActorRefFactory system)
        {
            var nameOfQueryActor = SystemData.CategoryQueryActor.Name;
            var queryProps = new ConsistentHashingPool(10).Props(Props.Create<CategoryQuery>());
            return system.ActorOf(queryProps, $"{nameOfQueryActor}");
        }
    }
}