﻿using System;
using System.Collections.Generic;
using System.Linq;
using DddCore.Contracts.BLL.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DddCore.BLL.Domain.Events
{
    public class DomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        #region Private Members

        private readonly IServiceProvider _serviceProvider;

        #endregion

        public DomainEventHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Public Methods

        public IEnumerable<IDomainEventHandler<T>> GetHandlers<T>() where T : IDomainEvent
        {
            return _serviceProvider.GetService<IEnumerable<IDomainEventHandler<T>>>() ?? Enumerable.Empty<IDomainEventHandler<T>>();
        }

        #endregion
    }
}