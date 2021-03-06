﻿using System.Threading.Tasks;
using DddCore.Contracts.BLL.Domain.Models;

namespace DddCore.Contracts.DAL.DomainStack
{
    public interface IDomainRepository<T, in TKey, TState> where T : class, IAggregateRoot<TKey, TState> where TState : IAggregateRootState<TKey>
    {
        /// <summary>
        /// Read aggregate root with all related entities but w/o other aggregate roots.
        /// </summary>
        /// <param name="aggregateRootKey"></param>
        /// <returns>Aggregate root graph</returns>
        Task<T> GetByIdAsync(TKey aggregateRootKey);

        /// <summary>
        /// Read aggregate root with all related entities but w/o other aggregate roots.
        /// </summary>
        /// <param name="aggregateRootKey"></param>
        /// <returns>Aggregate root graph</returns>
        T GetById(TKey aggregateRootKey);
    }
}