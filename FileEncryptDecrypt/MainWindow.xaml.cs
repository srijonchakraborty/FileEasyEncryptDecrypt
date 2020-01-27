/*
|-------------------------------------------------------------------------------|
|	This code was written by Srijon Chakraborty								    |
|	Main source code link on https://github.com/srijonchakro			        |
|	All my source codes are available on http://srijon.softallybd.com           |
|	C# File Encrypt	Decrypt                                                     |
|	LinkedIn https://bd.linkedin.com/in/srijon-chakraborty-0ab7aba7				|
|-------------------------------------------------------------------------------|
*/
using Microsoft.Win32;
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

namespace FileEncryptDecrypt
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileEncryptDecryptService.ActionEncryptDone +=actionEncrypt_Done;
            FileEncryptDecryptService.ActionDecryptDone += actionDecryptD_Done;
            FileEncryptDecryptService.ActionEncryptException += actionEncryptException_Done;
            FileEncryptDecryptService.ActionDecryptException += ActionDecryptException_Done;
        }

        void ActionDecryptException_Done(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void actionEncryptException_Done(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        void actionEncrypt_Done(bool obj)
        {
            MessageBox.Show("Encrypt Done");
        }
        void actionDecryptD_Done(bool obj)
        {
            MessageBox.Show("Decrypt Done");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    textBox.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = string.Empty;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Files | *.sjnx";
                saveFileDialog.DefaultExt = "sjnx";
                if (saveFileDialog.ShowDialog() == true)
                {
                    textBoxSave.Text = saveFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            textBoxSave.Text = string.Empty;
        }

        private async void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(textBox.Text)|| string.IsNullOrEmpty(textBox.Text))
                {
                    MessageBox.Show("Select Source file.","Warnning",MessageBoxButton.OK);
                    return;
                }
                if (string.IsNullOrEmpty(textBoxSave.Text) || string.IsNullOrEmpty(textBoxSave.Text))
                {
                    MessageBox.Show("Select ouput file.", "Warnning", MessageBoxButton.OK);
                    return;
                }
                if (File.Exists(@"" + textBoxSave.Text))
                {
                    File.Delete(@"" + textBoxSave.Text);
                }
                await FileEncryptDecryptService.FileEncryptAsync(textBox.Text, textBoxSave.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private async void button1Decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(textBox.Text))
                {
                    MessageBox.Show("Select Source file.", "Warnning", MessageBoxButton.OK);
                    return;
                }
                if (string.IsNullOrEmpty(textBoxSave.Text) || string.IsNullOrEmpty(textBoxSave.Text))
                {
                    MessageBox.Show("Select ouput file.", "Warnning", MessageBoxButton.OK);
                    return;
                }
                if (File.Exists(@"" + textBoxSave.Text))
                {
                    File.Delete(@"" + textBoxSave.Text);
                }
                await FileEncryptDecryptService.FileDecryptAsync(textBox.Text, textBoxSave.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
