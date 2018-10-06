using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace nstaller2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;

        public MainWindow()
        {
            Log.InfoStamp("startup");

            InitializeComponent();

            //Read settings
            Log.Info("reading settings");
            settings = XML.ReadXML("cfg.xml");
            Log.Info("done reading settings");

            //Color elements green
            if (IsUserAdded())
                labelAccount.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (DoesUserHaveCorrectPassword())
                labelPassword.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsDotNetInstalled())
                labelDotNet.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsAgentInstalled())
                labelAgent.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsAVInstalled())
                labelAV.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsProbeInstalled())
                labelProbe.Foreground = new SolidColorBrush(Colors.DarkGreen);

            Log.Info("initialized");
        }

        private void ButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            Log.InfoStamp("checking add user");
            if (!IsUserAdded())
            {
                Log.Info("creating account");
                //Create account
                PrincipalContext p = new PrincipalContext(ContextType.Machine);
                UserPrincipal u = new UserPrincipal(p);
                u.Name = textBoxAccount.Text;
                u.DisplayName = "RMM Admin";
                u.Description = "Industry Systems RMM Account";
                u.SetPassword(textBoxPassword.Text);
                u.PasswordNeverExpires = true;
                u.UserCannotChangePassword = true;
                u.Save();

                Log.Info("adding account to admin");
                //Add account to administrators
                GroupPrincipal g = GroupPrincipal.FindByIdentity(p, "administrators");
                g.Members.Add(u);
                g.Save();

                Log.Info("hiding account");
                //Hide account
                RegistryKey rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList");
                rk.SetValue(textBoxAccount.Text, 00000000, RegistryValueKind.DWord);
            };

            Log.InfoStamp("checking dotnet");
            if (!IsDotNetInstalled())
                if ((bool)checkBoxDotNet.IsChecked)
                {
                    Log.Info("installing dotnet");
                    Run(FindFile(settings.bindotnet), "/q");
                };

            Log.InfoStamp("checking agent");
            if (!IsAgentInstalled())
                if ((bool)checkBoxAgent.IsChecked)
                {
                    Log.Info("installing agent");
                    Run(FindFile(settings.binagent), "/quiet");
                };

            Log.InfoStamp("checking av");
            if (!IsAVInstalled())
                if ((bool)checkBoxAV.IsChecked)
                    if (Environment.Is64BitOperatingSystem)
                    {
                        Log.Info("installing avx64");
                        Run(FindFile(settings.binav64), "/quiet");
                    }
                    else
                    {
                        Log.Info("installing avx86");
                        Run(FindFile(settings.binav86), "/quiet");
                    }

            Log.InfoStamp("checking probe");
            if (!IsAgentInstalled())
                if ((bool)checkBoxProbe.IsChecked)
                {
                    Log.Info("installing probe");
                    Run(FindFile(settings.binprobe), "/quiet");
                };

            Log.InfoStamp("closing");
            this.Close();
        }

        public void Run(string exe, string arg)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = exe;
                p.StartInfo.Arguments = arg;
                p.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string FindFile(string input)
        {
            foreach (string file in Directory.GetFiles(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString() + "\\" + settings.bindir))
                if (file.Contains(input))
                    return file;
            return "null.exe"; //file not found
        }

        public bool IsUserAdded()
        {
            try
            {
                if (null != UserPrincipal.FindByIdentity(new PrincipalContext(ContextType.Machine), IdentityType.SamAccountName, textBoxAccount.Text))
                    return true;
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DoesUserHaveCorrectPassword()
        {
            return false;
        }

        public bool IsDotNetInstalled()
        {
            RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            string[] version_names = installed_versions.GetSubKeyNames();
            double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
            if (Framework >= 4.0)
                return true;
            return false;
        }

        public bool IsAgentInstalled()
        {
            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName.Contains("agent"))
                    return true;
            return false;
        }

        public bool IsAVInstalled()
        {
            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName.Contains("agent"))
                    return true;
            return false;
        }

        public bool IsProbeInstalled()
        {
            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName.Contains("probe"))
                    return true;
            return false;
        }

        private void LabelAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            labelAccount.Visibility = Visibility.Hidden;
            textBoxAccount.Visibility = Visibility.Visible;
        }

        private void LabelPassword_MouseEnter(object sender, MouseEventArgs e)
        {
            labelPassword.Visibility = Visibility.Hidden;
            textBoxPassword.Visibility = Visibility.Visible;
        }

        private void TextBoxAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            labelAccount.Visibility = Visibility.Visible;
            textBoxAccount.Visibility = Visibility.Hidden;
        }

        private void TextBoxPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            labelPassword.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Hidden;
        }
    }
}
