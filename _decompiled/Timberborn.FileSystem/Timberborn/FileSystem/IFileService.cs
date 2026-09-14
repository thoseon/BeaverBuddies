using System.IO;

namespace Timberborn.FileSystem;

public interface IFileService
{
	bool HasDocumentsPermissions { get; }

	bool FileExists(string fileName);

	Stream CreateFile(string fileName);

	Stream OpenFile(string fileName);

	void DeleteFile(string fileName);

	FileInfo GetFileInfo(string fileName);

	bool DirectoryExistsAndNotEmpty(string directoryName, string fileExtension);

	void CreateDirectory(string directoryName);

	void DeleteDirectory(string directoryName);

	string CombineIntoPath(string path1, string path2, string extension = "");

	void WriteTextToFile(string path, string text);

	void CopyFile(string sourceFileName, string destinationFileName);

	DirectoryCreationResult CreateDirectoryIfValid(string directoryPath);
}
