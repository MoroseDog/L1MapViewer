using L1MapViewer.Helper;
using L1MapViewer.Other;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static L1MapViewer.Other.Struct;

namespace L1MapViewer {
    public partial class Form1 : Form {

        private static Form1 instance;

        public static Form1 Get() {
            return instance;
        }

        public Form1() {
            InitializeComponent();
            instance = this;
        }

        //按下Open選擇天堂目錄
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(toolStripStatusLabel3.Text)) {
                return;
            }
            BetterFolderBrowser bfBrowser = new BetterFolderBrowser();
            bfBrowser.Title = "請選擇天堂資料夾";
            bfBrowser.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            //路徑的紀錄
            string iniFile = Path.GetTempPath() + "mapviewer.ini";
            if (File.Exists(iniFile)) {
                bfBrowser.RootFolder = Utils.GetINI("Path", "LineagePath", "", iniFile);
            }

            if (bfBrowser.ShowDialog() != DialogResult.OK) {
                return;
            }
            if (string.IsNullOrEmpty(bfBrowser.SelectedPath)) {
                return;
            }
            //停用Open按鈕
            openToolStripMenuItem.Enabled = false;

            //選的路徑 
            toolStripStatusLabel3.Text = bfBrowser.SelectedPath;

            //紀錄選的路徑 
            Utils.WriteINI("Path", "LineagePath", Directory.GetParent(bfBrowser.SelectedPath).FullName, iniFile);

            //讀取地圖檔
            LoadMap(bfBrowser.SelectedPath);

        }

        //comboBox1顯示地圖檔名稱
        public void LoadMap(string selectedPath) {
            Utils.ShowProgressBar(true);
            Dictionary<string, L1Map> result = L1MapHelper.Read(selectedPath);

            comboBox1.Items.Clear();

            //填入地圖名稱
            comboBox1.BeginUpdate();
            foreach (string key in Utils.SortAsc(result.Keys)) {
                L1Map map = result[key];
                comboBox1.Items.Add(string.Format("{0}-{1}", key, map.szName));
            }
            comboBox1.EndUpdate();
            comboBox1.SelectedIndex = 5;

            toolStripStatusLabel2.Text = "MapCount=" + result.Count;
            Utils.ShowProgressBar(false);
        }

        //自定義的滾動軸物件
        public void vScrollBar1_Scroll(object sender, ScrollEventArgs e) {
            pictureBox1.Top = -vScrollBar1.Value;
        }

        public void hScrollBar1_Scroll(object sender, ScrollEventArgs e) {
            pictureBox1.Left = -hScrollBar1.Value;
        }

        //記錄拖拽過程滑鼠位置
        private Point mouseDownPoint = new Point();
        private bool isMouseDrag = false;

        //滑鼠按下功能
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                Cursor = Cursors.Hand;
                isMouseDrag = true;

            } else if (e.Button == MouseButtons.Right) {
                L1MapHelper.doLocTagEvent(e);
            }
        }

        //滑鼠鬆開功能
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Cursor = Cursors.Default;
                isMouseDrag = false;
            }
        }
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e) {

            //拖曳中 表示在拉動圖片
            if (isMouseDrag) {
                if (hScrollBar1.Maximum == 0 || vScrollBar1.Maximum == 0) {
                    return;
                }

                int valueX = hScrollBar1.Value - (Cursor.Position.X - mouseDownPoint.X);
                int valueY = vScrollBar1.Value - (Cursor.Position.Y - mouseDownPoint.Y);

                if (valueX > hScrollBar1.Maximum) {
                    valueX = hScrollBar1.Maximum;
                }
                if (valueX < hScrollBar1.Minimum) {
                    valueX = hScrollBar1.Minimum;
                }

                if (valueY > vScrollBar1.Maximum) {
                    valueY = vScrollBar1.Maximum;
                }
                if (valueY < vScrollBar1.Minimum) {
                    valueY = vScrollBar1.Minimum;
                }
                hScrollBar1.Value = valueX;
                vScrollBar1.Value = valueY;
                vScrollBar1_Scroll(null, null);
                hScrollBar1_Scroll(null, null);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                return;
            }

            //其他滑鼠移動的事件處理
            L1MapHelper.doMouseMoveEvent(e);
        }
        //選擇地圖時
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) {
                return;
            }
            string selectName = comboBox1.SelectedItem.ToString();

            if (selectName.Contains("-")) {
                selectName = selectName.Split('-')[0].Trim();
            }
            L1MapHelper.doPaintEvent(selectName);
        }


    }
}
