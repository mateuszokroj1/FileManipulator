using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManipulator.UI
   
{
    public class MainViewModel : ModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            
        }

        #endregion

        #region Fields


        #endregion

        #region Properties



        #endregion

        #region Methods

        public void Close(Func<bool> canClose, Action action)
        {
            // TODO: W tym miejscu powinno się sprawdzić czy jakiś Task (z kolekcji Tasków, która powinna tu być) jest w trybie działającym, jeśli nie to od razu wykonać 'action'

            if (canClose())
                action();
        }

        #endregion

    }
}
