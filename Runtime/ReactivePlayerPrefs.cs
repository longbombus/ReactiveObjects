
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Scripting.LifecycleManagement;

namespace ReactiveObjects
{
	[AutoStaticsCleanup]
	public static partial class ReactivePlayerPrefs
	{
		internal static List<Action> ChangeListeners;

		public static ReactivePlayerPrefsStore<int> Ints => ReactivePlayerPrefs<int>.Store;
		public static ReactivePlayerPrefsStore<float> Floats => ReactivePlayerPrefs<float>.Store;
		public static ReactivePlayerPrefsStore<string> Strings => ReactivePlayerPrefs<string>.Store;
		public static ReactivePlayerPrefsStore<bool> Bools => ReactivePlayerPrefs<bool>.Store;

		public static void SetInt(string key, int value)
			=> ReactivePlayerPrefs<int>.Set(key, value);

		public static int GetInt(string key, int defaultValue)
			=> ReactivePlayerPrefs<int>.Get(key, defaultValue);

		public static void SetFloat(string key, float value)
			=> ReactivePlayerPrefs<float>.Set(key, value);

		public static float GetFloat(string key, float defaultValue)
			=> ReactivePlayerPrefs<float>.Get(key, defaultValue);

		public static void SetString(string key, string value)
			=> ReactivePlayerPrefs<string>.Set(key, value);

		public static string GetString(string key, string defaultValue)
			=> ReactivePlayerPrefs<string>.Get(key, defaultValue);

		public static void SetBool(string key, bool value)
			=> ReactivePlayerPrefs<bool>.Set(key, value);

		public static bool GetBool(string key, bool defaultValue)
			=> ReactivePlayerPrefs<bool>.Get(key, defaultValue);

		public static bool HasKey(string key)
			=> PlayerPrefs.HasKey(key);

		public static void DeleteKey(string key)
			=> throw new NotSupportedException("TODO: Implement DeleteKey in ReactivePlayerPrefs");

		public static void DeleteAll()
			=> throw new NotSupportedException("TODO: Implement DeleteAll in ReactivePlayerPrefs");
	}

	[AutoStaticsCleanup]
	public static partial class ReactivePlayerPrefs<T>
	{
		public static ReactivePlayerPrefsStore<T> Store = new();

		public static T Get(string key, T defaultValue)
			=> Store.Get(key, defaultValue);

		public static void Set(string key, T value)
			=> Store.Set(key, value);
	}

	[NoAutoStaticsCleanup]
	public class ReactivePlayerPrefsStore<T>
		: IReactiveKey<string>
		, IReactiveKey<string, T>
		, IReactivePair<string, T>
	{
		private static readonly Action<string, T> setter;
		private static readonly Func<string, T, T> getter;

		private Dictionary<string, List<Action>> keyListeners;
		private Dictionary<string, List<Action<T>>> keyValueListeners;
		private List<Action<string, T>> pairListeners;

		static ReactivePlayerPrefsStore()
		{
			if (typeof(T) == typeof(int))
			{
				setter = (Action<string, T>)(Delegate)(Action<string, int>)PlayerPrefs.SetInt;
				getter = (Func<string, T, T>)(Delegate)(Func<string, int, int>)PlayerPrefs.GetInt;
			}
			else if (typeof(T) == typeof(float))
			{
				setter = (Action<string, T>)(Delegate)(Action<string, float>)PlayerPrefs.SetFloat;
				getter = (Func<string, T, T>)(Delegate)(Func<string, float, float>)PlayerPrefs.GetFloat;
			}
			else if (typeof(T) == typeof(string))
			{
				setter = (Action<string, T>)(Delegate)(Action<string, string>)PlayerPrefs.SetString;
				getter = (Func<string, T, T>)(Delegate)(Func<string, string, string>)PlayerPrefs.GetString;
			}
			else if (typeof(T) == typeof(bool))
			{
				setter = (Action<string, T>)(Delegate)(Action<string, bool>)((key, value) => PlayerPrefs.SetInt(key, value ? 1 : 0));
				getter = (Func<string, T, T>)(Delegate)(Func<string, bool, bool>)((key, defaultValue) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0);
			}
			else
			{
				throw new NotSupportedException($"Type {typeof(T)} is not supported by ReactivePlayerPrefs");
			}
		}

		internal ReactivePlayerPrefsStore()
		{
		}

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref ReactivePlayerPrefs.ChangeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(ReactivePlayerPrefs.ChangeListeners, listener);

		public void Listen(string key, Action<T> listener)
			=> ListenersUtility.AddListener(ref keyValueListeners, key, listener);

		public void Listen(string key, Action listener)
			=> ListenersUtility.AddListener(ref keyListeners, key, listener);

		public void Forget(string key, Action listener)
			=> ListenersUtility.RemoveListener(keyListeners, key, listener);

		public void Forget(string key, Action<T> listener)
			=> ListenersUtility.RemoveListener(keyValueListeners, key, listener);

		public void Listen(Action<string, T> listener)
			=> ListenersUtility.AddListener(ref pairListeners, listener);

		public void Forget(Action<string, T> listener)
			=> ListenersUtility.RemoveListener(pairListeners, listener);

		public T Get(string key, T defaultValue)
			=> getter(key, defaultValue);

		public void Set(string key, T value)
		{
			setter(key, value);

			ListenersUtility.InvokeAllSafe(ReactivePlayerPrefs.ChangeListeners);
			ListenersUtility.InvokeAllSafe(keyListeners, key);
			ListenersUtility.InvokeAllSafe(keyValueListeners, key, value);
			ListenersUtility.InvokeAllSafe(pairListeners, key, value);
		}
	}

	public class ReactivePlayerPref<T> : IReadWriteReactive<T>
	{
		private readonly string key;
		private readonly T defaultValue;

		public ReactivePlayerPref(string key, T defaultValue)
		{
			this.key = key;
			this.defaultValue = defaultValue;
		}

		public T Value
		{
			get => ReactivePlayerPrefs<T>.Get(key, defaultValue);
			set => ReactivePlayerPrefs<T>.Set(key, value);
		}

		public void Listen(Action listener)
			=> ReactivePlayerPrefs<T>.Store.Listen(key, listener);

		public void Listen(Action<T> listener)
			=> ReactivePlayerPrefs<T>.Store.Listen(key, listener);

		public void Forget(Action listener)
			=> ReactivePlayerPrefs<T>.Store.Forget(key, listener);

		public void Forget(Action<T> listener)
			=> ReactivePlayerPrefs<T>.Store.Forget(key, listener);
	}
}
