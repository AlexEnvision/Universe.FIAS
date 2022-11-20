
namespace Universe.Fias.XML.GAR.Converter.x64.FormsApp
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
            this.label6 = new System.Windows.Forms.Label();
            this.btConvertedCsvFilesPath = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbConvertedCsvFilesPath = new System.Windows.Forms.TextBox();
            this.cbAddrSysFiasDownloadServiceEnable = new System.Windows.Forms.CheckBox();
            this.btMigrateDb = new System.Windows.Forms.Button();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.btSevenZipFolderPath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSevenZipFolderPath = new System.Windows.Forms.TextBox();
            this.btAddrSysImportBaseFolderPath = new System.Windows.Forms.Button();
            this.tbAddrSysImportBaseFolderPath = new System.Windows.Forms.TextBox();
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
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btConvertedCsvFilesPath);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbConvertedCsvFilesPath);
            this.groupBox1.Controls.Add(this.cbAddrSysFiasDownloadServiceEnable);
            this.groupBox1.Controls.Add(this.btMigrateDb);
            this.groupBox1.Controls.Add(this.tbConnectionString);
            this.groupBox1.Controls.Add(this.btSevenZipFolderPath);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbSevenZipFolderPath);
            this.groupBox1.Controls.Add(this.btAddrSysImportBaseFolderPath);
            this.groupBox1.Controls.Add(this.tbAddrSysImportBaseFolderPath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(30, 24);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(688, 504);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(7, 209);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(185, 51);
            this.label6.TabIndex = 21;
            this.label6.Text = "Строка подключения к БД ФИАС:";
            // 
            // btConvertedCsvFilesPath
            // 
            this.btConvertedCsvFilesPath.Location = new System.Drawing.Point(619, 101);
            this.btConvertedCsvFilesPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btConvertedCsvFilesPath.Name = "btConvertedCsvFilesPath";
            this.btConvertedCsvFilesPath.Size = new System.Drawing.Size(61, 55);
            this.btConvertedCsvFilesPath.TabIndex = 20;
            this.btConvertedCsvFilesPath.Text = "...";
            this.btConvertedCsvFilesPath.UseVisualStyleBackColor = true;
            this.btConvertedCsvFilesPath.Click += new System.EventHandler(this.btConvertedCsvFilesPath_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(185, 51);
            this.label5.TabIndex = 19;
            this.label5.Text = "Путь преобразованным к CSV-файлам:";
            // 
            // tbConvertedCsvFilesPath
            // 
            this.tbConvertedCsvFilesPath.Location = new System.Drawing.Point(199, 101);
            this.tbConvertedCsvFilesPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbConvertedCsvFilesPath.Multiline = true;
            this.tbConvertedCsvFilesPath.Name = "tbConvertedCsvFilesPath";
            this.tbConvertedCsvFilesPath.Size = new System.Drawing.Size(413, 53);
            this.tbConvertedCsvFilesPath.TabIndex = 18;
            // 
            // cbAddrSysFiasDownloadServiceEnable
            // 
            this.cbAddrSysFiasDownloadServiceEnable.AutoSize = true;
            this.cbAddrSysFiasDownloadServiceEnable.Location = new System.Drawing.Point(7, 380);
            this.cbAddrSysFiasDownloadServiceEnable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbAddrSysFiasDownloadServiceEnable.Name = "cbAddrSysFiasDownloadServiceEnable";
            this.cbAddrSysFiasDownloadServiceEnable.Size = new System.Drawing.Size(189, 24);
            this.cbAddrSysFiasDownloadServiceEnable.TabIndex = 14;
            this.cbAddrSysFiasDownloadServiceEnable.Text = "Скачать базу с nalog.ru";
            this.cbAddrSysFiasDownloadServiceEnable.UseVisualStyleBackColor = true;
            // 
            // btMigrateDb
            // 
            this.btMigrateDb.Location = new System.Drawing.Point(7, 307);
            this.btMigrateDb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btMigrateDb.Name = "btMigrateDb";
            this.btMigrateDb.Size = new System.Drawing.Size(674, 51);
            this.btMigrateDb.TabIndex = 13;
            this.btMigrateDb.Text = "Сформировать/Обновить БД";
            this.btMigrateDb.UseVisualStyleBackColor = true;
            this.btMigrateDb.Click += new System.EventHandler(this.btMigrateDb_Click);
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Location = new System.Drawing.Point(199, 209);
            this.tbConnectionString.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbConnectionString.Multiline = true;
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(481, 88);
            this.tbConnectionString.TabIndex = 10;
            // 
            // btSevenZipFolderPath
            // 
            this.btSevenZipFolderPath.Location = new System.Drawing.Point(619, 63);
            this.btSevenZipFolderPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSevenZipFolderPath.Name = "btSevenZipFolderPath";
            this.btSevenZipFolderPath.Size = new System.Drawing.Size(61, 32);
            this.btSevenZipFolderPath.TabIndex = 9;
            this.btSevenZipFolderPath.Text = "...";
            this.btSevenZipFolderPath.UseVisualStyleBackColor = true;
            this.btSevenZipFolderPath.Click += new System.EventHandler(this.btSevenZipFolderPath_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Путь к 7-Zip:";
            // 
            // tbSevenZipFolderPath
            // 
            this.tbSevenZipFolderPath.Location = new System.Drawing.Point(199, 63);
            this.tbSevenZipFolderPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbSevenZipFolderPath.Name = "tbSevenZipFolderPath";
            this.tbSevenZipFolderPath.Size = new System.Drawing.Size(413, 27);
            this.tbSevenZipFolderPath.TabIndex = 7;
            // 
            // btAddrSysImportBaseFolderPath
            // 
            this.btAddrSysImportBaseFolderPath.Location = new System.Drawing.Point(619, 23);
            this.btAddrSysImportBaseFolderPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btAddrSysImportBaseFolderPath.Name = "btAddrSysImportBaseFolderPath";
            this.btAddrSysImportBaseFolderPath.Size = new System.Drawing.Size(61, 32);
            this.btAddrSysImportBaseFolderPath.TabIndex = 6;
            this.btAddrSysImportBaseFolderPath.Text = "...";
            this.btAddrSysImportBaseFolderPath.UseVisualStyleBackColor = true;
            this.btAddrSysImportBaseFolderPath.Click += new System.EventHandler(this.btAddrSysImportBaseFolderPath_Click);
            // 
            // tbAddrSysImportBaseFolderPath
            // 
            this.tbAddrSysImportBaseFolderPath.Location = new System.Drawing.Point(199, 24);
            this.tbAddrSysImportBaseFolderPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbAddrSysImportBaseFolderPath.Name = "tbAddrSysImportBaseFolderPath";
            this.tbAddrSysImportBaseFolderPath.Size = new System.Drawing.Size(413, 27);
            this.tbAddrSysImportBaseFolderPath.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Размещение файлов GAR:";
            // 
            // btCancel
            // 
            this.btCancel.Enabled = false;
            this.btCancel.Location = new System.Drawing.Point(534, 536);
            this.btCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(184, 45);
            this.btCancel.TabIndex = 9;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(725, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Лог:";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(725, 48);
            this.tbLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(483, 532);
            this.tbLog.TabIndex = 7;
            // 
            // btSync
            // 
            this.btSync.Location = new System.Drawing.Point(30, 536);
            this.btSync.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSync.Name = "btSync";
            this.btSync.Size = new System.Drawing.Size(287, 45);
            this.btSync.TabIndex = 6;
            this.btSync.Text = "Загрузить/Преобразовать в CSV";
            this.btSync.UseVisualStyleBackColor = true;
            this.btSync.Click += new System.EventHandler(this.btSync_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1237, 613);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btSync);
            this.Name = "MainForm";
            this.Text = "GAR FIAS Downloader and Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btConvertedCsvFilesPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbConvertedCsvFilesPath;
        private System.Windows.Forms.CheckBox cbAddrSysFiasDownloadServiceEnable;
        private System.Windows.Forms.Button btMigrateDb;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.Button btSevenZipFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSevenZipFolderPath;
        private System.Windows.Forms.Button btAddrSysImportBaseFolderPath;
        private System.Windows.Forms.TextBox tbAddrSysImportBaseFolderPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btSync;
    }
}

