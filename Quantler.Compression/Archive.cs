#region License Header

/*
*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright 2018 Quantler B.V.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

#endregion License Header

using Quantler.Machine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Quantler.Compression
{
    /// <summary>
    /// Archiving logic
    /// </summary>
    public static class Archive
    {
        #region Public Methods

        /// <summary>
        /// Store 1 file into an existing archive (creates a new archive if not exists)
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool Append(string filepath, string entry, byte[] data, bool overwrite = false) =>
            WriteToArchive(filepath, new Dictionary<string, byte[]> {
                {entry, data}
            }, overwrite) > 0;

        /// <summary>
        /// Archive directory, preserving directory structure
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="destination"></param>
        /// <param name="archivename"></param>
        /// <param name="includebasedirectory"></param>
        /// <returns></returns>
        public static string Directory(string directory, string destination, string archivename, bool includebasedirectory = true)
        {
            string file = Instance.GetPath(destination, archivename);
            ZipFile.CreateFromDirectory(directory, file, CompressionLevel.Optimal, includebasedirectory);
            return file;
        }

        /// <summary>
        /// Extract all files from an existing archive into the target folder,
        /// if no entries are specified all entries are extracted
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="targetdirectory"></param>
        /// <param name="overwrite"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static string[] Extract(string filepath, string targetdirectory, bool overwrite = false, params string[] entries)
        {
            //Check if file is OK
            if (!IsValidArchive(filepath))
                throw new Exception("Invalid archive");

            //Set archive
            List<string> entriesknown = new List<string>();
            using (ZipArchive archive = ZipFile.Open(filepath, ZipArchiveMode.Read))
            {
                //Check which entries we need
                if (entries.Length == 0)
                    entries = GetEntries(filepath).Keys.ToArray();

                //Extract all files
                foreach (var entry in entries.Select(x => archive.GetEntry(x)).Where(x => x != null && x.Length > 0))
                {
                    var target = new FileInfo(Instance.GetPath(targetdirectory, entry.FullName));

                    if (!File.Exists(target.FullName))
                        System.IO.Directory.CreateDirectory(target.DirectoryName);
                    else if (File.Exists(target.FullName) && !overwrite)
                        continue;

                    entry.ExtractToFile(target.FullName);
                    entriesknown.Add(entry.FullName);
                }
            }

            //Return what we have extracted
            return entriesknown.ToArray();
        }

        /// <summary>
        /// Extracts a GZipStream.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="targetdirectory">The targetdirectory.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string[] ExtractGz(string filepath, string targetdirectory)
        {
            //get file
            FileInfo fileToDecompress = new FileInfo(filepath);

            //Check if file exists
            if(!fileToDecompress.Exists)
                throw new Exception($"File {filepath} does not exist");

            //Result
            List<string> toreturn = new List<string>();

            //Extract
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        toreturn.Add(fileToDecompress.Name);
                    }
                }
            }

            //Return result
            return toreturn.ToArray();
        }

        /// <summary>
        /// Get entries of an archive
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEntries(string filepath)
        {
            if (!IsValidArchive(filepath))
                return new Dictionary<string, string>();
            else
            {
                using (ZipArchive archive = ZipFile.OpenRead(filepath))
                {
                    return archive.Entries.ToDictionary(x => x.FullName, x => x.Name);
                }
            }
        }

        /// <summary>
        /// Returns true if this is a valid archive
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool IsValidArchive(string filepath) =>
            ArchiveExists(filepath);

        /// <summary>
        /// Read an entry into a memory stream
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static MemoryStream Read(string filepath, string entry) =>
            ReadAsync(filepath, entry).Result;

        /// <summary>
        /// Read an entry into a memory stream
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> ReadAsync(string filepath, string entry)
        {
            //Check if file is OK
            if (!IsValidArchive(filepath))
                throw new Exception($"Invalid archive {filepath}");

            //Get archive
            using (ZipArchive archive = ZipFile.Open(filepath, ZipArchiveMode.Read))
            {
                //Get entry
                var found = archive.GetEntry(entry);

                //Check if found
                if (found == null)
                    return null;
                else
                {
                    MemoryStream mstream = new MemoryStream();
                    await found.Open().CopyToAsync(mstream);
                    mstream.Position = 0;
                    return mstream;
                }
            }
        }

        /// <summary>
        /// Store a collection of files into a new archive
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="files"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static int Store(string filepath, Dictionary<string, byte[]> files, bool overwrite = false) =>
            WriteToArchive(filepath, files, overwrite);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns true if this archive exists
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private static bool ArchiveExists(string filepath) => File.Exists(filepath);

        /// <summary>
        /// Write files to archive
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="files"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        private static int WriteToArchive(string filepath, Dictionary<string, byte[]> files, bool overwrite = false)
        {
            //Check mode
            ZipArchiveMode mode = ZipArchiveMode.Create;
            Dictionary<string,string> currententries = new Dictionary<string, string>();

            if (ArchiveExists(filepath))
            {
                mode = ZipArchiveMode.Update;
                currententries = GetEntries(filepath);
            }
            int written = 0;

            //Set archive
            using (ZipArchive archive = ZipFile.Open(filepath, mode))
                foreach (var item in files)
                    //Check if we are allowed to overwrite the value
                    if ((currententries.ContainsKey(item.Key) && overwrite) ||
                            !currententries.ContainsKey(item.Key))
                    {
                        var entry = archive.CreateEntry(item.Key, CompressionLevel.Optimal);
                        using (var entrystream = entry.Open())
                        {
                            entrystream.Write(item.Value, 0, item.Value.Length);
                            written++;
                        }
                    }

            //Return amount of files written
            return written;
        }

        #endregion Private Methods
    }
}