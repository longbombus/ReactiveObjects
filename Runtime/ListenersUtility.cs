using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ReactiveObjects
{
	/// <summary> Stuff to work with listeners in implementations of Reactive objects. </summary>
	public static class ListenersUtility
	{
		#region Invokation

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeSafe(this Action listener)
		{
			try { listener(); }
			catch (Exception e) { Debug.LogException(e); }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeSafe<TValue>(this Action<TValue> listener, TValue value)
		{
			try { listener.Invoke(value); }
			catch (Exception e) { Debug.LogException(e); }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeSafe<TKey, TValue>(this Action<TKey, TValue> listener, TKey key, TValue value)
		{
			try { listener.Invoke(key, value); }
			catch (Exception e) { Debug.LogException(e); }
		}

		#endregion

		#region Change listeners

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddListener(ref List<Action> listeners, Action listener)
			=> (listeners ??= new List<Action>()).Add(listener);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveListener(List<Action> listeners, Action listener)
			=> listeners != null && listeners.Remove(listener);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeAllSafe(ICollection<Action> listeners)
		{
			if (listeners == null)
				return;

			var listenersCount = listeners.Count;
			var cachedListeners = ArrayPool<Action>.Shared.Rent(listenersCount);
			listeners.CopyTo(cachedListeners, listenersCount);

			for (var i = 0; i < listenersCount; ++i)
				cachedListeners[i].InvokeSafe();

			ArrayPool<Action>.Shared.Return(cachedListeners);
		}

		#endregion

		#region Value listeners

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddListener<TValue>(ref List<Action<TValue>> listeners, Action<TValue> listener)
			=> (listeners ??= new List<Action<TValue>>()).Add(listener);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveListener<TValue>(List<Action<TValue>> listeners, Action<TValue> item)
			=> listeners != null && listeners.Remove(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeAllSafe<TValue>(ICollection<Action<TValue>> listeners, in TValue value)
		{
			if (listeners == null)
				return;

			var listenersCount = listeners.Count;
			var cachedListeners = ArrayPool<Action<TValue>>.Shared.Rent(listenersCount);
			listeners.CopyTo(cachedListeners, listenersCount);

			for (var i = 0; i < listenersCount; ++i)
				cachedListeners[i].InvokeSafe(value);

			ArrayPool<Action<TValue>>.Shared.Return(cachedListeners);
		}

		#endregion

		#region Key-Value listeners

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddListener<TKey, TValue>(ref List<Action<TKey, TValue>> listeners, Action<TKey, TValue> listener)
			=> (listeners ??= new List<Action<TKey, TValue>>()).Add(listener);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveListener<TKey, TValue>(List<Action<TKey, TValue>> listeners, Action<TKey, TValue> item)
			=> listeners != null && listeners.Remove(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeAllSafe<TKey, TValue>(ICollection<Action<TKey, TValue>> listeners, in TKey key, in TValue value)
		{
			if (listeners == null)
				return;

			var listenersCount = listeners.Count;
			var cachedListeners = ArrayPool<Action<TKey, TValue>>.Shared.Rent(listenersCount);
			listeners.CopyTo(cachedListeners, listenersCount);

			for (var i = 0; i < listenersCount; ++i)
				cachedListeners[i].InvokeSafe(key, value);

			ArrayPool<Action<TKey, TValue>>.Shared.Return(cachedListeners);
		}

		#endregion

		#region Keyed change listeners

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddListener<TKey>(ref Dictionary<TKey, List<Action>> listeners, TKey key, Action listener)
		{
			listeners ??= new Dictionary<TKey, List<Action>>();

			if (!listeners.TryGetValue(key, out var keyListeners))
				listeners.Add(key, keyListeners = new List<Action>());

			keyListeners.Add(listener);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveListener<TKey>(Dictionary<TKey, List<Action>> listeners, TKey key, Action listener)
			=> listeners != null && listeners.TryGetValue(key, out var keyListeners) && keyListeners.Remove(listener);

		public static void InvokeAllSafe<TKey>(Dictionary<TKey, List<Action>> listeners, in TKey key)
		{
			if (listeners == null || !listeners.TryGetValue(key, out var keyListeners))
				return;

			InvokeAllSafe(keyListeners);
		}

		#endregion

		#region Keyed value listeners

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddListener<TKey, TValue>(ref Dictionary<TKey, List<Action<TValue>>> listeners, TKey key, Action<TValue> listener)
		{
			listeners ??= new Dictionary<TKey, List<Action<TValue>>>();

			if (!listeners.TryGetValue(key, out var keyListeners))
				listeners.Add(key, keyListeners = new List<Action<TValue>>());

			keyListeners.Add(listener);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveListener<TKey, TValue>(Dictionary<TKey, List<Action<TValue>>> listeners, TKey key, Action<TValue> listener)
			=> listeners != null && listeners.TryGetValue(key, out var keyListeners) && keyListeners.Remove(listener);

		public static void InvokeAllSafe<TKey, TValue>(Dictionary<TKey, List<Action<TValue>>> listeners, in TKey key, in TValue value)
		{
			if (listeners == null || !listeners.TryGetValue(key, out var keyListeners))
				return;

			InvokeAllSafe(keyListeners, value);
		}

		#endregion
	}
}