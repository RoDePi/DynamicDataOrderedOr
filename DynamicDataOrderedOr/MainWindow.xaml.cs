using System;
using System.Windows.Controls;

namespace DynamicDataOrderedOr
{
    public partial class MainWindow
    {
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            Closed += (_, _) => DisposeTabs();
        }
        #endregion

        #region Private members
        private void DisposeTabs()
        {
            foreach (TabItem tabControlItem in TabControl.Items)
            {
                if (tabControlItem.Content is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        #endregion
    }
}
