using System;
using System.Collections.Generic;
using System.Text;

namespace FolderCompare
{
    public class ModifiedFile
    {
        public string FileName { get; set; }
        public Double OldFileSize { get; set; }
        public Double NewFileSize { get; set; }
        public string Change
        {
            get
            {
                return $"CHANGE {Math.Round(NewFileSize - OldFileSize, 3)} MB ({Math.Round((Math.Abs(OldFileSize - NewFileSize) / OldFileSize) * 100, 2)} %)";
            }
        }

        public override string ToString()
        {
            return $"{FileName} : {Math.Round(OldFileSize, 0)} MB --> {Math.Round(NewFileSize, 0)} MB {Change}";
        }

    }
}
