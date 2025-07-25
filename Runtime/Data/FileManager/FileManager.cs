using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UniUtils.Extensions;

namespace UniUtils.Data
{
    /// <summary>
    /// Provides utility methods for managing files and directories within different storage locations.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Retrieves a list of files from a specified directory within a given storage location.
        /// </summary>
        /// <param name="relativePath">The relative path to the directory from which to retrieve files.</param>
        /// <param name="location">The storage location to use for resolving the directory path. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <returns>A list of <see cref="FileHandle"/> objects representing the files in the specified directory. If the directory does not exist, an empty list is returned.</returns>
        /// <example>
        /// <code>
        /// List&lt;FileHandle&gt; files = FileManager.GetFilesInDirectory("Logs");
        /// foreach (FileHandle file in files)
        ///     file.Delete();
        /// </code>
        /// </example>
        public static List<FileHandle> GetFilesInDirectory(
            string relativePath,
            EStorageLocation location = EStorageLocation.Persistent
        )
        {
            string rootPath = GetRootPath(location);
            string fullDirPath = Path.Combine(rootPath, relativePath);

            if (!Directory.Exists(fullDirPath))
                return new List<FileHandle>();

            string[] files = Directory.GetFiles(fullDirPath);
            return files
                .Select(fullFilePath =>
                {
                    string relPath = Path.GetRelativePath(rootPath, fullFilePath);
                    return new FileHandle(relPath, location);
                })
                .ToList();
        }

        /// <summary>
        /// Retrieves a list of subdirectories from a specified directory within a given storage location.
        /// </summary>
        /// <param name="relativePath">The relative path to the directory from which to retrieve subdirectories.</param>
        /// <param name="location">The storage location to use for resolving the directory path. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <returns>A list of relative paths representing the subdirectories in the specified directory. If the directory does not exist, an empty list is returned.</returns>
        /// <example>
        /// <code>
        /// List&lt;string&gt; subdirs = FileManager.GetSubdirectories("Projects");
        /// foreach (string dir in subdirs)
        ///     Debug.Log(dir);
        /// </code>
        /// </example>
        public static List<string> GetSubdirectories(
            string relativePath,
            EStorageLocation location = EStorageLocation.Persistent
        )
        {
            string fullDirPath = Path.Combine(GetRootPath(location), relativePath);

            if (!Directory.Exists(fullDirPath))
                return new List<string>();

            return Directory.GetDirectories(fullDirPath)
                .Select(dir => Path.GetRelativePath(GetRootPath(location), dir))
                .ToList();
        }

        /// <summary>
        /// Copies the contents of a source directory to a target directory within a specified storage location.
        /// </summary>
        /// <param name="sourceRelativePath">The relative path of the source directory.</param>
        /// <param name="targetRelativePath">The relative path of the target directory.</param>
        /// <param name="location">The storage location to use. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <returns><c>true</c> if the copy was successful; otherwise throws an exception.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the source directory does not exist.</exception>
        /// <exception cref="System.IO.IOException">Thrown if copying fails due to IO errors.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the operation lacks necessary permissions.</exception>
        /// <example>
        /// <code>
        /// FileManager.CopyDirectory("Configs", "Backup/Configs");
        /// </code>
        /// </example>
        public static bool CopyDirectory(
            string sourceRelativePath,
            string targetRelativePath,
            EStorageLocation location = EStorageLocation.Persistent)
        {
            string root = GetRootPath(location);
            string sourcePath = Path.Combine(root, sourceRelativePath);
            string targetPath = Path.Combine(root, targetRelativePath);

            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException(
                    $"Source directory '{sourceRelativePath}' does not exist in location '{location}'.");
            }

            Directory.CreateDirectory(targetPath);

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string destFile = Path.Combine(targetPath, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (string dir in Directory.GetDirectories(sourcePath))
            {
                CopyDirectory(Path.Combine(sourceRelativePath, Path.GetFileName(dir)),
                    Path.Combine(targetRelativePath, Path.GetFileName(dir)), location);
            }

            return true;
        }

        /// <summary>
        /// Moves a directory from one path to another within a given storage location.
        /// </summary>
        /// <param name="fromRelativePath">The relative path of the source directory.</param>
        /// <param name="toRelativePath">The relative path of the target directory.</param>
        /// <param name="location">The storage location. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <returns><c>true</c> if the move succeeds; otherwise throws an exception.</returns>
        /// <exception cref="System.IO.IOException">Thrown if the target directory already exists or if the move fails.</exception>
        /// <example>
        /// <code>
        /// FileManager.MoveDirectory("TempData", "Archived/TempData");
        /// </code>
        /// </example>
        public static bool MoveDirectory(
            string fromRelativePath,
            string toRelativePath,
            EStorageLocation location = EStorageLocation.Persistent)
        {
            string root = GetRootPath(location);
            string from = Path.Combine(root, fromRelativePath);
            string to = Path.Combine(root, toRelativePath);

            if (Directory.Exists(to))
            {
                throw new IOException(
                    $"Target directory '{toRelativePath}' already exists in location '{location}'.");
            }

            return TryIOAction(() => Directory.Move(from, to),
                $"Failed to move directory from '{fromRelativePath}' to '{toRelativePath}'",
                (exception, _) => throw exception
            );
        }

