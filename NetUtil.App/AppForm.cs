using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetUtil.App
{
    public partial class AppForm : Form
    {
        public AppForm()
        {
            InitializeComponent();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            //
            // TODO: stop any running threads & network listeners.
            //
            this.Close();
        }

        private void tsbStartStop_Click(object sender, EventArgs e)
        {
            if (tsbStartStop.Checked)
            {
                tsbStartStop.Text = "Stop";
            }
            else
            {
                tsbStartStop.Text = "Start";
            }
        }
    }
}
