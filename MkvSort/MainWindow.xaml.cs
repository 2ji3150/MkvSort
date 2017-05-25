using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MkvSort {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Listview_sorce = new ObservableCollection<ProcessPreview>();
            listView.DataContext = Listview_sorce;
        }

        List<string> OrgFIlePath = new List<string>();
        List<string> NewFIleNames = new List<string>();

        ObservableCollection<ProcessPreview> Listview_sorce;

        private void Open_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog() {
                Filter = "MKV Files (*.mkv)|*.mkv|ASS Files(*.ass) | *.ass",
                Multiselect = true
            };
            if (dlg.ShowDialog() == true) {
                Listview_sorce.Clear();
                int i = 1;
                foreach (string file in dlg.FileNames) {
                    string orgname = Path.GetFileName(file);
                    string newname = $"{AnimeName.Text} {i:D2}{Path.GetExtension(file)}";
                    if (orgname != newname) {
                        Listview_sorce.Add(new ProcessPreview() { Before = orgname, After = newname });
                        OrgFIlePath.Add(file);
                        NewFIleNames.Add(Path.Combine(Directory.GetParent(file).FullName, newname));
                    }
                    else Listview_sorce.Add(new ProcessPreview() { Before = orgname });
                    i++;
                }
                Task.Run(() => Dispatcher.BeginInvoke((Action)(() => Resizelistviewcolumn()), DispatcherPriority.Background));
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e) {
            if (!(OrgFIlePath.Count > 0)) return;
            for (int k = 0; k < OrgFIlePath.Count; k++) {
                try {
                    File.Move(OrgFIlePath[k], NewFIleNames[k]);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
            OrgFIlePath.Clear();
            NewFIleNames.Clear();
            Listview_sorce.Clear();
            Task.Run(() => Dispatcher.BeginInvoke((Action)(() => Resizelistviewcolumn()), DispatcherPriority.Background));
            SystemSounds.Asterisk.Play();
            MessageBox.Show("完成です");
        }
        private const double LIST_VIEW_COLUMN_MARGIN = 10;
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e) => Resizelistviewcolumn();

        public void Resizelistviewcolumn() {
            GridView gView = listView.View as GridView;
            var listBoxChrome = VisualTreeHelper.GetChild(listView, 0) as FrameworkElement;
            var scrollViewer = VisualTreeHelper.GetChild(listBoxChrome, 0) as ScrollViewer;
            var scrollBar = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as ScrollBar;
            var w = scrollBar.ActualWidth;
            var workingWidth = listView.ActualWidth - LIST_VIEW_COLUMN_MARGIN - w;
            gView.Columns[0].Width = workingWidth * 0.5;
            gView.Columns[1].Width = workingWidth * 0.5;
        }
    }
}
