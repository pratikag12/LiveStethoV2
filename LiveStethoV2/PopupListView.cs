using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveStethoV2
{
    public partial class PopupListView : Form
    {
        List<SoundDataModel.SoundData> SoundList; 
        public PopupListView(List<SoundDataModel.SoundData> SoundList)
        {
            this.SoundList = SoundList;
            InitializeComponent();
        }

        private void PopupListViewLoad(object sender, EventArgs e)
        {
            SizeLastColumn(SoundDataList);
            this.CreateMyListView(SoundDataList);
        }

        private void CreateMyListView(ListView listView1)
        {
            // Set the view to show details.
            listView1.View = View.Details;
            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;
            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;
            // Display grid lines.
            listView1.GridLines = true;
            // Sort the items in the list in ascending order.
            listView1.Sorting = SortOrder.Ascending;

           // Create three items and three sets of subitems for each item.
            for (int i=0; i < 5; i++)
            {
                ListViewItem item = new ListViewItem(i.ToString());
                item.SubItems.Add("2");
                item.SubItems.Add("3");
                item.SubItems.Add("4");
                listView1.Items.Add(item);
            }
        }

        private void PopupListViewShown(object sender, EventArgs e)
        {
        }

        private void SoundDataList_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(SoundDataList.SelectedItems[0].SubItems[0].Text);
        }

        private void SoundDataList_Resize(object sender, EventArgs e)
        {
            SizeLastColumn((ListView)sender);
        }

        private void SizeLastColumn(ListView lv)
        {
            SoundDataList.Columns[SoundDataList.Columns.Count - 1].Width = -2;
        }
    }
}
