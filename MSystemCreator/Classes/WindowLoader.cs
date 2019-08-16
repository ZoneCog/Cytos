using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSystemCreator.Classes
{
    public class WindowLoader
    {
        public void OpenMainWindow()
        {
            MSystemCreatorForm mainForm = new MSystemCreatorForm();
            mainForm.Show();
        }
    }
}
