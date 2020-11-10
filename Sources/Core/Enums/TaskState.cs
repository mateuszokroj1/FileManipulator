namespace FileManipulator
{
    /// <summary>
    /// Representing actual task working state
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// On undefined state
        /// </summary>
        Undefined,

        /// <summary>
        /// When exception is handled
        /// </summary>
        Error,

        /// <summary>
        /// Before start
        /// </summary>
        Ready,

        /// <summary>
        /// When stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// During work
        /// </summary>
        Working,
        
        /// <summary>
        /// When task is paused
        /// </summary>
        Paused,

        /// <summary>
        /// On work completed
        /// </summary>
        Done
    }
}
