using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public  static class DirectoryExt
    {
        public static void Merge (string sourceDirName, string destDirName)
        {
            if(!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);


            string[] files = Directory.GetFiles(sourceDirName);
            string[] directories = Directory.GetDirectories(sourceDirName);

            foreach (string s in files)
            {
                var filePath = Path.Combine(destDirName, Path.GetFileName(s));
                if (File.Exists(filePath))
                {
                    System.IO.File.Copy(s, filePath, true);
                    System.IO.File.Delete(s);
                }
                else
                {
                    File.Move(s, filePath);
                }
            }
            foreach (string d in directories)
            {
                Merge(Path.Combine(sourceDirName, Path.GetFileName(d)), Path.Combine(destDirName, Path.GetFileName(d)));
            }
            Directory.Delete(sourceDirName, true);
        }
    }
}
