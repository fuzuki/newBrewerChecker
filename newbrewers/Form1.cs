using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newbrewers
{
    public partial class Form1 : Form
    {
        private bool checking;
        List<Brewer> brewelist;

        public Form1()
        {
            InitializeComponent();

            checking = false;
            brewelist = new List<Brewer>();
        }

        private void check_Click(object sender, EventArgs e)
        {
            var l = BrewerUtil.getBrewers();
            var db = new BrewerDB();
            db.open();
            foreach(var b in l)
            {
//                Console.WriteLine(b);
                db.regist(b);
            }
            db.close();

        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchbutton_Click(object sender, EventArgs e)
        {
            var db = new BrewerDB();
            db.open();
            brewelist = db.searchBrewer(searchTextBox.Text.Trim());
            db.close();

            brewerListView.Items.Clear();

            foreach (var b in brewelist)
            {
                brewerListView.Items.Add(new ListViewItem(b.brewer));
            }
        }

        private void csvExport_Click(object sender, EventArgs e)
        {
            if(brewelist.Count > 0 && saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                BrewerUtil.writeList(saveFileDialog1.FileName, brewelist);
            }
        }
    }
}
