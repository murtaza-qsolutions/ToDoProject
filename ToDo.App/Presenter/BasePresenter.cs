using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.App.Presenter
{
    public abstract class BasePresenter<TView>
    {
        protected TView View { get; private set; }

        protected BasePresenter(TView view) => View = view;
        public virtual void Initialize() { }
    }
}
