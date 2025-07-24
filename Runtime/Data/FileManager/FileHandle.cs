using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UniUtils.Extensions;

namespace UniUtils.Data
{
    /// <summary>
    /// Represents a file handle that provides methods for file operations such as reading, writing, and deleting files.
    /// </summary>
    /// <example>
    /// Example of using a FileHandle to read, write and delete text files:
    /// <code>
    /// FileHandle handle = new FileHandle("files/example.txt", EStorageLocation.Persistent);
    /// handle.WriteText("Hello, World!");
    /// Debug.Log(handle.ReadText()); // Outputs: Hello, World!
    /// handle.Delete();
    /// </code>
    /// </example>
    public class FileHandle
    {
        public virtual string FullPath { get; }
        public virtual EStorageLocation Location { get; }

        public virtual string FileName => Path.GetFileName(FullPath);
        public virtual string RelativePath => Path.GetRelativePath(FileManager.GetRootPath(Location), FullPath);
        public virtual bool Exists => File.Exists(FullPath);
        public string Extension => Path.GetExtension(FullPath);
        public virtual bool IsReadOnly => Exists && new FileInfo(FullPath).IsReadOnly;
        public virtual long FileSizeBytes() => Exists ? new FileInfo(FullPath).Length : 0;
        public virtual DateTime? GetLastModified() => Exists ? File.GetLastWriteTime(FullPath) : null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandle"/> class with the specified relative path and storage location.
        /// Ensures the directory for the file exists.
        /// </summary>
        /// <param name="relativePath">The relative path of the file.</param>
        /// <param name="location">The storage location. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        public FileHandle(string relativePath, EStorageLocation location = EStorageLocation.Persistent)
        {
            string storageLocation = FileManager.GetRootPath(location);
            Location = location;
            FullPath = Path.Combine(storageLocation, relativePath);

            string directory = Path.GetDirectoryName(FullPath);
            FileManager.EnsureDirectoryExists(directory, Location);
        }

        #region File Operations

        /// <summary>
        /// Copies the current file to a new location and returns a new <see cref="FileHandle"/> instance for the copied file.
        /// </summary>
        /// <param name="newRelativePath">The relative path for the new file location.</param>
        /// <param name="location">
        /// The storage location for the new file. If <c>null</c>, the current file's location is used.
        /// </param>
        /// <param name="canOverwrite">
        /// If <c>true</c>, allows overwriting an existing file at the new location;
        /// otherwise, throws an error if the target file already exists.
        /// </param>
        /// <returns>
        /// A new <see cref="FileHandle"/> instance representing the copied file, or <c>null</c> if the operation fails.
        /// </returns>
        /// <example>
        /// <code>
        /// FileHandle file = new FileHandle("data/source.txt");
        /// FileHandle copiedFile = file.Copy("data/destination.txt", canOverwrite: true);
        /// </code>
        /// </example>
        public virtual FileHandle Copy(
            string newRelativePath,
            EStorageLocation? location = null,
            bool canOverwrite = false)
        {
            location ??= Location;
            string newFullPath = Path.Combine(FileManager.GetRootPath(location.Value), newRelativePath);
            string? newDir = Path.GetDirectoryName(newFullPath);
            FileManager.EnsureDirectoryExists(newDir, location.Value);

            bool wasSuccessful = FileManager.TryIOAction(
                () => File.Copy(FullPath, newFullPath, canOverwrite),
                $"Failed to copy file to '{newFullPath}'",
                (_, err) => this.LogError(err)
            );

            return wasSuccessful ? new FileHandle(newRelativePath, location.Value) : null;
        }

