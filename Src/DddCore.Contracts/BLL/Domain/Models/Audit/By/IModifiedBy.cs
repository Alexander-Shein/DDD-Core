﻿namespace DddCore.Contracts.BLL.Domain.Models.Audit.By
{
    public interface IModifiedBy<TKey>
    {
        /// <summary>
        /// Use when you need to audit a person who modified the entity last time
        /// </summary>
        TKey ModifiedBy { get; set; }
    }
}