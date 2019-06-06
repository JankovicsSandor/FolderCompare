using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolderCompare
{
    // This implementation defines a very simple comparison  
    // between two FileInfo objects. It only compares the name  
    // of the files being compared and their length in bytes.  
    class FileCompare : IEqualityComparer<FileInfo>
    {
        private readonly int index;

        /// <summary>
        ///
        /// </summary>
        /// <param name="substringIndex">Start index to check the substring uniqueness.</param>
        public FileCompare(int substringIndex)
        {
            this.index = substringIndex;
        }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.FullName.Substring(index) == f2.FullName.Substring(index) &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the   
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }

    class RemovedFileCompare : IEqualityComparer<FileInfo>
    {
        private readonly int index;

        public RemovedFileCompare(int substringIndex)
        {
            this.index = substringIndex;
        }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.FullName.Substring(index) == f2.FullName.Substring(index));
        }

        // Return a hash that reflects the comparison criteria. According to the   
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(FileInfo fi)
        {
            string s = $"{fi.FullName.Substring(index)}";
            return s.GetHashCode();
        }
    }
}
