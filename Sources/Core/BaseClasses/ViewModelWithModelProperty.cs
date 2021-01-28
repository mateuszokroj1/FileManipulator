using System;

namespace FileManipulator
{
    public abstract class ViewModelWithModelProperty<TModel> : ModelBase, IViewModelWithModelProperty<TModel>
    {
        protected ViewModelWithModelProperty(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Model = model;
        }

        public TModel Model { get; protected set; }
    }
}
