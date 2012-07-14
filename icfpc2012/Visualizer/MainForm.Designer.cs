namespace Visualizer
{
	partial class MainForm
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
			this.infoPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.waterproofLabel = new System.Windows.Forms.Label();
			this.scoreLabel = new System.Windows.Forms.Label();
			this.zoomBar = new System.Windows.Forms.TrackBar();
			this.picture = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.movesBox = new System.Windows.Forms.TextBox();
			this.robotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label3 = new System.Windows.Forms.Label();
			this.infoPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// infoPanel
			// 
			this.infoPanel.BackColor = System.Drawing.SystemColors.Info;
			this.infoPanel.Controls.Add(this.tableLayoutPanel1);
			this.infoPanel.Controls.Add(this.zoomBar);
			this.infoPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.infoPanel.Location = new System.Drawing.Point(0, 24);
			this.infoPanel.Name = "infoPanel";
			this.infoPanel.Size = new System.Drawing.Size(289, 488);
			this.infoPanel.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.waterproofLabel, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.scoreLabel, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 7);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
			this.tableLayoutPanel1.RowCount = 8;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 61F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(289, 443);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "Score";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(8, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Waterproof";
			// 
			// waterproofLabel
			// 
			this.waterproofLabel.AutoSize = true;
			this.waterproofLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.waterproofLabel.Location = new System.Drawing.Point(147, 5);
			this.waterproofLabel.Name = "waterproofLabel";
			this.waterproofLabel.Size = new System.Drawing.Size(41, 29);
			this.waterproofLabel.TabIndex = 7;
			this.waterproofLabel.Text = "42";
			// 
			// scoreLabel
			// 
			this.scoreLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.scoreLabel.Location = new System.Drawing.Point(147, 48);
			this.scoreLabel.Name = "scoreLabel";
			this.scoreLabel.Size = new System.Drawing.Size(134, 43);
			this.scoreLabel.TabIndex = 8;
			this.scoreLabel.Text = "42";
			// 
			// zoomBar
			// 
			this.zoomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.zoomBar.LargeChange = 8;
			this.zoomBar.Location = new System.Drawing.Point(0, 443);
			this.zoomBar.Maximum = 76;
			this.zoomBar.Minimum = 4;
			this.zoomBar.Name = "zoomBar";
			this.zoomBar.Size = new System.Drawing.Size(289, 45);
			this.zoomBar.TabIndex = 4;
			this.zoomBar.TabStop = false;
			this.zoomBar.TickFrequency = 4;
			this.zoomBar.Value = 24;
			this.zoomBar.ValueChanged += new System.EventHandler(this.zoomBar_ValueChanged);
			// 
			// picture
			// 
			this.picture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picture.Location = new System.Drawing.Point(289, 24);
			this.picture.Name = "picture";
			this.picture.Size = new System.Drawing.Size(507, 488);
			this.picture.TabIndex = 2;
			this.picture.TabStop = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mapToolStripMenuItem,
            this.robotToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(796, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openFileToolStripMenuItem
			// 
			this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
			this.openFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openFileToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.openFileToolStripMenuItem.Text = "OpenFile";
			this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
			// 
			// mapToolStripMenuItem
			// 
			this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem});
			this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
			this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.mapToolStripMenuItem.Text = "Map";
			// 
			// reloadToolStripMenuItem
			// 
			this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
			this.reloadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.reloadToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.reloadToolStripMenuItem.Text = "Reload";
			this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
			// 
			// movesBox
			// 
			this.movesBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.movesBox.Location = new System.Drawing.Point(289, 492);
			this.movesBox.Name = "movesBox";
			this.movesBox.ReadOnly = true;
			this.movesBox.Size = new System.Drawing.Size(507, 20);
			this.movesBox.TabIndex = 4;
			// 
			// robotToolStripMenuItem
			// 
			this.robotToolStripMenuItem.Name = "robotToolStripMenuItem";
			this.robotToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
			this.robotToolStripMenuItem.Text = "Run robot";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(8, 375);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(235, 34);
			this.label3.TabIndex = 10;
			this.label3.Text = "SPACE — wait\r\nENTER — perform one robotAI move";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(796, 512);
			this.Controls.Add(this.movesBox);
			this.Controls.Add(this.picture);
			this.Controls.Add(this.infoPanel);
			this.Controls.Add(this.menuStrip1);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.infoPanel.ResumeLayout(false);
			this.infoPanel.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel infoPanel;
		private System.Windows.Forms.PictureBox picture;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
		private System.Windows.Forms.TrackBar zoomBar;
		private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
		private System.Windows.Forms.TextBox movesBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label waterproofLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label scoreLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolStripMenuItem robotToolStripMenuItem;
		private System.Windows.Forms.Label label3;
	}
}

