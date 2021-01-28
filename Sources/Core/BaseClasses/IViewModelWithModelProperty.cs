namespace FileManipulator
{
    public interface IViewModelWithModelProperty<TModel>
    {
        TModel Model { get; }
    }
}