        /// <summary>
        /// Moves the current file to a new location and returns a new <see cref="FileHandle"/> instance for the moved file.
        /// </summary>
        /// <param name="newRelativePath">The relative path for the new file location.</param>
        /// <param name="canOverwrite">
        /// If <c>true</c>, allows overwriting an existing file at the new location;
        /// otherwise, throws an error if the target file already exists.
        /// </param>
        /// <returns>
        /// A new <see cref="FileHandle"/> instance representing the moved file, or <c>null</c> if the operation fails.
        /// </returns>
        /// <example>
        /// <code>
        /// FileHandle file = new FileHandle("data/oldname.txt");
        /// FileHandle movedFile = file.Move("data/newname.txt", canOverwrite: true);
        /// </code>
        /// </example>
        public virtual FileHandle Move(
            string newRelativePath,
            EStorageLocation? location = null,
            bool canOverwrite = false
        )
        {
            location ??= Location;
            string newFullPath = Path.Combine(FileManager.GetRootPath(location.Value), newRelativePath);

            if (File.Exists(newFullPath) && !canOverwrite)
            {
                this.LogError(
                    $"Cannot move file '{FullPath}' to '{newFullPath}' because the target file already exists.");
                return null;
            }

            string? newDir = Path.GetDirectoryName(newFullPath);
            FileManager.EnsureDirectoryExists(newDir, location.Value);

            bool wasSuccessful = FileManager.TryIOAction(
                () => File.Move(FullPath, newFullPath),
                $"Failed to move file to '{newFullPath}'",
                (_, err) => this.LogError(err)
            );

            return wasSuccessful ? new FileHandle(newRelativePath, Location) : null;
        }

        /// <summary>
        /// Renames the current file by moving it to a new name within the same directory.
        /// </summary>
        /// <param name="newName">The new name for the file.</param>
        /// <param name="canOverwrite">
        /// If <c>true</c>, allows overwriting an existing file with the new name;
        /// otherwise, throws an error if the target file already exists.
        /// </param>
        /// <returns>
        /// A new <see cref="FileHandle"/> instance representing the renamed file, or <c>null</c> if the operation fails.
        /// </returns>
        public virtual FileHandle Rename(string newName, bool canOverwrite = false)
        {
            string safeName = Path.GetFileName(newName);
            string dir = Path.GetDirectoryName(RelativePath) ?? string.Empty;
            string newRelativePath = Path.Combine(dir, safeName);
            return Move(newRelativePath, Location, canOverwrite);
        }

        /// <summary>
        /// Deletes the current file.
        /// </summary>
        /// <returns><c>true</c> if the file was successfully deleted; otherwise, <c>false</c>.</returns>
        public virtual bool Delete()
        {
            return FileManager.TryIOAction(
                () => File.Delete(FullPath), $"Failed to delete file '{FullPath}'",
                (_, err) => this.LogError(err)
            );
        }

        #endregion

        #region Read / Write

        /// <summary>
        /// Reads the content of the file as a string.
        /// </summary>
        /// <returns>The content of the file as a string, or <c>null</c> if the file does not exist.</returns>
        public virtual string ReadText()
        {
            string result = null;
            FileManager.TryIOAction(
                () => result = File.ReadAllText(FullPath),
                $"Failed to read text from '{FullPath}'",
                (_, err) => this.LogError(err)
            );

            return result;
        }

        /// <summary>
        /// Reads the content of the file as a byte array.
        /// </summary>
        /// <returns>The content of the file as a byte array, or <c>null</c> if the file does not exist.</returns>
        public virtual byte[] ReadBytes()
        {
            byte[] result = null;
            FileManager.TryIOAction(
                () => result = File.ReadAllBytes(FullPath),
                $"Failed to read bytes from '{FullPath}'",
                (_, err) => this.LogError(err)
            );

            return result;
        }

        /// <summary>
        /// Writes the specified text content to the file, overwriting any existing content.
        /// </summary>
        /// <param name="content">The text content to write to the file.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool WriteText(string content)
        {
            return FileManager.TryIOAction(
                () => File.WriteAllText(FullPath, content),
                $"Failed to write text to '{FullPath}'",
                (_, err) => this.LogError(err)
            );
        }

