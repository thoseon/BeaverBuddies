using System;
using System.IO;

namespace Timberborn.MapRepositorySystem;

public readonly struct MapFileReference : IEquatable<MapFileReference>
{
	public string Name { get; }

	public string Path { get; }

	public bool Resource { get; }

	public bool UserFolder { get; }

	private MapFileReference(string name, string path, bool resource, bool userFolder)
	{
		Name = name;
		Path = path;
		Resource = resource;
		UserFolder = userFolder;
	}

	public static MapFileReference FromResource(string name)
	{
		return new MapFileReference(name, string.Empty, resource: true, userFolder: false);
	}

	public static MapFileReference FromUserFolder(string name)
	{
		return new MapFileReference(name, string.Empty, resource: false, userFolder: true);
	}

	public static MapFileReference FromDisk(string path)
	{
		return new MapFileReference(System.IO.Path.GetFileNameWithoutExtension(path), path, resource: false, userFolder: false);
	}

	public bool Equals(MapFileReference other)
	{
		if (Name == other.Name && Path == other.Path && Resource == other.Resource)
		{
			return UserFolder == other.UserFolder;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is MapFileReference other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((Name != null) ? Name.GetHashCode() : 0) * 397) ^ ((Path != null) ? Path.GetHashCode() : 0)) * 397) ^ Resource.GetHashCode()) * 397) ^ UserFolder.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}: {1}, {2}: {3}, {4}: {5}", "Name", Name, "Path", Path, "Resource", Resource);
	}

	public static bool operator ==(MapFileReference left, MapFileReference right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MapFileReference left, MapFileReference right)
	{
		return !left.Equals(right);
	}
}
