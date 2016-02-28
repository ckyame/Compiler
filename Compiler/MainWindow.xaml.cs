using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Machine Machine = new Machine();

        public MainWindow()
        {
            InitializeComponent();
            Machine.OnStateProcessed += UpdateCompilerWatch;
            Machine.OnStateProcessedFailed += CatchCompilerError;
            Machine.OnParseSuccessful += CatchCompilerCompletion;
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            Machine.Parse(Code.Text);
            CPPCode.Text = Machine.CPP.ToString();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Code.Clear();
            CompilerWatch.Clear();
            CPPCode.Clear();
            Machine.CPP.Clear();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            using (FileStream fs = File.Create(@"C:\compiled.cpp"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(Machine.CPP.ToString());
                fs.Write(info, 0, info.Length);
            }
        }

        private void UpdateCompilerWatch(string state)
        {
            CompilerWatch.Text += string.Format("\n{0}", state);

        }

        private void CatchCompilerError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CatchCompilerCompletion()
        {
            MessageBox.Show("Code fully parsed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
