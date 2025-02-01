using System.Diagnostics;
using System.IO;
using System.Windows;

namespace OrnaBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread clickThread;
        private bool isClicking = false;
        public MainWindow()
        {
            InitializeComponent();
            StartStopButton.Checked += StartStopButton_Checked;
            StartStopButton.Unchecked += StartStopButton_Unchecked;
        }
        private async void StartStopButton_Checked(object sender, RoutedEventArgs e)
        {
            // 开始点击
            if (!isClicking)
            {
                isClicking = true;
                clickThread = new Thread(ClickLoop);
                clickThread.Start();
            }
        }

        private async void StartStopButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // 停止点击
            if (isClicking)
            {
                isClicking = false;
                clickThread?.Join(); // 等待线程结束
            }
        }

        private void ClickLoop()
        {
            Random random = new Random();
            int baseX = 264;
            int baseY = 2000;

            while (isClicking)
            {
                // 计算浮动后的坐标
                int offsetX = random.Next(-10, 11); // -10 到 10之间的随机数
                int offsetY = random.Next(-10, 11); // -10 到 10之间的随机数

                int finalX = baseX + offsetX;
                int finalY = baseY + offsetY;

                // 执行 adb 命令来模拟点击
                Tap(finalX, finalY);

                // 每5秒点击一次
                Thread.Sleep(5000);
            }
        }

        //private void Tap(int x, int y)
        //{
        //    // 使用 adb 命令模拟点击
        //    var startInfo = new ProcessStartInfo
        //    {
        //        FileName = "adb",
        //        Arguments = $"shell input tap {x} {y}",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    Process process = new Process { StartInfo = startInfo };
        //    process.Start();
        //    process.WaitForExit();
        //}
        private void Tap(int x, int y)
        {
            // 获取当前应用程序目录的路径
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // 拼接出 adb.exe 的路径
            string adbPath = Path.Combine(appDirectory, "adb", "adb.exe");

            // 检查文件是否存在
            if (!File.Exists(adbPath))
            {
                MessageBox.Show($"未找到 adb.exe 文件，请确保文件存在于 {adbPath} 路径下。");
                return;
            }

            // 使用 adb 命令模拟点击
            var startInfo = new ProcessStartInfo
            {
                FileName = adbPath,
                Arguments = $"shell input tap {x} {y}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();
        }
    }
}