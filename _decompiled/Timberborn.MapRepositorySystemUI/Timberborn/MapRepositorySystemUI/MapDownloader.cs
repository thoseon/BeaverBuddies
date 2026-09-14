using System;
using Timberborn.Common;

namespace Timberborn.MapRepositorySystemUI;

public class MapDownloader
{
	private Action _downloadAction;

	public bool HasDownloader => _downloadAction != null;

	public void SetDownloadAction(Action action)
	{
		Asserts.FieldIsNull(this, _downloadAction, "_downloadAction");
		_downloadAction = action;
	}

	public void Download()
	{
		Asserts.FieldIsNotNull(this, _downloadAction, "_downloadAction");
		_downloadAction();
	}
}
