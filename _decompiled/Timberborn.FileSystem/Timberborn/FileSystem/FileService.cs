using System;
using System.IO;
using System.Linq;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.FileSystem;

public class FileService : IFileService
{
	public bool HasDocumentsPermissions { get; } = DocumentsPermissions.HasPermissions();

	public bool FileExists(string fileName)
	{
		return File.Exists(fileName);
	}

	public Stream CreateFile(string fileName)
	{
		try
		{
			return File.Create(fileName);
		}
		catch (ArgumentNullException)
		{
			throw;
		}
		catch (ArgumentException e)
		{
			throw ThrowAsIOException(e, fileName);
		}
		catch (NotSupportedException e2)
		{
			throw ThrowAsIOException(e2, fileName);
		}
	}

	public Stream OpenFile(string fileName)
	{
		try
		{
			return File.OpenRead(fileName);
		}
		catch (ArgumentNullException)
		{
			throw;
		}
		catch (ArgumentException e)
		{
			throw ThrowAsIOException(e, fileName);
		}
		catch (NotSupportedException e2)
		{
			throw ThrowAsIOException(e2, fileName);
		}
	}

	public void DeleteFile(string fileName)
	{
		try
		{
			File.SetAttributes(fileName, FileAttributes.Normal);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception while setting file attributes to normal: " + ex.Message + "/n" + ex.StackTrace);
		}
		try
		{
			File.Delete(fileName);
		}
		catch (Exception e)
		{
			throw ThrowAsIOException(e, fileName);
		}
	}

	public FileInfo GetFileInfo(string fileName)
	{
		try
		{
			return new FileInfo(fileName);
		}
		catch (ArgumentNullException)
		{
			throw;
		}
		catch (ArgumentException e)
		{
			throw ThrowAsIOException(e, fileName);
		}
	}

	public bool DirectoryExistsAndNotEmpty(string directoryName, string fileExtension)
	{
		if (Directory.Exists(directoryName))
		{
			return Directory.EnumerateFiles(directoryName).Any((string file) => fileExtension == null || Path.GetExtension(file) == fileExtension);
		}
		return false;
	}

	public DirectoryCreationResult CreateDirectoryIfValid(string directoryPath)
	{
		try
		{
			if (DirectoryExistsAndNotEmpty(directoryPath, null))
			{
				return DirectoryCreationResult.NameTaken;
			}
			CreateDirectory(directoryPath);
			return DirectoryCreationResult.OK;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to create directory " + directoryPath + " due to " + ex.Message);
			return DirectoryCreationResult.NameInvalid;
		}
	}

	public void CreateDirectory(string directoryName)
	{
		try
		{
			Directory.CreateDirectory(directoryName);
		}
		catch (Exception e)
		{
			throw ThrowAsIOException(e, directoryName);
		}
	}

	public void DeleteDirectory(string directoryName)
	{
		try
		{
			SetAttributesToNormal(new DirectoryInfo(directoryName));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception while setting attributes to normal: " + ex.Message + "/n" + ex.StackTrace);
		}
		try
		{
			Directory.Delete(directoryName, recursive: true);
		}
		catch (Exception e)
		{
			throw ThrowAsIOException(e, directoryName);
		}
	}

	public string CombineIntoPath(string path1, string path2, string extension = "")
	{
		try
		{
			return Path.Combine(path1, path2 + extension);
		}
		catch (ArgumentNullException)
		{
			throw;
		}
		catch (ArgumentException e)
		{
			throw ThrowAsIOException(e, path1 + "/" + path2 + extension);
		}
	}

	public void WriteTextToFile(string path, string text)
	{
		try
		{
			CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, text);
		}
		catch (Exception e)
		{
			throw ThrowAsIOException(e, path);
		}
	}

	public void CopyFile(string sourceFileName, string destinationFileName)
	{
		try
		{
			File.Copy(sourceFileName, destinationFileName);
		}
		catch (Exception e)
		{
			throw ThrowAsIOException(e, sourceFileName + " -> " + destinationFileName);
		}
	}

	private static Exception ThrowAsIOException(Exception e, string name)
	{
		return new IOException(e.Message + " Name: " + name, e);
	}

	private static void SetAttributesToNormal(DirectoryInfo directory)
	{
		directory.Attributes = FileAttributes.Normal;
		DirectoryInfo[] directories = directory.GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			SetAttributesToNormal(directories[i]);
		}
		FileInfo[] files = directory.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			files[i].Attributes = FileAttributes.Normal;
		}
	}
}
