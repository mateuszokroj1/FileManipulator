namespace FileManipulator
{
    public interface IViewModelWithModelProperty { }

    public interface IViewModelWithModelProperty<TModel> : IViewModelWithModelProperty
    {
        TModel Model { get; }
    }
}