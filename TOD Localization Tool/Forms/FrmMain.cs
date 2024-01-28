using Helper;

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TOD_Localization_Tool
{
    public partial class FrmMain : Form
    {


        MainAsset main;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Main file(*.main)|*.main";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                TxtMainPath.Text = OFD.FileName;
            }
        }

        private void LoadMainFile()
        {
            if (main != null)
            {
                main.Dispose();
            }
            if (!File.Exists(TxtMainPath.Text))
            {
                throw new Exception("Can't find main file!\n\n make sure you select right file.");
            }

            if (checkBox1.Checked)
            {
                if (!File.Exists(TxtMainPath.Text + ".bak"))
                {
                    File.Copy(TxtMainPath.Text, TxtMainPath.Text + ".bak");
                }
            }

            //if (File.Exists(TxtMainPath.Text + ".bak"))
            //{
            //    File.Copy(TxtMainPath.Text + ".bak",TxtMainPath.Text,true);
            //}

            main = new MainAsset(new MStream(TxtMainPath.Text));

            ClearLog();
        }

        private string GetFolderPath()
        {

            FolderDialog folderDialog = new FolderDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.FileName;
            }
            else
            {
                return null;
            }

        }

        private void ExportFonts_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMainFile();
                string FolderPath = GetFolderPath();

                if (FolderPath == null) return;
                Directory.SetCurrentDirectory(FolderPath);
                foreach (var entry in main.FilesEntres.Where(x => x.Key.EndsWith(".font")))
                {
                    var fontasset = main.GetFile(entry.Value) as FontAsset;

                    Log("Exporting: \"" + entry.Key + "\"");
                    Directory.CreateDirectory(Path.GetDirectoryName(entry.Key));
                    File.WriteAllText(entry.Key, fontasset.GetFontData());
                    File.WriteAllBytes(Path.ChangeExtension(entry.Key, ".dds"), fontasset.GetFontTexture());
                    // DDSToBitmap.Convert(fontasset.GetFontTexture()).Save(Path.ChangeExtension(entry.Key, ".png"));
                    LogLine(" Done");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportTexts_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMainFile();
                string FolderPath = GetFolderPath();
                if (FolderPath == null) return;

                Directory.SetCurrentDirectory(FolderPath);


                foreach (var entry in main.FilesEntres.Where(x => x.Key.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var fontasset = main.GetFile(entry.Value) as TextAsset;

                    Log("Exporting: \"" + entry.Key + "\"");

                    Directory.CreateDirectory(Path.GetDirectoryName(entry.Key));
                    File.WriteAllLines(entry.Key, fontasset.GetStringFormFile());
                    LogLine(" Done");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImportFonts_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMainFile();
                string FolderPath = GetFolderPath();
                if (FolderPath == null) return;

                Directory.SetCurrentDirectory(FolderPath);
                foreach (var entry in main.FilesEntres.Where(x => x.Key.EndsWith(".font")))
                {
                    var fontasset = main.GetFile(entry.Value) as FontAsset;

                    if (!File.Exists(entry.Key))
                    {
                        LogLine("Can't find: " + entry.Key + " Fail");
                        continue;
                    }

                    Log("Importing: \"" + entry.Key + "\"");

                    fontasset.EditFile(entry.Key);
                    LogLine(" Done");
                }
                main.SaveFile(TxtMainPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImportTexts_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMainFile();
                string FolderPath = GetFolderPath();
                if (FolderPath == null) return;

                Directory.SetCurrentDirectory(FolderPath);
                foreach (var entry in main.FilesEntres.Where(x => x.Key.EndsWith(".txt")))
                {
                    var fontasset = main.GetFile(entry.Value) as TextAsset;

                    if (!File.Exists(entry.Key))
                    {
                        LogLine("Can't find: " + entry.Key + " Fail");
                        continue;
                    }

                    Log("Importing: \"" + entry.Key + "\"");
                    fontasset.EditFile(File.ReadAllText(entry.Key));

                    LogLine(" Done");
                }
                main.SaveFile(TxtMainPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void Log(string text)
        {
            textBox1.AppendText(text);
        }
        private void LogLine(string text)
        {
            textBox1.AppendText(text + "\r\n");
        }

        private void ClearLog()
        {
            textBox1.Clear();
        }
    }
}
