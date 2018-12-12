namespace LiveStethoV2
{
    partial class PopupListView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SoundDataList = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DateUploaded = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Bytes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EnableAnalyze = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SoundDataList
            // 
            this.SoundDataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Name,
            this.DateUploaded,
            this.Bytes});
            this.SoundDataList.Location = new System.Drawing.Point(12, 12);
            this.SoundDataList.Name = "SoundDataList";
            this.SoundDataList.Size = new System.Drawing.Size(374, 213);
            this.SoundDataList.TabIndex = 0;
            this.SoundDataList.UseCompatibleStateImageBehavior = false;
            this.SoundDataList.DoubleClick += new System.EventHandler(this.SoundDataList_DoubleClick);
            this.SoundDataList.Resize += new System.EventHandler(this.SoundDataList_Resize);
            // 
            // Id
            // 
            this.Id.Text = "Test No.";
            // 
            // Name
            // 
            this.Name.Text = "Name";
            this.Name.Width = 80;
            // 
            // DateUploaded
            // 
            this.DateUploaded.Text = "Date Uploaded";
            this.DateUploaded.Width = 150;
            // 
            // Bytes
            // 
            this.Bytes.Text = "Size (Bytes)";
            // 
            // EnableAnalyze
            // 
            this.EnableAnalyze.Location = new System.Drawing.Point(12, 231);
            this.EnableAnalyze.Name = "EnableAnalyze";
            this.EnableAnalyze.Size = new System.Drawing.Size(104, 24);
            this.EnableAnalyze.TabIndex = 1;
            // 
            // PopupListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 292);
            this.Controls.Add(this.SoundDataList);
            this.Controls.Add(this.EnableAnalyze);
            this.Text = "Recorded Sound Files";
            this.Load += new System.EventHandler(this.PopupListViewLoad);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView SoundDataList;
        private System.Windows.Forms.CheckBox EnableAnalyze;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Bytes;
        private System.Windows.Forms.ColumnHeader DateUploaded;
        private System.Windows.Forms.ColumnHeader Id;
    }
}