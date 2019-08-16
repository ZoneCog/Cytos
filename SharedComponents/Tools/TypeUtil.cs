using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Provides a centralized place for doing casting operations.
    /// </summary>
    public static class TypeUtil
    {
        #region Public methods

        /// <summary>
        /// Mimics the C# cast operation; i.e.
        /// <code>TypeUtil.Cast&lt;T&gt;(o)</code>
        /// is equivalent to
        /// <code>(T)o</code>.
        /// </summary>
        /// <typeparam name="T">Specifies the target type for the cast.</typeparam>
        /// <param name="o">Specifies the object to be cast to T.</param>
        /// <returns>o cast to T</returns>
        /// <exception cref="InvalidOperationException">
        /// If o cannot be cast to T.
        /// </exception>
        public static T Cast<T>(object o)
        {
            try
            {
                return (T)o;
            }
            catch (InvalidCastException couldNotCast)
            {
                throw new InvalidOperationException(couldNotCast.Message);
            }
        }

        /// <summary>
        /// Mimics the C# is operator; i.e.
        /// <code>TypeUtil.IsA&lt;T&gt;(o)</code>
        /// is equivalent to
        /// <code>o is T</code>.
        /// </summary>
        /// <typeparam name="T">Specifies the target type for the "is a" check.</typeparam>
        /// <param name="o">Specifies the object to be checked whether it can be cast to T.</param>
        /// <returns>True if o can be cast to T, otherwise false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "The intent of this method is to determine at runtime whether the given object is of type T.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsA<T>(object o)
        {
            return o is T;
        }

        /// <summary>
        /// Mimics the C# as operator; i.e.
        /// <code>TypeUtil.TryCast&lt;T&gt;(o)</code>
        /// is equivalent to
        /// <code>o as T</code>.
        /// </summary>
        /// <typeparam name="T">Specifies the target reference type for the attempted cast.</typeparam>
        /// <param name="o">Specifies the object reference to be cast to T.</param>
        /// <returns>o as a T if possible, otherwise null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryCast<T>(object o) where T : class
        {
            return o as T;
        }

        #endregion
    }
}
