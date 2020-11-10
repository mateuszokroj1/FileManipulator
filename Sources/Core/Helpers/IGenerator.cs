namespace FileManipulator
{
    /// <summary>
    /// Using for generating values
    /// </summary>
    public interface IGenerator<T>
    {
        T Generate();
    }
}
