////////////////////////////////////////////////////////////////////////////////////
// NavigatorClient.xaml.cs - Demonstrates Directory Navigation in WPF App         //
// ver 2.0                                                                        //
// Source- Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2017
//Author - Amruta Joshi,
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package defines WPF application processing by the client.  The client
 * displays a local FileFolder view, and a remote FileFolder view.  It supports
 * navigating into subdirectories, both locally and in the remote Server.
 * 
 * It also supports viewing local files.
 * 
 * Maintenance History:
 * --------------------
 * ver 2.1 : 26 Oct 2017
 * - relatively minor modifications to the Comm channel used to send messages
 *   between NavigatorClient and NavigatorServer
 * ver 2.0 : 24 Oct 2017
 * - added remote processing - Up functionality not yet implemented
 *   - defined NavigatorServer
 *   - added the CsCommMessagePassing prototype
 * ver 1.0 : 22 Oct 2017
 * - first release
 */
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Threading;
using MessagePassingComm;

namespace Navigator
{
    public partial class MainWindow : Window
    {
        private IFileMgr fileMgr { get; set; } = null;  // note: Navigator just uses interface declarations
        Comm comm { get; set; } = null;
        Dictionary<string, Action<CommMessage>> messageDispatcher = new Dictionary<string, Action<CommMessage>>();
        Thread rcvThread = null;

        public MainWindow()
        {
            InitializeComponent();
            initializeEnvironment();
            Console.Title = "Navigator Client";
            fileMgr = FileMgrFactory.create(FileMgrType.Local); // uses Environment
            getTopFiles();
            comm = new Comm(ClientEnvironment.address, ClientEnvironment.port);
            initializeMessageDispatcher();
            rcvThread = new Thread(rcvThreadProc);
            rcvThread.Start();
        }
        //----< make Environment equivalent to ClientEnvironment >-------

        void initializeEnvironment()
        {
            Environment.root = ServerEnvironment.root;
            Environment.address = ServerEnvironment.address;
            Environment.port = ServerEnvironment.port;
            Environment.endPoint = ServerEnvironment.endPoint;
        }
        //----< define how to process each message command >-------------

        void initializeMessageDispatcher()
        {
            messageDispatcher["getTok"] = (CommMessage msg) =>
            {
                foreach (string file in msg.arguments)
                {
                    string fileRead = File.ReadAllText(file);
                    CodePopUp codepop = new CodePopUp();
                    codepop.codeView.Text = fileRead;
                    codepop.Show();
                }
            };
            messageDispatcher["getSemi"] = (CommMessage msg) =>
            {
                foreach (string file in msg.arguments)
                {
                    string fileRead = File.ReadAllText(file);
                    CodePopUp codepop = new CodePopUp();
                    codepop.codeView.Text = fileRead;
                    codepop.Show();
                }
            };
            messageDispatcher["getTypeTable"] = (CommMessage msg) =>
            {
                foreach (string file in msg.arguments)
                {
                    string fileRead = File.ReadAllText(file);
                    CodePopUp codepop = new CodePopUp();
                    codepop.codeView.Text = fileRead;
                    codepop.Show();
                }
            };
            messageDispatcher["getDependency"] = (CommMessage msg) =>
            {
                foreach (string file in msg.arguments)
                {
                    string fileRead = File.ReadAllText(file);
                    CodePopUp codepop = new CodePopUp();
                    codepop.codeView.Text = fileRead;
                    codepop.Show();
                }
            };
            messageDispatcher["getStrongComponent"] = (CommMessage msg) =>
            {
                foreach (string file in msg.arguments)
                {
                    string fileRead = File.ReadAllText(file);
                    CodePopUp codepop = new CodePopUp();
                    codepop.codeView.Text = fileRead;
                    codepop.Show();
                }
            };

        }
        //----< define processing for GUI's receive thread >-------------

        void rcvThreadProc()
        {
            Console.Write("\n  starting client's receive thread");
            while (true)
            {
                CommMessage msg = comm.getMessage();
                msg.show();
                if (msg.command == null)
                    continue;

                // pass the Dispatcher's action value to the main thread for execution

                Dispatcher.Invoke(messageDispatcher[msg.command], new object[] { msg });
            }
        }
        //----< shut down comm when the main window closes >-------------

        private void Window_Closed(object sender, EventArgs e)
        {
            comm.close();

            // The step below should not be nessary, but I've apparently caused a closing event to 
            // hang by manually renaming packages instead of getting Visual Studio to rename them.

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        //----< not currently being used >-------------------------------


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tokenizer_Click();
            SemiExpression_Click();
            TypeTable_Click();
            DependencyAnalysis_Click();
            StrongComponent_Click();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        //----< show files and dirs in root path >--------------------

        public void getTopFiles()
        {
            List<string> files = fileMgr.getFiles().ToList<string>();
            localFiles.Items.Clear();
            foreach (string file in files)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = file.ToString();
                checkbox.Checked += new RoutedEventHandler(checkFiles);
                checkbox.Unchecked += new RoutedEventHandler(UncheckFiles);
                localFiles.Items.Add(checkbox);
            }
            List<string> dirs = fileMgr.getDirs().ToList<string>();
            localDirs.Items.Clear();
            foreach (string dir in dirs)
            {
                localDirs.Items.Add(dir);
            }
        }

        private void UncheckFiles(object sender, RoutedEventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            DisplayFiles.Items.Remove(chb.Content);
        }

        private void checkFiles(object sender, RoutedEventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            DisplayFiles.Items.Add(chb.Content);
        }

