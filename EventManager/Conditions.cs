using System;

namespace Conditions
{
    public static class Conditional
    {
        private const bool Example1 = true;
        private const bool Example2 = true;

        public static readonly Func<bool>[] ArrayAll = 
        {
            () => Example1 == Example2,
            () => Example2 == Example1
        };
    }
    
    // Conditions can be added in the array above if they can be defined statically. A function is provided to add them dynamically.
}