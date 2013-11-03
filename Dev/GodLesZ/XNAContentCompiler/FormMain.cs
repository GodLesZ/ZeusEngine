// @TODO: Wont work, was old code, just refactored, find the bug!

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace XNAContentCompiler {

    public partial class FormMain : Form {
        private delegate void CallEnablePanels(Boolean enable);

        private ComboItemCollection _fileTypes;
        private ComboItemCollection _selectedFiles;
        private ContentBuilder _contentBuilder;

        public FormMain() {
            InitializeComponent();
            Load += FormMain_Load;
        }


        private void FormMain_Load(object sender, EventArgs e) {
            LoadFileTypes();

            _selectedFiles = new ComboItemCollection();
            _contentBuilder = new ContentBuilder();
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        private void btnOpenFile_Click(object sender, EventArgs e) {
            using (var jnl = new OpenFileDialog()) {
                jnl.Filter = (string) cmbFileTypes.SelectedValue;
                if (jnl.ShowDialog() == DialogResult.OK) {
                    txtFilename.Text = jnl.FileName;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            if (_selectedFiles.ContainsValue(txtFilename.Text) == false) {
                _selectedFiles.Add(new ComboItem(Path.GetFileName(txtFilename.Text), txtFilename.Text));
                LoadListOfFiles();
            } else {
                MessageBox.Show(@"The file is already in the collection.");
            }

            txtFilename.Text = String.Empty;
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (listResources.SelectedIndex < 0) {
                return;
            }

            if (MessageBox.Show(@"Do you really want to remove this item?", @"Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                return;
            }

            _selectedFiles.RemoveAt(listResources.SelectedIndex);
            LoadListOfFiles();
        }

        private void btnCompile_Click(object sender, EventArgs e) {
            if (_selectedFiles.Count > 0 && txtDestinationDir.Text.Trim().Length > 0) {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void btnOutput_Click(object sender, EventArgs e) {
            using (var jnl = new FolderBrowserDialog()) {
                if (jnl.ShowDialog() == DialogResult.OK) {
                    txtDestinationDir.Text = jnl.SelectedPath;
                }
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            EnablePanels(true);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            CallEnablePanels cm = EnablePanels;
            Invoke(cm, new object[] {false});

            _contentBuilder.Clear();
            foreach (var item in _selectedFiles) {
                _contentBuilder.Add(item);
            }

            var error = _contentBuilder.Build();
            if (!String.IsNullOrEmpty(error)) {
                MessageBox.Show(error);
                return;
            }

            var tempPath = _contentBuilder.OutputDirectory;
            var files = Directory.GetFiles(tempPath, "*.xnb");

            foreach (var file in files.Where(file => file != null)) {
                File.Copy(file, Path.Combine(txtDestinationDir.Text, Path.GetFileName(file)), true);
            }
            MessageBox.Show(@"Files compiled");
        }

        private void btnClear_Click(object sender, EventArgs e) {
            if (listResources.SelectedIndex < 0) {
                return;
            }

            if (MessageBox.Show(@"Do you really want to clear the list?", @"Clear List", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                return;
            }

            _selectedFiles.Clear();
            LoadListOfFiles();
        }


        private void LoadFileTypes() {
            _fileTypes = new ComboItemCollection {
                new ComboItem("Textures", "Image Files(*.bmp;*.jpg;*.png;*.tga;*.dds)|*.bmp;*.jpg;*.png;*.tga;*.dds"), 
                new ComboItem("Audio", "Audio Files(*.wav;*.mp3;*.wma)|*.wav;*.mp3;*.wma"), 
                new ComboItem("Fonts", "SpriteFont Files(*.spritefont)|*.spritefont"),
                new ComboItem("Model", "Model Files(*.fbx;*.x)|*.fbx;*.x"),
                new ComboItem("Effect", "Effect Files(*.fx)|*.fx")
            };

            cmbFileTypes.DataSource = _fileTypes;
            cmbFileTypes.DisplayMember = "Name";
            cmbFileTypes.ValueMember = "Value";
            cmbFileTypes.Refresh();
        }

        private void LoadListOfFiles() {
            listResources.DataSource = null;
            listResources.Refresh();
            listResources.DataSource = _selectedFiles;
            listResources.DisplayMember = "Name";
            listResources.ValueMember = "Value";
            listResources.Refresh();
        }

        private void EnablePanels(Boolean enable) {
            pnlMain.Enabled = enable;
            pnCompiling.Visible = !enable;
        }

    }

}