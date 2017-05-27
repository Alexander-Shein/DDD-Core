﻿using System;

namespace DddCore.Crosscutting
{
    internal static class Guard
    {
        public static void ThrowIfNull(object instance, string message)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}