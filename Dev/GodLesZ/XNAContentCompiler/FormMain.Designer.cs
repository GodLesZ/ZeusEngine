namespace XNAContentCompiler
{
    partial class FormMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFileTypes = new System.Windows.Forms.ComboBox();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.listResources = new System.Windows.Forms.ListBox();
            this.btnOpenDestinationDir = new System.Windows.Forms.Button();
            this.txtDestinationDir = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCompile = new System.Windows.Forms.Button();
            this.imageProgressBar = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnCompiling = new System.Windows.Forms.Panel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.imageProgressBar)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.pnCompiling.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Type:";
            // 
            // cmbFileTypes
            // 
            this.cmbFileTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFileTypes.FormattingEnabled = true;
            this.cmbFileTypes.Location = new System.Drawing.Point(309, 4);
            this.cmbFileTypes.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFileTypes.Name = "cmbFileTypes";
            this.cmbFileTypes.Size = new System.Drawing.Size(202, 24);
            this.cmbFileTypes.TabIndex = 1;
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(49, 35);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(419, 23);
            this.txtFilename.TabIndex = 2;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(474, 35);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(37, 23);
            this.btnOpenFile.TabIndex = 3;
            this.btnOpenFile.Text = "...";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // listResources
            // 
            this.listResources.FormattingEnabled = true;
            this.listResources.ItemHeight = 16;
            this.listResources.Location = new System.Drawing.Point(9, 64);
            this.listResources.Name = "listResources";
            this.listResources.Size = new System.Drawing.Size(350, 180);
            this.listResources.TabIndex = 4;
            // 
            // btnOpenDestinationDir
            // 
            this.btnOpenDestinationDir.Location = new System.Drawing.Point(474, 250);
            this.btnOpenDestinationDir.Name = "btnOpenDestinationDir";
            this.btnOpenDestinationDir.Size = new System.Drawing.Size(37, 23);
            this.btnOpenDestinationDir.TabIndex = 6;
            this.btnOpenDestinationDir.Text = "...";
            this.btnOpenDestinationDir.UseVisualStyleBackColor = true;
            this.btnOpenDestinationDir.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // txtDestinationDir
            // 
            this.txtDestinationDir.Location = new System.Drawing.Point(66, 250);
            this.txtDestinationDir.Name = "txtDestinationDir";
            this.txtDestinationDir.Size = new System.Drawing.Size(402, 23);
            this.txtDestinationDir.TabIndex = 5;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(365, 93);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(103, 24);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(365, 64);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(103, 24);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "File:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 253);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Output:";
            // 
            // btnCompile
            // 
            this.btnCompile.Location = new System.Drawing.Point(365, 279);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(103, 24);
            this.btnCompile.TabIndex = 11;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // imageProgressBar
            // 
            this.imageProgressBar.Image = global::XNAContentCompiler.Properties.Resources.validation_anim;
            this.imageProgressBar.Location = new System.Drawing.Point(4, 26);
            this.imageProgressBar.Name = "imageProgressBar";
            this.imageProgressBar.Size = new System.Drawing.Size(117, 10);
            this.imageProgressBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imageProgressBar.TabIndex = 12;
            this.imageProgressBar.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Compiling...";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.cmbFileTypes);
            this.pnlMain.Controls.Add(this.label1);
            this.pnlMain.Controls.Add(this.txtFilename);
            this.pnlMain.Controls.Add(this.btnCompile);
            this.pnlMain.Controls.Add(this.btnOpenFile);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.listResources);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.txtDestinationDir);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Controls.Add(this.btnOpenDestinationDir);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Location = new System.Drawing.Point(1, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(516, 307);
            this.pnlMain.TabIndex = 14;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(365, 123);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(103, 24);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pnCompiling
            // 
            this.pnCompiling.Controls.Add(this.label4);
            this.pnCompiling.Controls.Add(this.imageProgressBar);
            this.pnCompiling.Location = new System.Drawing.Point(378, 202);
            this.pnCompiling.Name = "pnCompiling";
            this.pnCompiling.Size = new System.Drawing.Size(130, 41);
            this.pnCompiling.TabIndex = 15;
            this.pnCompiling.Visible = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 311);
            this.Controls.Add(this.pnCompiling);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deploy - XNA Content Compiler";
            ((System.ComponentModel.ISupportInitialize)(this.imageProgressBar)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnCompiling.ResumeLayout(false);
            this.pnCompiling.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFileTypes;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.ListBox listResources;
        private System.Windows.Forms.Button btnOpenDestinationDir;
        private System.Windows.Forms.TextBox txtDestinationDir;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.PictureBox imageProgressBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnCompiling;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button btnClear;
    }
}

