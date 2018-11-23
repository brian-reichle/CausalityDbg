// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("Count = {Count}")]
	sealed class ValueDictionary<T>
	{
		public ValueDictionary()
		{
			_list = new List<Pair>();
		}

		public void Set(ICorDebugReferenceValue key, T value)
		{
			var index = IndexOf(key);

			if (index >= 0)
			{
				var tmp = _list[index].Handle;
				_list[index] = new Pair(tmp, value);
			}
			else
			{
				var obj = (ICorDebugHeapValue2)key.Dereference();
				var handle = obj.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION);
				_list.Add(new Pair(handle, value));
			}
		}

		public void Remove(ICorDebugReferenceValue key)
		{
			var index = IndexOf(key);

			if (index >= 0)
			{
				Release(_list[index].Handle);
				_list.RemoveAt(index);
			}
		}

		public bool TryGetValue(ICorDebugReferenceValue key, out T value)
		{
			var index = IndexOf(key);

			if (index < 0)
			{
				value = default;
				return false;
			}
			else
			{
				value = _list[index].Value;
				return true;
			}
		}

		public void Purge()
		{
			var write = 0;

			for (var read = 0; read < _list.Count; read++)
			{
				var tmp = _list[read];

				if (!tmp.Handle.IsNullOrCollected())
				{
					_list[write++] = tmp;
				}
				else
				{
					Release(tmp.Handle);
				}
			}

			if (write < _list.Count)
			{
				_list.RemoveRange(write, _list.Count - write);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count => _list.Count;

		int IndexOf(ICorDebugReferenceValue key)
		{
			var targetAddress = key.GetValue();

			for (var i = 0; i < _list.Count; i++)
			{
				var handle = _list[i].Handle;

				if (!handle.IsNullOrCollected() && handle.GetValue() == targetAddress)
				{
					return i;
				}
			}

			return -1;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<Pair> _list;

		static void Release(ICorDebugHandleValue handle)
		{
			var hr = handle.Dispose();

			if (hr < 0 && hr != (int)HResults.CORDBG_E_OBJECT_NEUTERED)
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			Marshal.FinalReleaseComObject(handle);
		}

		readonly struct Pair
		{
			public Pair(ICorDebugHandleValue handle, T value)
			{
				Handle = handle;
				Value = value;
			}

			public ICorDebugHandleValue Handle { get; }
			public T Value { get; }
		}
	}
}