        //----< move to directory root and display files and subdirs >---

        private void localTop_Click(object sender, RoutedEventArgs e)
        {
            fileMgr.currentPath = "";
            getTopFiles();
        }
        //----< show selected file in code popup window >----------------

        private void localFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string fileName = localFiles.SelectedValue as string;
            try
            {
                string path = System.IO.Path.Combine(ClientEnvironment.root, fileName);
                string contents = File.ReadAllText(path);
                CodePopUp popup = new CodePopUp();
                popup.codeView.Text = contents;
                popup.Show();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }
        //----< move to parent directory and show files and subdirs >----

        private void localUp_Click(object sender, RoutedEventArgs e)
        {
            if (fileMgr.currentPath == "")
                return;
            fileMgr.currentPath = fileMgr.pathStack.Peek();
            fileMgr.pathStack.Pop();
            getTopFiles();
        }
        //----< move into subdir and show files and subdirs >------------

        private void localDirs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string dirName = localDirs.SelectedValue as string;
            fileMgr.pathStack.Push(fileMgr.currentPath);
            fileMgr.currentPath = dirName;
            getTopFiles();
        }
        //----< move to root of remote directories >---------------------
        /*
         * - sends a message to server to get files from root
         * - recv thread will create an Action<CommMessage> for the UI thread
         *   to invoke to load the remoteFiles listbox
         */
        private void RemoteTop_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Jim Fawcett";
            msg1.command = "getTopFiles";
            msg1.arguments.Add("");
            comm.postMessage(msg1);
            CommMessage msg2 = msg1.clone();
            msg2.command = "getTopDirs";
            comm.postMessage(msg2);
        }
        private void Tokenizer_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getTok";
            foreach (string allFiles in DisplayFiles.Items)
            {
                string path = ServerEnvironment.root + allFiles;
                string getPath = System.IO.Path.GetFullPath(path);
                msg1.arguments.Add(getPath);
            }
            comm.postMessage(msg1);
        }
        private void SemiExpression_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getSemi";
            foreach (string allFiles in DisplayFiles.Items)
            {
                string path = ServerEnvironment.root + allFiles;
                string getPath = System.IO.Path.GetFullPath(path);
                msg1.arguments.Add(getPath);
            }
            comm.postMessage(msg1);
        }
        private void TypeTable_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getTypeTable";
            foreach (string allFiles in DisplayFiles.Items)
            {
                string path = ServerEnvironment.root + allFiles;
                string getPath = System.IO.Path.GetFullPath(path);
                msg1.arguments.Add(getPath);
            }
            comm.postMessage(msg1);
        }
        private void DependencyAnalysis_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getDependency";
            foreach (string allFiles in DisplayFiles.Items)
            {
                string path = ServerEnvironment.root + allFiles;
                string getPath = System.IO.Path.GetFullPath(path);
                msg1.arguments.Add(getPath);
            }
            comm.postMessage(msg1);
        }
        private void StrongComponent_Click(object sender, RoutedEventArgs e)
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getStrongComponent";
            foreach (string allFiles in DisplayFiles.Items)
            {
                string path = ServerEnvironment.root + allFiles;
                string getPath = System.IO.Path.GetFullPath(path);
                msg1.arguments.Add(getPath);
            }
            comm.postMessage(msg1);
        }



        private void Tokenizer_Click()
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getTok";
            string pathofFiles = System.IO.Path.GetFullPath(ServerEnvironment.root + "ProjectFiles");
            string[] resultofiles = new string[] { };
            resultofiles = Directory.GetFiles(pathofFiles);
            foreach (string allFiles in resultofiles)
            {

                msg1.arguments.Add(allFiles);
            }
            comm.postMessage(msg1);
        }
        private void SemiExpression_Click()
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getSemi";
            string pathofFiles = System.IO.Path.GetFullPath(ServerEnvironment.root + "ProjectFiles");
            string[] resultofiles = new string[] { };
            resultofiles = Directory.GetFiles(pathofFiles);
            foreach (string allFiles in resultofiles)
            {

                msg1.arguments.Add(allFiles);
            }
            comm.postMessage(msg1);
        }
        private void TypeTable_Click()
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getTypeTable";
            string pathofFiles = System.IO.Path.GetFullPath(ServerEnvironment.root + "ProjectFiles");
            string[] resultofiles = new string[] { };
            resultofiles = Directory.GetFiles(pathofFiles);
            foreach (string allFiles in resultofiles)
            {

                msg1.arguments.Add(allFiles);
            }
            comm.postMessage(msg1);
        }
        private void DependencyAnalysis_Click()
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getDependency";
            string pathofFiles = System.IO.Path.GetFullPath(ServerEnvironment.root + "ProjectFiles");
            string[] resultofiles = new string[] { };
            resultofiles = Directory.GetFiles(pathofFiles);
            foreach (string allFiles in resultofiles)
            {

                msg1.arguments.Add(allFiles);
            }
            comm.postMessage(msg1);
        }
        private void StrongComponent_Click()
        {
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Amruta Joshi";
            msg1.command = "getStrongComponent";
            string pathofFiles = System.IO.Path.GetFullPath(ServerEnvironment.root + "ProjectFiles");
            string[] resultofiles = new string[] { };
            resultofiles = Directory.GetFiles(pathofFiles);
            foreach (string allFiles in resultofiles)
            {

                msg1.arguments.Add(allFiles);
            }
            comm.postMessage(msg1);
        }




    }
}
