
namespace Universe.Fias.Import.FormsApp
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
            this.btSync = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAllowHousesImport = new System.Windows.Forms.CheckBox();
            this.btConvertedCsvFilesPath = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbConvertedCsvFilesPath = new System.Windows.Forms.TextBox();
            this.btMigrateDb = new System.Windows.Forms.Button();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.btSevenZipFolderPath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSevenZipFolderPath = new System.Windows.Forms.TextBox();
            this.btAddrSysImportBaseFolderPath = new System.Windows.Forms.Button();
            this.tbAddrSysImportBaseFolderPath = new System.Windows.Forms.TextBox();
            this.cbDeleteFromTables = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSync
            // 
            this.btSync.Location = new System.Drawing.Point(26, 412);
            this.btSync.Name = "btSync";
            this.btSync.Size = new System.Drawing.Size(161, 34);
            this.btSync.TabIndex = 0;
            this.btSync.Text = "Импорт";
            this.btSync.UseVisualStyleBackColor = true;
            this.btSync.Click += new System.EventHandler(this.btSync_Click);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(634, 46);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(423, 400);
            this.tbLog.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(634, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Лог:";
            // 
            // btCancel
            // 
            this.btCancel.Enabled = false;
            this.btCancel.Location = new System.Drawing.Point(208, 412);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(161, 34);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Размещение файлов ФИАС:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDeleteFromTables);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbAllowHousesImport);
            this.groupBox1.Controls.Add(this.btConvertedCsvFilesPath);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbConvertedCsvFilesPath);
            this.groupBox1.Controls.Add(this.btMigrateDb);
            this.groupBox1.Controls.Add(this.tbConnectionString);
            this.groupBox1.Controls.Add(this.btSevenZipFolderPath);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbSevenZipFolderPath);
            this.groupBox1.Controls.Add(this.btAddrSysImportBaseFolderPath);
            this.groupBox1.Controls.Add(this.tbAddrSysImportBaseFolderPath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(26, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(602, 378);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 38);
            this.label4.TabIndex = 19;
            this.label4.Text = "Строка подключения к БД ФИАС:";
            // 
            // cbAllowHousesImport
            // 
            this.cbAllowHousesImport.AutoSize = true;
            this.cbAllowHousesImport.Location = new System.Drawing.Point(6, 289);
            this.cbAllowHousesImport.Name = "cbAllowHousesImport";
            this.cbAllowHousesImport.Size = new System.Drawing.Size(145, 19);
            this.cbAllowHousesImport.TabIndex = 18;
            this.cbAllowHousesImport.Text = "Импортировать дома";
            this.cbAllowHousesImport.UseVisualStyleBackColor = true;
            // 
            // btConvertedCsvFilesPath
            // 
            this.btConvertedCsvFilesPath.Location = new System.Drawing.Point(542, 76);
            this.btConvertedCsvFilesPath.Name = "btConvertedCsvFilesPath";
            this.btConvertedCsvFilesPath.Size = new System.Drawing.Size(53, 41);
            this.btConvertedCsvFilesPath.TabIndex = 17;
            this.btConvertedCsvFilesPath.Text = "...";
            this.btConvertedCsvFilesPath.UseVisualStyleBackColor = true;
            this.btConvertedCsvFilesPath.Click += new System.EventHandler(this.btConvertedCsvFilesPath_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 38);
            this.label5.TabIndex = 16;
            this.label5.Text = "Путь преобразованным к CSV-файлам:";
            // 
            // tbConvertedCsvFilesPath
            // 
            this.tbConvertedCsvFilesPath.Location = new System.Drawing.Point(182, 76);
            this.tbConvertedCsvFilesPath.Multiline = true;
            this.tbConvertedCsvFilesPath.Name = "tbConvertedCsvFilesPath";
            this.tbConvertedCsvFilesPath.Size = new System.Drawing.Size(354, 41);
            this.tbConvertedCsvFilesPath.TabIndex = 15;
            // 
            // btMigrateDb
            // 
            this.btMigrateDb.Location = new System.Drawing.Point(6, 234);
            this.btMigrateDb.Name = "btMigrateDb";
            this.btMigrateDb.Size = new System.Drawing.Size(590, 38);
            this.btMigrateDb.TabIndex = 13;
            this.btMigrateDb.Text = "Сформировать/Обновить БД";
            this.btMigrateDb.UseVisualStyleBackColor = true;
            this.btMigrateDb.Click += new System.EventHandler(this.btMigrateDb_Click);
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Location = new System.Drawing.Point(182, 161);
            this.tbConnectionString.Multiline = true;
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(413, 67);
            this.tbConnectionString.TabIndex = 10;
            // 
            // btSevenZipFolderPath
            // 
            this.btSevenZipFolderPath.Location = new System.Drawing.Point(542, 47);
            this.btSevenZipFolderPath.Name = "btSevenZipFolderPath";
            this.btSevenZipFolderPath.Size = new System.Drawing.Size(53, 24);
            this.btSevenZipFolderPath.TabIndex = 9;
            this.btSevenZipFolderPath.Text = "...";
            this.btSevenZipFolderPath.UseVisualStyleBackColor = true;
            this.btSevenZipFolderPath.Click += new System.EventHandler(this.btSevenZipFolderPath_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Путь к 7-Zip:";
            // 
            // tbSevenZipFolderPath
            // 
            this.tbSevenZipFolderPath.Location = new System.Drawing.Point(182, 47);
            this.tbSevenZipFolderPath.Name = "tbSevenZipFolderPath";
            this.tbSevenZipFolderPath.Size = new System.Drawing.Size(354, 23);
            this.tbSevenZipFolderPath.TabIndex = 7;
            // 
            // btAddrSysImportBaseFolderPath
            // 
            this.btAddrSysImportBaseFolderPath.Location = new System.Drawing.Point(542, 17);
            this.btAddrSysImportBaseFolderPath.Name = "btAddrSysImportBaseFolderPath";
            this.btAddrSysImportBaseFolderPath.Size = new System.Drawing.Size(53, 24);
            this.btAddrSysImportBaseFolderPath.TabIndex = 6;
            this.btAddrSysImportBaseFolderPath.Text = "...";
            this.btAddrSysImportBaseFolderPath.UseVisualStyleBackColor = true;
            this.btAddrSysImportBaseFolderPath.Click += new System.EventHandler(this.btAddrSysImportBaseFolderPath_Click);
            // 
            // tbAddrSysImportBaseFolderPath
            // 
            this.tbAddrSysImportBaseFolderPath.Location = new System.Drawing.Point(182, 18);
            this.tbAddrSysImportBaseFolderPath.Name = "tbAddrSysImportBaseFolderPath";
            this.tbAddrSysImportBaseFolderPath.Size = new System.Drawing.Size(354, 23);
            this.tbAddrSysImportBaseFolderPath.TabIndex = 5;
            // 
            // cbDeleteFromTables
            // 
            this.cbDeleteFromTables.AutoSize = true;
            this.cbDeleteFromTables.Checked = true;
            this.cbDeleteFromTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDeleteFromTables.Location = new System.Drawing.Point(6, 314);
            this.cbDeleteFromTables.Name = "cbDeleteFromTables";
            this.cbDeleteFromTables.Size = new System.Drawing.Size(273, 19);
            this.cbDeleteFromTables.TabIndex = 20;
            this.cbDeleteFromTables.Text = "Удаление существующих даннных из таблиц";
            this.cbDeleteFromTables.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 472);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btSync);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "FIAS Import Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btSync;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btAddrSysImportBaseFolderPath;
        private System.Windows.Forms.TextBox tbAddrSysImportBaseFolderPath;
        private System.Windows.Forms.Button btSevenZipFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSevenZipFolderPath;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.Button btMigrateDb;
        private System.Windows.Forms.Button btConvertedCsvFilesPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbConvertedCsvFilesPath;
        private System.Windows.Forms.CheckBox cbAllowHousesImport;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbDeleteFromTables;
    }
}

