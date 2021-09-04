namespace NetDbgUI
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.splMain = new System.Windows.Forms.SplitContainer();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtInterpreter = new System.Windows.Forms.TextBox();
			this.lblStatus = new System.Windows.Forms.Label();
			this.txtInterpreterCommand = new System.Windows.Forms.TextBox();
			this.splBottom = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtDebug = new System.Windows.Forms.TextBox();
			this.splCaptureControls = new System.Windows.Forms.SplitContainer();
			this.txtCommand = new System.Windows.Forms.TextBox();
			this.chkCapturePackets = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtError = new System.Windows.Forms.TextBox();
			this.timRefresh = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splMain)).BeginInit();
			this.splMain.Panel1.SuspendLayout();
			this.splMain.Panel2.SuspendLayout();
			this.splMain.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splBottom)).BeginInit();
			this.splBottom.Panel1.SuspendLayout();
			this.splBottom.Panel2.SuspendLayout();
			this.splBottom.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splCaptureControls)).BeginInit();
			this.splCaptureControls.Panel1.SuspendLayout();
			this.splCaptureControls.Panel2.SuspendLayout();
			this.splCaptureControls.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splMain
			// 
			this.splMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splMain.Location = new System.Drawing.Point(0, 0);
			this.splMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.splMain.Name = "splMain";
			this.splMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splMain.Panel1
			// 
			this.splMain.Panel1.Controls.Add(this.groupBox3);
			// 
			// splMain.Panel2
			// 
			this.splMain.Panel2.Controls.Add(this.splBottom);
			this.splMain.Size = new System.Drawing.Size(1407, 599);
			this.splMain.SplitterDistance = 167;
			this.splMain.SplitterWidth = 5;
			this.splMain.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtInterpreter);
			this.groupBox3.Controls.Add(this.lblStatus);
			this.groupBox3.Controls.Add(this.txtInterpreterCommand);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox3.Location = new System.Drawing.Point(0, 0);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox3.Size = new System.Drawing.Size(1407, 167);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Interpreter Console";
			// 
			// txtInterpreter
			// 
			this.txtInterpreter.BackColor = System.Drawing.Color.White;
			this.txtInterpreter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtInterpreter.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtInterpreter.Location = new System.Drawing.Point(4, 19);
			this.txtInterpreter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtInterpreter.Multiline = true;
			this.txtInterpreter.Name = "txtInterpreter";
			this.txtInterpreter.ReadOnly = true;
			this.txtInterpreter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtInterpreter.Size = new System.Drawing.Size(1399, 73);
			this.txtInterpreter.TabIndex = 0;
			this.txtInterpreter.WordWrap = false;
			// 
			// lblStatus
			// 
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.lblStatus.Location = new System.Drawing.Point(4, 92);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Padding = new System.Windows.Forms.Padding(4);
			this.lblStatus.Size = new System.Drawing.Size(1399, 52);
			this.lblStatus.TabIndex = 2;
			this.lblStatus.Visible = false;
			// 
			// txtInterpreterCommand
			// 
			this.txtInterpreterCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtInterpreterCommand.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtInterpreterCommand.Location = new System.Drawing.Point(4, 144);
			this.txtInterpreterCommand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtInterpreterCommand.Name = "txtInterpreterCommand";
			this.txtInterpreterCommand.Size = new System.Drawing.Size(1399, 20);
			this.txtInterpreterCommand.TabIndex = 1;
			this.txtInterpreterCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInterpreterCommand_KeyPress);
			// 
			// splBottom
			// 
			this.splBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splBottom.Location = new System.Drawing.Point(0, 0);
			this.splBottom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.splBottom.Name = "splBottom";
			// 
			// splBottom.Panel1
			// 
			this.splBottom.Panel1.Controls.Add(this.groupBox1);
			// 
			// splBottom.Panel2
			// 
			this.splBottom.Panel2.Controls.Add(this.groupBox2);
			this.splBottom.Size = new System.Drawing.Size(1407, 427);
			this.splBottom.SplitterDistance = 900;
			this.splBottom.SplitterWidth = 5;
			this.splBottom.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtDebug);
			this.groupBox1.Controls.Add(this.splCaptureControls);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Size = new System.Drawing.Size(900, 427);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Raw Output";
			// 
			// txtDebug
			// 
			this.txtDebug.BackColor = System.Drawing.Color.White;
			this.txtDebug.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDebug.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtDebug.Location = new System.Drawing.Point(4, 19);
			this.txtDebug.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtDebug.Multiline = true;
			this.txtDebug.Name = "txtDebug";
			this.txtDebug.ReadOnly = true;
			this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtDebug.Size = new System.Drawing.Size(892, 381);
			this.txtDebug.TabIndex = 0;
			this.txtDebug.WordWrap = false;
			// 
			// splCaptureControls
			// 
			this.splCaptureControls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splCaptureControls.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splCaptureControls.IsSplitterFixed = true;
			this.splCaptureControls.Location = new System.Drawing.Point(4, 400);
			this.splCaptureControls.Name = "splCaptureControls";
			// 
			// splCaptureControls.Panel1
			// 
			this.splCaptureControls.Panel1.Controls.Add(this.txtCommand);
			// 
			// splCaptureControls.Panel2
			// 
			this.splCaptureControls.Panel2.Controls.Add(this.chkCapturePackets);
			this.splCaptureControls.Size = new System.Drawing.Size(892, 24);
			this.splCaptureControls.SplitterDistance = 770;
			this.splCaptureControls.TabIndex = 3;
			this.splCaptureControls.Text = "splitContainer1";
			// 
			// txtCommand
			// 
			this.txtCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtCommand.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtCommand.Location = new System.Drawing.Point(0, 4);
			this.txtCommand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtCommand.Name = "txtCommand";
			this.txtCommand.Size = new System.Drawing.Size(770, 20);
			this.txtCommand.TabIndex = 1;
			this.txtCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCommand_KeyPress);
			// 
			// chkCapturePackets
			// 
			this.chkCapturePackets.AutoSize = true;
			this.chkCapturePackets.Location = new System.Drawing.Point(13, 5);
			this.chkCapturePackets.Name = "chkCapturePackets";
			this.chkCapturePackets.Size = new System.Drawing.Size(98, 19);
			this.chkCapturePackets.TabIndex = 2;
			this.chkCapturePackets.Text = "Show Packets";
			this.chkCapturePackets.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtError);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox2.Size = new System.Drawing.Size(502, 427);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Error Output";
			// 
			// txtError
			// 
			this.txtError.BackColor = System.Drawing.Color.White;
			this.txtError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtError.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtError.Location = new System.Drawing.Point(4, 19);
			this.txtError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtError.Multiline = true;
			this.txtError.Name = "txtError";
			this.txtError.ReadOnly = true;
			this.txtError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtError.Size = new System.Drawing.Size(494, 405);
			this.txtError.TabIndex = 1;
			this.txtError.WordWrap = false;
			// 
			// timRefresh
			// 
			this.timRefresh.Enabled = true;
			this.timRefresh.Interval = 50;
			this.timRefresh.Tick += new System.EventHandler(this.timRefresh_Tick);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1407, 599);
			this.Controls.Add(this.splMain);
			this.Location = new System.Drawing.Point(1400, 0);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "frmMain";
			this.Text = "ProxyDebug V1.0";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.splMain.Panel1.ResumeLayout(false);
			this.splMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splMain)).EndInit();
			this.splMain.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.splBottom.Panel1.ResumeLayout(false);
			this.splBottom.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splBottom)).EndInit();
			this.splBottom.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.splCaptureControls.Panel1.ResumeLayout(false);
			this.splCaptureControls.Panel1.PerformLayout();
			this.splCaptureControls.Panel2.ResumeLayout(false);
			this.splCaptureControls.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splCaptureControls)).EndInit();
			this.splCaptureControls.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splMain;
		private System.Windows.Forms.SplitContainer splBottom;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtDebug;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Timer timRefresh;
		private System.Windows.Forms.TextBox txtError;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtInterpreter;
		private System.Windows.Forms.TextBox txtInterpreterCommand;
		private System.Windows.Forms.SplitContainer splCaptureControls;
		private System.Windows.Forms.TextBox txtCommand;
		private System.Windows.Forms.CheckBox chkCapturePackets;
		private System.Windows.Forms.Label lblStatus;
	}
}

