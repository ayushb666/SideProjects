namespace QRCodeDecoder
{
    partial class Form1
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
            this.selectPic = new System.Windows.Forms.LinkLabel();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.message = new System.Windows.Forms.Label();
            this.decoder = new System.Windows.Forms.Button();
            this.qrImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.qrImage)).BeginInit();
            this.SuspendLayout();
            // 
            // selectPic
            // 
            this.selectPic.AutoSize = true;
            this.selectPic.Location = new System.Drawing.Point(12, 9);
            this.selectPic.Name = "selectPic";
            this.selectPic.Size = new System.Drawing.Size(103, 20);
            this.selectPic.TabIndex = 0;
            this.selectPic.TabStop = true;
            this.selectPic.Text = "Select Image";
            this.selectPic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "openFileDialog1";
            this.fileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialog_FileOk);
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.Location = new System.Drawing.Point(12, 526);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(74, 20);
            this.message.TabIndex = 3;
            this.message.Text = "Message";
            // 
            // decoder
            // 
            this.decoder.Location = new System.Drawing.Point(460, 215);
            this.decoder.Name = "decoder";
            this.decoder.Size = new System.Drawing.Size(253, 89);
            this.decoder.TabIndex = 4;
            this.decoder.Text = "Decode";
            this.decoder.UseVisualStyleBackColor = true;
            this.decoder.Click += new System.EventHandler(this.button1_Click);
            // 
            // qrImage
            // 
            this.qrImage.Location = new System.Drawing.Point(16, 46);
            this.qrImage.Name = "qrImage";
            this.qrImage.Size = new System.Drawing.Size(438, 447);
            this.qrImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.qrImage.TabIndex = 6;
            this.qrImage.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 577);
            this.Controls.Add(this.qrImage);
            this.Controls.Add(this.decoder);
            this.Controls.Add(this.message);
            this.Controls.Add(this.selectPic);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.qrImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel selectPic;
        private System.Windows.Forms.Label message;
        private System.Windows.Forms.Button decoder;
        private System.Windows.Forms.PictureBox qrImage;
        private System.Windows.Forms.OpenFileDialog fileDialog;

    }
}

