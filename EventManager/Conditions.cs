using System;

namespace Conditions
{
    public static class Conditional
    {
        private static bool example1 = true;
        private static bool example2 = true;
        public static Func<bool>[] ArrayAll = 
        {
            () => example1 == example2,
            () => example2 == example1
        };
    }
    
    // Conditions can be added in the array above if they can be defined statically. A function is provided to add them dynamically.
}