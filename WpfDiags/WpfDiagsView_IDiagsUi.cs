﻿using System.Collections.Generic;
using System.Windows.Controls;
using NongFormat;
using NongIssue;

namespace AppView
{
    // Implement IDiagsIUi here
    public partial class WpfDiagsView
    {
        private int totalLinesReported = 0;
        private string curDir = null, curFile = null;
        private bool dirShown = false, fileShown = false;

        public string BrowseFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { Filter="Media files (*.*)|*.*" };
            dlg.ShowDialog();
            return dlg.FileName;
        }

        public void ConsoleZoom (int delta)
        {
            var newZoom = unchecked (consoleBox.FontSize + delta);
            if (newZoom >= 4 && newZoom <= 60)
                consoleBox.FontSize = newZoom;
        }

        public IList<string> GetHeadings()
        {
            var result = new List<string>();
            var items = (ItemCollection) ((TabControl) infoTabs).Items;
            foreach (TabItem item in items)
                result.Add ((string) item.Header);
            return result;
        }

        public void FileProgress (string dirName, string fileName)
        {
            if (curDir != dirName)
            {
                curDir = dirName;
                dirShown = false;
                curFile = fileName;
                fileShown = false;
            }
            else if (curFile != fileName)
            {
                curFile = fileName;
                fileShown = false;
            }
        }

        public void SetText (string message)
        {
            consoleBox.Text = message;
            curDir = null; curFile = null;
            dirShown = false; fileShown = false;
            totalLinesReported = 0;
        }

        public void ShowLine (string message, Severity severity, Likeliness repairability)
        {
            if (! fileShown)
            {
                fileShown = true;

                if (totalLinesReported != 0)
                    if (presenterModel.Data.Scope < Granularity.Verbose)
                        consoleBox.AppendText ("\n\n---- ---- ----\n");
                    else if (! dirShown)
                        consoleBox.AppendText ("\n");

                if (! dirShown)
                {
                    dirShown = true;

                    if (! string.IsNullOrEmpty (presenterModel.Data.CurrentDirectory))
                    {
                        consoleBox.AppendText (presenterModel.Data.CurrentDirectory);
                        if (presenterModel.Data.CurrentDirectory[presenterModel.Data.CurrentDirectory.Length-1] != System.IO.Path.DirectorySeparatorChar)
                            consoleBox.AppendText (System.IO.Path.DirectorySeparatorChar.ToString());
                    }
                    consoleBox.AppendText ("\n");
                }

                consoleBox.AppendText (presenterModel.Data.CurrentFile);
            }

            consoleBox.AppendText ("\n");
            consoleBox.AppendText (message);
            ++totalLinesReported;
        }
    }
}
