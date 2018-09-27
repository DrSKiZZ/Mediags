﻿using System;
using System.IO;

namespace NongFormat
{
    // en.wikipedia.org/wiki/M3U
    public class M3uFormat : FilesContainer
    {
        public static string[] Names
        { get { return new string[] { "m3u" }; } }

        public override string[] ValidNames
        { get { return Names; } }

        public static Model CreateModel (Stream stream, byte[] hdr, string path)
        {
            if (path.ToLower().EndsWith(".m3u") ||
                  hdr.Length >= 7 && hdr[0]=='#' && hdr[1]=='E' && hdr[2]=='X'
                   && hdr[3]=='T' && hdr[4]=='M' && hdr[5]=='3' && hdr[6]=='U')
                return new Model (stream, hdr, path);
            return null;
        }


        public new class Model : FilesContainer.Model
        {
            public readonly M3uFormat Bind;

            public Model (Stream stream, byte[] header, string path) : base (path)
            {
                BaseBind = BindFiles = Bind = new M3uFormat (stream, path, FilesModel.Bind);
                Bind.Issues = IssueModel.Data;

                stream.Position = 0;
                TextReader tr = new StreamReader (stream, LogBuffer.cp1252);

                for (int line = 1; ; ++line)
                {
                    var lx = tr.ReadLine();
                    if (lx == null)
                        break;
                    lx = lx.TrimStart();
                    if (lx.Length > 0 && lx[0] != '#')
                        FilesModel.Add (lx);
                }
            }

            public Model (Stream stream, string m3uPath, LogEacFormat log) : base (m3uPath)
            {
                BaseBind = BindFiles = Bind = new M3uFormat (stream, m3uPath, log, FilesModel.Bind);
                Bind.Issues = IssueModel.Data;

                foreach (var track in log.Tracks.Items)
                    FilesModel.Add (track.Match.Name);
            }


            public void WriteFile()
            {
                var nl = LogBuffer.cp1252.GetBytes (Environment.NewLine);
                Bind.fbs.Position = 0;
                foreach (var line in Bind.Files.Items)
                {
                    var bb = LogBuffer.cp1252.GetBytes (line.Name);
                    Bind.fbs.Write (bb, 0, bb.Length);
                    Bind.fbs.Write (nl, 0, nl.Length);
                }
                Bind.fbs.SetLength (Bind.fbs.Position);
                ResetFile();
            }
        }


        private M3uFormat (Stream stream, string path, FileItem.Vector files) : base (stream, path, files)
        { }


        private M3uFormat (Stream stream, string m3uPath, LogEacFormat log, FileItem.Vector files) : base (stream, m3uPath, files)
        { }
    }
}
