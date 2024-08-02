namespace Xorrupt.Windows.Forms
{
    partial class OpenProcessDialog
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
            components = new System.ComponentModel.Container();
            button_Open = new Button();
            button_Cancel = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            createProcessToolStripMenuItem = new ToolStripMenuItem();
            cancelToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            pIDHEXToolStripMenuItem = new ToolStripMenuItem();
            openFileDialog_Exe = new OpenFileDialog();
            button_Refresh = new Button();
            textBox_Filter = new TextBox();
            label_Filter = new Label();
            imageList_Icons = new ImageList(components);
            listView_Applications = new ListView();
            columnHeader_ID_Applications = new ColumnHeader();
            columnHeader_Name_Applications = new ColumnHeader();
            tabControl_Processes = new TabControl();
            tabPage_Applications = new TabPage();
            tabPage_Windows = new TabPage();
            listView_Windows = new ListView();
            columnHeader_ID_Windows = new ColumnHeader();
            columnHeader_Name_Windows = new ColumnHeader();
            tabPage_Processes = new TabPage();
            listView_Processes = new ListView();
            columnHeader_ID_Processes = new ColumnHeader();
            columnHeader_Name_Processes = new ColumnHeader();
            menuStrip1.SuspendLayout();
            tabControl_Processes.SuspendLayout();
            tabPage_Applications.SuspendLayout();
            tabPage_Windows.SuspendLayout();
            tabPage_Processes.SuspendLayout();
            SuspendLayout();
            // 
            // button_Open
            // 
            button_Open.Anchor = AnchorStyles.Bottom;
            button_Open.Location = new Point(33, 383);
            button_Open.Name = "button_Open";
            button_Open.Size = new Size(75, 23);
            button_Open.TabIndex = 1;
            button_Open.Text = "Open";
            button_Open.UseVisualStyleBackColor = true;
            button_Open.Click += button_Open_Click;
            // 
            // button_Cancel
            // 
            button_Cancel.Anchor = AnchorStyles.Bottom;
            button_Cancel.Location = new Point(195, 383);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new Size(75, 23);
            button_Cancel.TabIndex = 2;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(314, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createProcessToolStripMenuItem, cancelToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // createProcessToolStripMenuItem
            // 
            createProcessToolStripMenuItem.Name = "createProcessToolStripMenuItem";
            createProcessToolStripMenuItem.Size = new Size(151, 22);
            createProcessToolStripMenuItem.Text = "Create Process";
            createProcessToolStripMenuItem.Click += createProcessToolStripMenuItem_Click;
            // 
            // cancelToolStripMenuItem
            // 
            cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            cancelToolStripMenuItem.Size = new Size(151, 22);
            cancelToolStripMenuItem.Text = "Cancel";
            cancelToolStripMenuItem.Click += cancelToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { pIDHEXToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // pIDHEXToolStripMenuItem
            // 
            pIDHEXToolStripMenuItem.CheckOnClick = true;
            pIDHEXToolStripMenuItem.Name = "pIDHEXToolStripMenuItem";
            pIDHEXToolStripMenuItem.Size = new Size(180, 22);
            pIDHEXToolStripMenuItem.Text = "PID HEX";
            pIDHEXToolStripMenuItem.CheckedChanged += pIDHEXToolStripMenuItem_CheckedChanged;
            // 
            // openFileDialog_Exe
            // 
            openFileDialog_Exe.DefaultExt = "exe";
            openFileDialog_Exe.FileName = "openFileDialog1";
            openFileDialog_Exe.Filter = "exe files (*.exe)|*.exe";
            // 
            // button_Refresh
            // 
            button_Refresh.Anchor = AnchorStyles.Bottom;
            button_Refresh.Location = new Point(114, 383);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(75, 23);
            button_Refresh.TabIndex = 4;
            button_Refresh.Text = "Refresh";
            button_Refresh.UseVisualStyleBackColor = true;
            button_Refresh.Click += button_Refresh_Click;
            // 
            // textBox_Filter
            // 
            textBox_Filter.Anchor = AnchorStyles.Bottom;
            textBox_Filter.Location = new Point(12, 354);
            textBox_Filter.Name = "textBox_Filter";
            textBox_Filter.Size = new Size(290, 23);
            textBox_Filter.TabIndex = 5;
            textBox_Filter.TextChanged += textBox_Filter_TextChanged;
            // 
            // label_Filter
            // 
            label_Filter.Anchor = AnchorStyles.Bottom;
            label_Filter.AutoSize = true;
            label_Filter.Location = new Point(12, 336);
            label_Filter.Name = "label_Filter";
            label_Filter.Size = new Size(33, 15);
            label_Filter.TabIndex = 6;
            label_Filter.Text = "Filter";
            // 
            // imageList_Icons
            // 
            imageList_Icons.ColorDepth = ColorDepth.Depth8Bit;
            imageList_Icons.ImageSize = new Size(16, 16);
            imageList_Icons.TransparentColor = Color.Transparent;
            // 
            // listView_Applications
            // 
            listView_Applications.Columns.AddRange(new ColumnHeader[] { columnHeader_ID_Applications, columnHeader_Name_Applications });
            listView_Applications.Dock = DockStyle.Fill;
            listView_Applications.LargeImageList = imageList_Icons;
            listView_Applications.Location = new Point(3, 3);
            listView_Applications.MultiSelect = false;
            listView_Applications.Name = "listView_Applications";
            listView_Applications.Size = new Size(300, 276);
            listView_Applications.SmallImageList = imageList_Icons;
            listView_Applications.StateImageList = imageList_Icons;
            listView_Applications.TabIndex = 9;
            listView_Applications.UseCompatibleStateImageBehavior = false;
            listView_Applications.View = View.Details;
            listView_Applications.ColumnClick += listView_Processes_ColumnClick;
            // 
            // columnHeader_ID_Applications
            // 
            columnHeader_ID_Applications.Text = "ID";
            columnHeader_ID_Applications.Width = 155;
            // 
            // columnHeader_Name_Applications
            // 
            columnHeader_Name_Applications.Text = "Name";
            columnHeader_Name_Applications.Width = 155;
            // 
            // tabControl_Processes
            // 
            tabControl_Processes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl_Processes.Controls.Add(tabPage_Applications);
            tabControl_Processes.Controls.Add(tabPage_Windows);
            tabControl_Processes.Controls.Add(tabPage_Processes);
            tabControl_Processes.Location = new Point(0, 24);
            tabControl_Processes.Name = "tabControl_Processes";
            tabControl_Processes.SelectedIndex = 0;
            tabControl_Processes.Size = new Size(314, 310);
            tabControl_Processes.TabIndex = 10;
            // 
            // tabPage_Applications
            // 
            tabPage_Applications.Controls.Add(listView_Applications);
            tabPage_Applications.Location = new Point(4, 24);
            tabPage_Applications.Name = "tabPage_Applications";
            tabPage_Applications.Padding = new Padding(3);
            tabPage_Applications.Size = new Size(306, 282);
            tabPage_Applications.TabIndex = 0;
            tabPage_Applications.Text = "Applications";
            tabPage_Applications.UseVisualStyleBackColor = true;
            // 
            // tabPage_Windows
            // 
            tabPage_Windows.Controls.Add(listView_Windows);
            tabPage_Windows.Location = new Point(4, 24);
            tabPage_Windows.Name = "tabPage_Windows";
            tabPage_Windows.Padding = new Padding(3);
            tabPage_Windows.Size = new Size(306, 282);
            tabPage_Windows.TabIndex = 1;
            tabPage_Windows.Text = "Windows";
            tabPage_Windows.UseVisualStyleBackColor = true;
            // 
            // listView_Windows
            // 
            listView_Windows.Columns.AddRange(new ColumnHeader[] { columnHeader_ID_Windows, columnHeader_Name_Windows });
            listView_Windows.Dock = DockStyle.Fill;
            listView_Windows.LargeImageList = imageList_Icons;
            listView_Windows.Location = new Point(3, 3);
            listView_Windows.MultiSelect = false;
            listView_Windows.Name = "listView_Windows";
            listView_Windows.Size = new Size(300, 276);
            listView_Windows.SmallImageList = imageList_Icons;
            listView_Windows.StateImageList = imageList_Icons;
            listView_Windows.TabIndex = 10;
            listView_Windows.UseCompatibleStateImageBehavior = false;
            listView_Windows.View = View.Details;
            listView_Windows.ColumnClick += listView_Processes_ColumnClick;
            // 
            // columnHeader_ID_Windows
            // 
            columnHeader_ID_Windows.Text = "ID";
            columnHeader_ID_Windows.Width = 155;
            // 
            // columnHeader_Name_Windows
            // 
            columnHeader_Name_Windows.Text = "Name";
            columnHeader_Name_Windows.Width = 155;
            // 
            // tabPage_Processes
            // 
            tabPage_Processes.Controls.Add(listView_Processes);
            tabPage_Processes.Location = new Point(4, 24);
            tabPage_Processes.Name = "tabPage_Processes";
            tabPage_Processes.Padding = new Padding(3);
            tabPage_Processes.Size = new Size(306, 282);
            tabPage_Processes.TabIndex = 2;
            tabPage_Processes.Text = "Processes";
            tabPage_Processes.UseVisualStyleBackColor = true;
            // 
            // listView_Processes
            // 
            listView_Processes.Columns.AddRange(new ColumnHeader[] { columnHeader_ID_Processes, columnHeader_Name_Processes });
            listView_Processes.Dock = DockStyle.Fill;
            listView_Processes.LargeImageList = imageList_Icons;
            listView_Processes.Location = new Point(3, 3);
            listView_Processes.MultiSelect = false;
            listView_Processes.Name = "listView_Processes";
            listView_Processes.Size = new Size(300, 276);
            listView_Processes.SmallImageList = imageList_Icons;
            listView_Processes.StateImageList = imageList_Icons;
            listView_Processes.TabIndex = 10;
            listView_Processes.UseCompatibleStateImageBehavior = false;
            listView_Processes.View = View.Details;
            listView_Processes.ColumnClick += listView_Processes_ColumnClick;
            // 
            // columnHeader_ID_Processes
            // 
            columnHeader_ID_Processes.Text = "ID";
            columnHeader_ID_Processes.Width = 155;
            // 
            // columnHeader_Name_Processes
            // 
            columnHeader_Name_Processes.Text = "Name";
            columnHeader_Name_Processes.Width = 155;
            // 
            // OpenProcessDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(314, 411);
            Controls.Add(tabControl_Processes);
            Controls.Add(label_Filter);
            Controls.Add(textBox_Filter);
            Controls.Add(button_Refresh);
            Controls.Add(button_Cancel);
            Controls.Add(button_Open);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(330, 450);
            Name = "OpenProcessDialog";
            Text = "Process List";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl_Processes.ResumeLayout(false);
            tabPage_Applications.ResumeLayout(false);
            tabPage_Windows.ResumeLayout(false);
            tabPage_Processes.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button_Open;
        private Button button_Cancel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem createProcessToolStripMenuItem;
        private ToolStripMenuItem cancelToolStripMenuItem;
        private OpenFileDialog openFileDialog_Exe;
        private Button button_Refresh;
        private TextBox textBox_Filter;
        private Label label_Filter;
        private ImageList imageList_Icons;
        private ListView listView_Applications;
        private ColumnHeader columnHeader_Name_Applications;
        private ColumnHeader columnHeader_ID_Applications;
        private TabControl tabControl_Processes;
        private TabPage tabPage_Applications;
        private TabPage tabPage_Windows;
        private TabPage tabPage_Processes;
        private ListView listView_Windows;
        private ColumnHeader columnHeader_Name_Windows;
        private ColumnHeader columnHeader_ID_Windows;
        private ListView listView_Processes;
        private ColumnHeader columnHeader_Name_Processes;
        private ColumnHeader columnHeader_ID_Processes;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem pIDHEXToolStripMenuItem;
    }
}