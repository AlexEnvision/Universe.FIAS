
namespace Universe.Fias.Normalizer.FormsApp
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btMigrateDb = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAddrTwo = new System.Windows.Forms.TextBox();
            this.tbAddrOne = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btSync = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btMigrateDb);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbConnectionString);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbAddrTwo);
            this.groupBox1.Controls.Add(this.tbAddrOne);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(602, 378);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // btMigrateDb
            // 
            this.btMigrateDb.Location = new System.Drawing.Point(6, 110);
            this.btMigrateDb.Name = "btMigrateDb";
            this.btMigrateDb.Size = new System.Drawing.Size(590, 38);
            this.btMigrateDb.TabIndex = 13;
            this.btMigrateDb.Text = "Сформировать/Обновить БД";
            this.btMigrateDb.UseVisualStyleBackColor = true;
            this.btMigrateDb.Click += new System.EventHandler(this.btMigrateDb_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Connection string:";
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Location = new System.Drawing.Point(174, 37);
            this.tbConnectionString.Multiline = true;
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(421, 67);
            this.tbConnectionString.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Адрес 2:";
            // 
            // tbAddrTwo
            // 
            this.tbAddrTwo.Location = new System.Drawing.Point(65, 225);
            this.tbAddrTwo.Name = "tbAddrTwo";
            this.tbAddrTwo.Size = new System.Drawing.Size(531, 23);
            this.tbAddrTwo.TabIndex = 7;
            // 
            // tbAddrOne
            // 
            this.tbAddrOne.Location = new System.Drawing.Point(65, 192);
            this.tbAddrOne.Name = "tbAddrOne";
            this.tbAddrOne.Size = new System.Drawing.Size(531, 23);
            this.tbAddrOne.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 197);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Адрес 1:";
            // 
            // btCancel
            // 
            this.btCancel.Enabled = false;
            this.btCancel.Location = new System.Drawing.Point(453, 408);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(161, 34);
            this.btCancel.TabIndex = 9;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(620, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Лог:";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(620, 42);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(423, 400);
            this.tbLog.TabIndex = 7;
            // 
            // btSync
            // 
            this.btSync.Location = new System.Drawing.Point(12, 408);
            this.btSync.Name = "btSync";
            this.btSync.Size = new System.Drawing.Size(251, 34);
            this.btSync.TabIndex = 6;
            this.btSync.Text = "Сравнить адреса";
            this.btSync.UseVisualStyleBackColor = true;
            this.btSync.Click += new System.EventHandler(this.btSync_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 480);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btSync);
            this.Name = "MainForm";
            this.Text = "Приложение обработки адресов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btMigrateDb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbAddrTwo;
        private System.Windows.Forms.TextBox tbAddrOne;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btSync;
    }
}

