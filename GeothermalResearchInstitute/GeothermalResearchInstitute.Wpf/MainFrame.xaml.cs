using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : Page
    {
        private Frame mainFrame;

        public MainFrame()
        {
            this.InitializeComponent();
        }

        public MainFrame(Frame mainFrame)
        {
            this.mainFrame = mainFrame;
        }

        private UserIdentity User
        {
            get { return (UserIdentity)Application.Current.FindResource("User"); }
        }

        //private void BtnLogin_Click(object sender, RoutedEventArgs e)
        //{
        //    LoginWindow loginWindow = new LoginWindow
        //    {
        //        Owner = this
        //    };

        //    if (loginWindow.ShowDialog() == true)
        //    {
        //        this.User.Username = "刘冰";
        //        this.User.Role = "管理员";
        //    }
        //}

        


    }
}
