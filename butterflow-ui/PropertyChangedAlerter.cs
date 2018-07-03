using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    public abstract class PropertyChangedAlerter : INotifyPropertyChanged
    {
        #region Members

        /// <summary> Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary> A list of properties to always call as updated. Generally used for composite properties. </summary>
        private List<string> alwaysCall = new List<string>();

        #endregion

        #region Properties

        //

        #endregion

        #region Methods

        /// <summary> Executes the property changed action. </summary>
        /// <param name="name"> The name. </param>
        protected virtual void OnPropertyChanged([CallerMemberName]string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                foreach (var updatedProperty in this.alwaysCall)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(updatedProperty));
                }
            }
        }

        /// <summary> Adds a property that will always be called when any property is updated.. </summary>
        /// <param name="name"> The name of the property. </param>
        public void AddConstantCallProperty(string name)
        {
            if (!this.alwaysCall.Any(c => c.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                this.alwaysCall.Add(name);
            }
        }

        #endregion
    }
}
