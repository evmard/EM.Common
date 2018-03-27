using Crm.DbUpdater.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Crm.Patch.Creator
{
    public partial class MainForm : Form
    {
        Regex verEditRx = new Regex(@"^(\d+)?(?(1)\.)?(\d+)?(?(2)\.)?(\d+)?$");
        Regex fullVersRx = new Regex(@"^(\d+\.\d+\.\d+)$");
        Regex nameRx = new Regex(@"^\w+$");

        public MainForm()
        {
            InitializeComponent();
            ValidateParams(null, null);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var fdb = new FolderBrowserDialog())
            {
                if (fdb.ShowDialog(this) != DialogResult.OK)
                    return;

                tbDir.Text = fdb.SelectedPath;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var path = tbDir.Text;
            if (!Directory.Exists(path))
            {
                MessageBox.Show(this, $"Каталог {path} не найден.", "Список файлов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var files = Directory.GetFiles(path);
            clbFiles.Items.Clear();
            clbFiles.Items.AddRange(files);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void VersionEditKeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || e.KeyChar == '\b')
                return;

            var text =  textBox.Text + e.KeyChar;
            if (verEditRx.IsMatch(text))
                textBox.Text += e.KeyChar;

            textBox.SelectionStart = textBox.Text.Length;
            e.Handled = true;
        }

        private bool valid;
        private void ValidateParams(object sender, EventArgs e)
        {
            valid = true;
            string tips = string.Empty;

            if (!fullVersRx.IsMatch(tbFrom.Text))
            {
                tips += "'Версия с' имеет не верный формат.\n";
                valid = false;
            }

            if (!fullVersRx.IsMatch(tbTo.Text))
            {
                tips += "'Версия по' имеет не верный формат.\n";
                valid = false;
            }

            if (clbFiles.CheckedItems.Count == 0)
            {
                tips += "Файлы не выбраны.\n";
                valid = false;
            }

            toolTip.SetToolTip(btnCreate, valid ? "Создать патч" : tips);            
        }

        private void NameEditKeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || e.KeyChar == '\b')
                return;

            var text = textBox.Text + e.KeyChar;
            if (nameRx.IsMatch(text))
                textBox.Text += e.KeyChar;

            textBox.SelectionStart = textBox.Text.Length;
            e.Handled = true;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!valid)
                return;

            List<string> files = new List<string>();
            foreach (string cFile in clbFiles.CheckedItems)
                files.Add(cFile);

            var fromVer = new Version(tbFrom.Text);
            var toVer = new Version(tbTo.Text);

            var patchName = $"version_{toVer.Major}_{toVer.Minor}_{toVer.Build}";

            var currDir = Directory.GetCurrentDirectory();
            try
            {
                UpdaterCore.CreatePath(patchName, currDir, files, fromVer, toVer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Создание патча", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
