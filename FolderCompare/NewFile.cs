using System;
using System.Collections.Generic;
using System.Text;

namespace FolderCompare
{
    public class NewFile
    {
        public string FileName { get; set; }
        public Double FileSize { get; set; }
    }

    public class RemovedFile : NewFile { }
}
