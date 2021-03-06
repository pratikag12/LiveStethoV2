﻿using System;
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
        private List<SoundDataModel.SoundData> SoundList;
        public int SelectedRecord { get; set; }
        public bool Analyze { get; set; }

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
            listView1.Sorting = SortOrder.Descending;

           // Create three items and three sets of subitems for each item.
            foreach (SoundDataModel.SoundData elem in this.SoundList)
            {
                ListViewItem item = new ListViewItem(elem.Id.ToString());
                item.SubItems.Add(elem.Name);
                item.SubItems.Add(elem.Date.ToString());
                item.SubItems.Add(elem.Length.ToString());
                listView1.Items.Add(item);
            }
        }

        private void SoundDataList_DoubleClick(object sender, EventArgs e)
        {
            this.SelectedRecord = Convert.ToInt32(SoundDataList.SelectedItems[0].Text);
            this.Analyze = this.EnableAnalyze.Checked;
            this.Close();
        }

        //Colum Resizing function
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