        /// <summary>
        /// Deletes all contents of a directory without deleting the directory itself.
        /// </summary>
        /// <param name="relativePath">The relative path of the directory to clear.</param>
        /// <param name="location">The storage location. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <exception cref="System.IO.IOException">Thrown if file or directory deletion fails.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the operation lacks necessary permissions.</exception>
        /// <example>
        /// <code>
        /// FileManager.ClearDirectory("Cache");
        /// </code>
        /// </example>
        public static void ClearDirectory(
            string relativePath,
            EStorageLocation location = EStorageLocation.Persistent
        )
        {
            string fullPath = Path.Combine(GetRootPath(location), relativePath);

            if (!Directory.Exists(fullPath))
                return;

            foreach (string file in Directory.GetFiles(fullPath))
                TryIOAction(
                    () => File.Delete(file),
                    $"Failed to delete file '{file}'",
                    (exception, _) => throw exception
                );

            foreach (string dir in Directory.GetDirectories(fullPath))
                TryIOAction(
                    () => Directory.Delete(dir, true),
                    $"Failed to delete directory '{dir}'",
                    (exception, _) => throw exception
                );
        }

        /// <summary>
        /// Deletes a specified directory within a given storage location.
        /// </summary>
        /// <param name="relativePath">The relative path of the directory to delete.</param>
        /// <param name="location">The storage location to use for resolving the directory path. Defaults to <see cref="EStorageLocation.Persistent"/>.</param>
        /// <returns><c>true</c> if the directory is deleted successfully; <c>false</c> if an error occurs.</returns>
        /// <exception cref="System.IO.IOException">Thrown if file or directory deletion fails.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the operation lacks necessary permissions.</exception>
        public static bool DeleteDirectory(
            string relativePath,
            EStorageLocation location = EStorageLocation.Persistent
        )
        {
            string rootPath = GetRootPath(location);
            string fullDirPath = Path.Combine(rootPath, relativePath);

            return TryIOAction(
                () => Directory.Delete(fullDirPath, true),
                $"Failed to delete directory '{relativePath}'",
                (exception, _) => throw exception
            );
        }

        #region Helpers

        /// <summary>
        /// Retrieves the root path for the specified storage location.
        /// </summary>
        /// <param name="location">The storage location for which to retrieve the root path.</param>
        /// <returns>The root path of the specified storage location.</returns>
        public static string GetRootPath(EStorageLocation location)
        {
            return location switch
            {
                EStorageLocation.Persistent => Application.persistentDataPath,
                EStorageLocation.DataPath => Application.dataPath,
                EStorageLocation.Temporary => Application.temporaryCachePath,
                EStorageLocation.StreamingAssets => Application.streamingAssetsPath,
                _ => Application.persistentDataPath,
            };
        }

        /// <summary>
        /// Ensures that a directory exists at the specified relative path within a given storage location.
        /// If the directory does not exist, it is created.
        /// </summary>
        /// <param name="relativePath">The relative path of the directory to check or create.</param>
        /// <param name="location">
        /// The storage location to use for resolving the directory path. Defaults to <see cref="EStorageLocation.Persistent"/>.
        /// </param>
        public static void EnsureDirectoryExists(
            string relativePath,
            EStorageLocation location = EStorageLocation.Persistent
        )
        {
            string fullDirPath = Path.Combine(GetRootPath(location), relativePath);
            if (!Directory.Exists(fullDirPath))
                Directory.CreateDirectory(fullDirPath);
        }

        /// <summary>
        /// Executes an I/O action and handles any exceptions that occur during its execution.
        /// </summary>
        /// <param name="action">The I/O action to execute.</param>
        /// <param name="errorContext">
        /// A descriptive context for the error, used to provide additional information in the error message.
        /// </param>
        /// <param name="onError">
        /// An optional callback to handle exceptions. The callback receives the exception and the formatted error message.
        /// </param>
        /// <returns>
        /// <c>true</c> if the action executes successfully; <c>false</c> if an exception occurs.
        /// </returns>
        public static bool TryIOAction(Action action, string errorContext, Action<Exception, string> onError = null)
        {
            try
            {
                action();
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                onError?.Invoke(ex, $"{errorContext} (Access denied): {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                onError?.Invoke(ex, $"{errorContext} (Path too long): {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                onError?.Invoke(ex, $"{errorContext} (Directory not found): {ex.Message}");
            }
            catch (IOException ex)
            {
                onError?.Invoke(ex, $"{errorContext} (IO error): {ex.Message}");
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex, $"{errorContext} (Unexpected error): {ex.Message}");
            }

            return false;
        }

        #endregion
    }
}