        /// <summary>
        /// Writes the specified text content to the file using the provided encoding, overwriting any existing content.
        /// </summary>
        /// <param name="content">The text content to write to the file.</param>
        /// <param name="encoding">The encoding to use when writing the text content.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool WriteText(string content, Encoding encoding)
        {
            return FileManager.TryIOAction(
                () => File.WriteAllText(FullPath, content, encoding),
                $"Failed to write text to '{FullPath}' with encoding",
                (_, err) => this.LogError(err)
            );
        }

        /// <summary>
        /// Appends the specified text content to the file.
        /// </summary>
        /// <param name="content">The text content to append to the file.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool AppendText(string content)
        {
            return FileManager.TryIOAction(
                () => File.AppendAllText(FullPath, content),
                $"Failed to append text to '{FullPath}'",
                (_, err) => this.LogError(err)
            );
        }

        /// <summary>
        /// Writes the specified byte array content to the file, overwriting any existing content.
        /// </summary>
        /// <param name="content">The byte array content to write to the file.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool WriteBytes(byte[] content)
        {
            return FileManager.TryIOAction(
                () => File.WriteAllBytes(FullPath, content),
                $"Failed to write bytes to '{FullPath}'",
                (_, err) => this.LogError(err)
            );
        }

        /// <summary>
        /// Opens and returns a read-only <see cref="FileStream"/> for this file.
        /// <para><b>Important:</b> You must close or dispose this stream when done.</para>
        /// <example>
        /// <code language="csharp">
        /// FileHandle h = new FileHandle("data.bin");
        /// using (FileStream fs = h.OpenReadStream())
        /// {
        ///     byte[] buffer = new byte[fs.Length];
        ///     fs.Read(buffer, 0, buffer.Length);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>A <see cref="FileStream"/> opened for read.</returns>
        public virtual FileStream OpenReadStream()
        {
            if (!Exists)
            {
                this.LogError($"Cannot open read stream for file '{FullPath}' because it does not exist.");
                return null;
            }

            return new FileStream(FullPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Opens and returns a write-only <see cref="FileStream"/> for this file.
        /// <para><b>Important:</b> You must close or dispose this stream when done.</para>
        /// <example>
        /// <code language="csharp">
        /// FileHandle h = new FileHandle("output.bin");
        /// using (FileStream fs = h.OpenWriteStream(overwrite: true))
        /// {
        ///     byte[] data = new byte[] { 1, 2, 3 };
        ///     fs.Write(data, 0, data.Length);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="overwrite">
        /// If true, the file will be truncated (recreated); if false, data will be appended.
        /// </param>
        /// <returns>A <see cref="FileStream"/> opened for write.</returns>
        public virtual FileStream OpenWriteStream(bool overwrite = true)
        {
            FileManager.EnsureDirectoryExists(Path.GetDirectoryName(FullPath), Location);

            FileMode mode = overwrite ? FileMode.Create : FileMode.Append;
            return new FileStream(FullPath, mode, FileAccess.Write);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Logs the content of the file by reading its text and outputting it to the log.
        /// </summary>
        public void Dump()
        {
            this.Log(ReadText());
        }

        /// <summary>
        /// Returns the full path of the file as a string.
        /// </summary>
        /// <returns>The full path of the file.</returns>
        public override string ToString() => FullPath;

        #endregion
    }

    /// <summary>
    /// Provides methods to compare two <see cref="FileHandle"/> objects for equality based on their full paths.
    /// </summary>
    public class FileHandleComparer : IEqualityComparer<FileHandle>
    {
        /// <summary>
        /// Determines whether the specified <see cref="FileHandle"/> objects are equal by comparing their full paths.
        /// </summary>
        /// <param name="x">The first <see cref="FileHandle"/> to compare.</param>
        /// <param name="y">The second <see cref="FileHandle"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the full paths of both <see cref="FileHandle"/> objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(FileHandle x, FileHandle y) =>
            string.Equals(x?.FullPath, y?.FullPath, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(FileHandle obj) =>
            obj.FullPath.ToLowerInvariant().GetHashCode();
    }
}