using System;

namespace Retry.NET.Validation
{
    public static class Assert
    {
        public static void NotNull(object @object, string parameterName)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("Parameter " + parameterName + " cannot be null");
            }
        }

        public static void Positive(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException("Paramater " + parameterName + " must be positive");
            }
        }
    }
}