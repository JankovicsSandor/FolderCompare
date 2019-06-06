using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FolderCompare
{
    public class CompareTask
    {
        public static CompareSetting OneMeasure(CompareSetting setting, CancellationToken token)
        {
            string oldBuildPath, newBuildPath;
            IEnumerable<FileInfo> oldBuild, newBuild;
            IEnumerable<FileAttribute> queryList1Only;
            IList<NewFile> newFile;
            IList<ModifiedFile> modFile;
            IList<RemovedFile> removeFile;
            string path = @"./outPut/";
            int threshold = 5;
            string fileName;

            fileName = $"DIFF_{setting.Type}_{setting.FromBuild}_{setting.ToBuild}";
            // Create two identical or different temporary folders   
            // on a local drive and change these file paths.  
            if (setting.Type == "FIN")
            {
                oldBuildPath = $@"";
                Console.WriteLine($"Investigating old build folder : {oldBuildPath}");

                newBuildPath = $@"";
                Console.WriteLine($"Investigating new build folder: {newBuildPath}");
            }
            else
            {
                oldBuildPath = $@"";
                Console.WriteLine($"Investigating old build folder : {oldBuildPath}");

                newBuildPath = $@"";
                Console.WriteLine($"Investigating new build folder: {newBuildPath}");
            }


            DirectoryInfo newBuildDir = new DirectoryInfo(newBuildPath);
            DirectoryInfo oldBuildDir = new DirectoryInfo(oldBuildPath);

            // Take a snapshot of the file system.  
            newBuild = newBuildDir.GetFiles("", SearchOption.AllDirectories);
            oldBuild = oldBuildDir.GetFiles("", SearchOption.AllDirectories);

            int index = oldBuildPath.IndexOf(setting.FromBuild) + setting.FromBuild.Length;
            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare(index);
            RemovedFileCompare removeCompare = new RemovedFileCompare(index);

            #region comment
            // This query determines whether the two folders contain  
            // identical file lists, based on the custom file comparer  
            // that is defined in the FileCompare class.  
            // The query executes immediately because it returns a bool.  
            /*  bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

              if (areIdentical == true)
              {
                  Console.WriteLine("the two folders are the same");
              }
              else
              {
                  Console.WriteLine("The two folders are not the same");
              }

              // Find the common files. It produces a sequence and doesn't   
              // execute until the foreach statement.  
              var queryCommonFiles = list1.Intersect(list2, myFileCompare);

              if (queryCommonFiles.Count() > 0)
              {
                  Console.WriteLine("The following files are in both folders:");
                  foreach (var v in queryCommonFiles)
                  {
                      Console.WriteLine(v.FullName); //shows which items end up in result list  
                  }
              }
              else
              {
                  Console.WriteLine("There are no common files in the two folders.");
              }*/

            #endregion

            // Find the set difference between the two folders.  
            // For this example we only check one way.  
            queryList1Only = (from file in newBuild
                              select file).Except(oldBuild, myFileCompare).Select(e => new FileAttribute()
                              {
                                  File = e,
                                  FileName = e.FullName,
                                  FileSize = e.Length / (1024d * 1024d)
                              }).OrderByDescending(e => e.FileSize);
            double newSize = 0;
            double modifiedSize = 0;
            double removedSize = 0;
            newFile = new List<NewFile>();
            modFile = new List<ModifiedFile>();
            removeFile = new List<RemovedFile>();
            double size;
            foreach (var v in queryList1Only)
            {
                string file = v.FileName;
                Console.WriteLine($"Processing: {file}");
                file = file.Replace(newBuildPath, oldBuildPath);
                try
                {
                    FileInfo info = new FileInfo(file);
                    size = info.Length / (1024d * 1024d);
                    modifiedSize += (v.FileSize - size);
                    if (modifiedSize > 0)
                    {
                        if ((Math.Abs(v.FileSize - size) / size) * 100 > threshold)
                        {
                            modFile.Add(new ModifiedFile() { FileName = v.FileName, OldFileSize = Math.Round(size, 3), NewFileSize = Math.Round(v.FileSize, 3) });
                        }
                    }

                }
                catch (FileNotFoundException e)
                {
                    newFile.Add(new NewFile() { FileName = v.FileName, FileSize = v.FileSize });
                    newSize += v.FileSize;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            queryList1Only = (from file in oldBuild
                              select file).Except(newBuild, removeCompare).Select(e => new FileAttribute()
                              {
                                  File = e,
                                  FileName = e.FullName,
                                  FileSize = e.Length / (1024d * 1024d)
                              }).OrderByDescending(e => e.FileSize);

            foreach (var v in queryList1Only)
            {
                string file = v.FileName;
                Console.WriteLine($"Processing: {file}");
                file = file.Replace(oldBuildPath, newBuildPath);
                try
                {
                    FileInfo info = new FileInfo(file);
                    size = info.Length;
                }
                catch (FileNotFoundException e)
                {
                    removedSize += v.FileSize;
                    removeFile.Add(new RemovedFile() { FileName = v.FileName, FileSize = v.FileSize });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (!File.Exists(path + fileName))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path + fileName + ".txt"))
                {
                    sw.WriteLine($"------------------------------------------------------------------------");
                    sw.WriteLine($"NEW FILES :{newFile.Count}  - {Math.Round(newSize, 0)} MB");
                    sw.WriteLine($"MODIFIED FILES :{modFile.Count}  - {Math.Round(modifiedSize, 0)} MB");
                    sw.WriteLine($"REMOVED FILES :{removeFile.Count}  - {Math.Round(removedSize, 0)} MB");
                    sw.WriteLine($"TOTAL CHANGE :{Math.Round(newSize, 0) + Math.Round(modifiedSize) - Math.Round(removedSize, 0)} MB");
                    sw.WriteLine($"------------------------------------------------------------------------");
                }

                // This text is always added, making the file longer over time
                // if it is not deleted.
                using (StreamWriter sw = File.AppendText(path + fileName + ".txt"))
                {
                    sw.WriteLine($"------------------------------------------------------------------------");
                    sw.WriteLine($"NEW FILES :{newFile.Count}  - {Math.Round(newSize, 0)} MB");
                    foreach (NewFile newF in newFile)
                    {
                        sw.WriteLine($"{newF.FileName} : {Math.Round(newF.FileSize, 0)} MB");
                    }
                    sw.WriteLine($"------------------------------------------------------------------------");
                    sw.WriteLine($"MODIFIED FILES :{modFile.Count}  - {Math.Round(modifiedSize, 0)} MB");
                    foreach (ModifiedFile modF in modFile)
                    {
                        sw.WriteLine(modF.ToString());
                    }
                    sw.WriteLine($"------------------------------------------------------------------------");
                    sw.WriteLine($"REMOVED FILES :{removeFile.Count}  - {Math.Round(removedSize, 0)} MB");
                    foreach (RemovedFile remove in removeFile)
                    {
                        sw.WriteLine($"{remove.FileName} : {Math.Round(remove.FileSize, 0)} MB");
                    }
                }
            }
            return setting;
        }
    }
}
