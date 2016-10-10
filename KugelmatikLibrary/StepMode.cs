namespace KugelmatikLibrary
{
    /// <summary>
    /// Gibt den Schrittmodus an. 
    /// </summary>
    public enum StepMode : byte
    { 
        /// <summary>
        /// Immer einen halben Schritt machen.
        /// </summary>
        Half = 1,

        /// <summary>
        /// Immer einen vollen Schritt machen.
        /// </summary>
        Full = 2,

        /// <summary>
        /// Je nach Situation einen halben oder einen vollen Schritt machen.
        /// </summary>
        Both = 3
    }
}
