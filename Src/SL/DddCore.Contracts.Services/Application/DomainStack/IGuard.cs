﻿using System.Threading.Tasks;
using DddCore.Contracts.Domain.Entities;

namespace DddCore.Contracts.Services.Application.DomainStack
{
    public interface IGuard
    {
        void NotNull(object obj, string message = "");
        Task AggregateRootIsValidAsync<T, TKey>(T aggregateRoot) where T : IAggregateRootEntity<TKey>;
        void AggregateRootIsValid<T, TKey>(T aggregateRoot) where T : IAggregateRootEntity<TKey>;
    }
}