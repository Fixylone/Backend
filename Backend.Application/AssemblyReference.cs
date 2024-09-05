using System.Reflection;

namespace Backend.Application
{
    /// <summary>
    /// Represents the class reponsible for assembly retrieval.
    /// </summary>
    internal static class AssemblyReference
    {
        /// <summary>
        /// Gets the assembly.
        /// </summary>
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
