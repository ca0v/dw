namespace WinFormsApp1
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			buttonConnectToCamera = new Button();
			pictureBoxVideoCapture = new PictureBox();
			textBoxBarcode = new TextBox();
			comboBoxInputDevice = new ComboBox();
			tableLayoutPanel1 = new TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)pictureBoxVideoCapture).BeginInit();
			SuspendLayout();
			// 
			// buttonConnectToCamera
			// 
			buttonConnectToCamera.Location = new System.Drawing.Point(411, 18);
			buttonConnectToCamera.Name = "buttonConnectToCamera";
			buttonConnectToCamera.Size = new System.Drawing.Size(299, 55);
			buttonConnectToCamera.TabIndex = 0;
			buttonConnectToCamera.Text = "Connect To Camera";
			buttonConnectToCamera.UseVisualStyleBackColor = true;
			buttonConnectToCamera.Click += button1_Click;
			// 
			// pictureBoxVideoCapture
			// 
			pictureBoxVideoCapture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pictureBoxVideoCapture.Location = new System.Drawing.Point(34, 80);
			pictureBoxVideoCapture.Name = "pictureBoxVideoCapture";
			pictureBoxVideoCapture.Size = new System.Drawing.Size(834, 448);
			pictureBoxVideoCapture.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBoxVideoCapture.TabIndex = 1;
			pictureBoxVideoCapture.TabStop = false;
			// 
			// textBoxBarcode
			// 
			textBoxBarcode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			textBoxBarcode.Location = new System.Drawing.Point(34, 535);
			textBoxBarcode.Name = "textBoxBarcode";
			textBoxBarcode.Size = new System.Drawing.Size(833, 31);
			textBoxBarcode.TabIndex = 2;
			// 
			// comboBoxInputDevice
			// 
			comboBoxInputDevice.FormattingEnabled = true;
			comboBoxInputDevice.Location = new System.Drawing.Point(31, 18);
			comboBoxInputDevice.Name = "comboBoxInputDevice";
			comboBoxInputDevice.Size = new System.Drawing.Size(305, 33);
			comboBoxInputDevice.TabIndex = 5;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanel1.Location = new System.Drawing.Point(34, 582);
			tableLayoutPanel1.Margin = new Padding(4, 5, 4, 5);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 12;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
			tableLayoutPanel1.Size = new System.Drawing.Size(834, 343);
			tableLayoutPanel1.TabIndex = 6;
			// 
			// Form1
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(894, 985);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(comboBoxInputDevice);
			Controls.Add(textBoxBarcode);
			Controls.Add(pictureBoxVideoCapture);
			Controls.Add(buttonConnectToCamera);
			Name = "Form1";
			Text = "Form1";
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;
			((System.ComponentModel.ISupportInitialize)pictureBoxVideoCapture).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button buttonConnectToCamera;
		private TextBox textBoxBarcode;
		private ListBox listBox1;
		private ComboBox comboBoxInputDevice;
		private TableLayoutPanel tableLayoutPanel1;
	}
}