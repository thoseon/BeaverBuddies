using System;
using System.Collections.Generic;
using Timberborn.SingletonSystem;

namespace Timberborn.InputSystem;

public class KeywordService : ILoadableSingleton
{
	private class KeywordItem
	{
		private readonly string _keyword;

		private readonly Action _onMatch;

		private int _position;

		public string KeywordNotification { get; }

		public KeywordItem(string keywordNotification, string keyword, Action onMatch)
		{
			KeywordNotification = keywordNotification;
			_keyword = keyword;
			_onMatch = onMatch;
		}

		public bool IsNextCharMatching(string key)
		{
			if (key.Length == 1)
			{
				return _keyword[_position] == key[0];
			}
			return false;
		}

		public void Increase()
		{
			_position++;
		}

		public bool IsKeywordMatching()
		{
			return _position == _keyword.Length;
		}

		public void Match()
		{
			Reset();
			_onMatch();
		}

		public void Reset()
		{
			_position = 0;
		}
	}

	private readonly EventBus _eventBus;

	private readonly KeyboardListener _keyboardListener;

	private readonly List<KeywordItem> _keywordItems = new List<KeywordItem>();

	public KeywordService(EventBus eventBus, KeyboardListener keyboardListener)
	{
		_eventBus = eventBus;
		_keyboardListener = keyboardListener;
	}

	public void Load()
	{
		_keyboardListener.KeyPressed += OnKeyPressed;
	}

	public void AddKeyword(string keyword, string keywordNotification, Action onMatch)
	{
		_keywordItems.Add(new KeywordItem(keywordNotification, keyword.ToUpper(), onMatch));
	}

	private void OnKeyPressed(object sender, KeyPressedEvent e)
	{
		CheckKeywords(e.Key);
	}

	private void CheckKeywords(string key)
	{
		for (int i = 0; i < _keywordItems.Count; i++)
		{
			KeywordItem keywordItem = _keywordItems[i];
			CheckKeyword(key, keywordItem);
		}
	}

	private void CheckKeyword(string key, KeywordItem keywordItem)
	{
		if (!keywordItem.IsNextCharMatching(key))
		{
			keywordItem.Reset();
		}
		if (keywordItem.IsNextCharMatching(key))
		{
			keywordItem.Increase();
			if (keywordItem.IsKeywordMatching())
			{
				keywordItem.Match();
				_eventBus.Post(new KeywordMatchedEvent(keywordItem.KeywordNotification));
			}
		}
	}
}
