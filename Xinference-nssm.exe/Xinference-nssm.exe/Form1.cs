using System;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace AI
{
    public partial class Form1 : Form
    {
        private ServiceController serviceController;
        private Timer statusCheckTimer;
        private bool isExiting = false;

        public Form1()
        {
            InitializeComponent();
            
            // 初始化服务控制器
            serviceController = new ServiceController("AI.xinference");
            
            // 设置窗体启动时最小化到系统托盘
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            
            // 初始化状态检查定时器
            statusCheckTimer = new Timer();
            statusCheckTimer.Interval = 2000; // 每2秒检查一次服务状态
            statusCheckTimer.Tick += StatusCheckTimer_Tick;
            statusCheckTimer.Start();
            
            // 首次检查服务状态
            UpdateServiceStatus();
        }

        private void UpdateServiceStatus()
        {
            try
            {
                serviceController.Refresh();
                switch (serviceController.Status)
                {
                    case ServiceControllerStatus.Running:
                        // 使用自定义的 ON.ico 图标
                        notifyIcon1.Icon = new Icon(Path.Combine(Application.StartupPath, "ON.ico"));
                        notifyIcon1.Text = "AI.xinference 服务正在运行";
                        break;
                    case ServiceControllerStatus.Stopped:
                        // 使用自定义的 OFF.ico 图标
                        notifyIcon1.Icon = new Icon(Path.Combine(Application.StartupPath, "OFF.ico"));
                        notifyIcon1.Text = "AI.xinference 服务已停止";
                        break;
                    default:
                        notifyIcon1.Icon = SystemIcons.Warning;
                        notifyIcon1.Text = "AI.xinference 服务状态: " + serviceController.Status.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                notifyIcon1.Icon = SystemIcons.Warning;
                notifyIcon1.Text = "无法获取服务状态: " + ex.Message;
            }
        }

        private void StatusCheckTimer_Tick(object sender, EventArgs e)
        {
            UpdateServiceStatus();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 双击显示服务状态
            MessageBox.Show(notifyIcon1.Text, "服务状态", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    if (MessageBox.Show("确定要停止 AI.xinference 服务吗？", "确认", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        UpdateServiceStatus();
                    }
                }
                else
                {
                    MessageBox.Show("服务当前未运行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("停止服务时出错: " + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    if (MessageBox.Show("确定要启动 AI.xinference 服务吗？", "确认", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        UpdateServiceStatus();
                    }
                }
                else
                {
                    MessageBox.Show("服务已在运行中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动服务时出错: " + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 先检查服务是否在运行
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    if (MessageBox.Show("退出前需要停止服务，是否继续？", "确认", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // 停止服务
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        
                        // 结束 nssm.exe 进程
                        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("nssm");
                        foreach (System.Diagnostics.Process process in processes)
                        {
                            process.Kill();
                        }
                        
                        // 退出应用程序
                        Application.Exit();
                    }
                }
                else
                {
                    // 如果服务已经停止，直接结束 nssm.exe 进程并退出
                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("nssm");
                    foreach (System.Diagnostics.Process process in processes)
                    {
                        process.Kill();
                    }
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("退出时发生错误: " + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 如果不是通过退出菜单关闭，则最小化到托盘
                if (!isExiting)
                {
                    e.Cancel = true;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
            }
            else
            {
                statusCheckTimer.Stop();
                notifyIcon1.Visible = false;
                base.OnFormClosing(e);
            }
        }
    }
}
