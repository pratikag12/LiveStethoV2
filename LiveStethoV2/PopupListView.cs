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
        public PopupListView()
        {
            InitializeComponent();
        }

        private void PopupListViewLoad(object sender, EventArgs e)
        {
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



           //ListViewItem item1 = new ListViewItem();
           // Place a check mark next to the item.
           // item1.SubItems.Add("1");
           // item1.SubItems.Add("2");
           // ListViewItem item2 = new ListViewItem();
           // item2.SubItems.Add("4");
           // item2.SubItems.Add("5");
           // ListViewItem item3 = new ListViewItem();
           // Place a check mark next to the item.
           // item3.SubItems.Add("7");
           // item3.SubItems.Add("8");

           // Create columns for the items and subitems.

           // Width of - 2 indicates auto - size.
           //listView1.Columns.Add("Name");
           // listView1.Columns.Add("Size (Bytes)");
           // listView1.Columns.Add("Date");

           // Add the items to the ListView.
           // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
        }

        private void PopupListViewShown(object sender, EventArgs e)
        {
        }

        private void SoundDataList_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(SoundDataList.SelectedItems.ToString());
            MessageBox.Show(SoundDataList.SelectedItems[0].SubItems[0].Text);
        }
    }
}
