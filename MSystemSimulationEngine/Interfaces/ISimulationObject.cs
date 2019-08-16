namespace MSystemSimulationEngine.Interfaces
{
    /// <summary>
    /// Intefrace which defines methods required for each simulation object (Fixed or Floating object, Protein).
    /// </summary>
    public interface ISimulationObject
    {
        /// <summary>
        /// Object name.
        /// </summary>
        string Name { get; }
}
}