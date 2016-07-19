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
        public MainWindow()
        {
            InitializeComponent();

            //Color elements green
            if (IsUserAdded())
                labelAccount.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (DoesUserHaveCorrectPassword())
                labelPassword.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsNetInstalled())
                labelNet.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsAgentInstalled())
                labelAgent.Foreground = new SolidColorBrush(Colors.DarkGreen);
            if (IsProbeInstalled())
                labelProbe.Foreground = new SolidColorBrush(Colors.DarkGreen);
        }

        private void buttonInstall_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserAdded())
            {
                PrincipalContext p = new PrincipalContext(ContextType.Machine);
                UserPrincipal u = new UserPrincipal(p);
                u.Name = textBoxAccount.Text;
                u.DisplayName = "RMM Admin";
                u.Description = "Industry Systems RMM Account";
                u.SetPassword(textBoxPassword.Text);
                u.PasswordNeverExpires = true;
                u.UserCannotChangePassword = true;
                u.Save();

                GroupPrincipal g = GroupPrincipal.FindByIdentity(p, "administrators");
                g.Members.Add(u);
                g.Save();
            };

            if (!IsNetInstalled())
                Run(FindFile("net"), "/q");

            if (IsAgentInstalled())
                if ((bool)checkBoxAgent.IsChecked)
                    Run(FindFile("agent"), "/quiet");


            if (IsAgentInstalled())
                if ((bool)checkBoxProbe.IsChecked)
                    Run(FindFile("probe"), "/quiet");

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
            foreach (string file in Directory.GetFiles(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString()))
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

        public bool IsNetInstalled()
        {
            RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            string[] version_names = installed_versions.GetSubKeyNames();
            double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
            if (Framework > 3.5)
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

        public bool IsProbeInstalled()
        {
            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName.Contains("probe"))
                    return true;
            return false;
        }

        private void labelAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            labelAccount.Visibility = Visibility.Hidden;
            textBoxAccount.Visibility = Visibility.Visible;
        }

        private void labelPassword_MouseEnter(object sender, MouseEventArgs e)
        {
            labelPassword.Visibility = Visibility.Hidden;
            textBoxPassword.Visibility = Visibility.Visible;
        }

        private void textBoxAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            labelAccount.Visibility = Visibility.Visible;
            textBoxAccount.Visibility = Visibility.Hidden;
        }

        private void textBoxPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            labelPassword.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Hidden;
        }
    }
}
