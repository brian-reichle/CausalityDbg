// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace CausalityDbg.Main
{
	[DebuggerDisplay("Count = {Count}")]
	sealed class TraceWrapper : IList<FrameWrapper>, IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public TraceWrapper(IEventScope eventScope)
		{
			_frames = FlattenedFrames(eventScope);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count => _frames.Length;
		public FrameWrapper this[int index] => _frames[index];

		public int IndexOf(FrameWrapper item)
		{
			if (item != null &&
				item.Trace == this &&
				item.Index < _frames.Length &&
				item == _frames[item.Index])
			{
				return item.Index;
			}

			return -1;
		}

		public bool Contains(FrameWrapper item) => IndexOf(item) >= 0;
		public void CopyTo(FrameWrapper[] array, int arrayIndex) => _frames.CopyTo(array, arrayIndex);
		public IEnumerator<FrameWrapper> GetEnumerator() => ((IEnumerable<FrameWrapper>)_frames).GetEnumerator();

		FrameWrapper[] FlattenedFrames(IEventScope eventScope)
		{
			var currentTrace = eventScope.Item.StackTrace;
			var nextEventScope = eventScope;
			var nextEventScopeDepth = nextEventScope.Item.StackTrace.TotalDepth;
			var builder = new List<FrameWrapper>();

			for (var r = currentTrace.TotalDepth; r > 0; r--)
			{
				var i = currentTrace.TotalDepth - r;

				while (i >= currentTrace.Frames.Length)
				{
					currentTrace = currentTrace.ContainingTrace;
					i = currentTrace.TotalDepth - r;
				}

				var frame = currentTrace.Frames[i];

				if (nextEventScopeDepth == r)
				{
					builder.Add(new FrameWrapper(this, builder.Count, frame, nextEventScope.Item.Category));

					do
					{
						nextEventScope = nextEventScope.Host;
						nextEventScopeDepth = nextEventScope == null ? -1 : nextEventScope.Item.StackTrace.TotalDepth;
					}
					while (nextEventScopeDepth == r);
				}
				else
				{
					builder.Add(new FrameWrapper(this, builder.Count, frame, null));
				}
			}

			return builder.ToArray();
		}

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add { }
			remove { }
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { }
			remove { }
		}

		void IList<FrameWrapper>.Insert(int index, FrameWrapper item) => throw new NotSupportedException();
		void IList<FrameWrapper>.RemoveAt(int index) => throw new NotImplementedException();

		FrameWrapper IList<FrameWrapper>.this[int index]
		{
			[DebuggerStepThrough]
			get => this[index];
			set => throw new NotSupportedException();
		}

		void ICollection<FrameWrapper>.Add(FrameWrapper item) => throw new NotSupportedException();
		void ICollection<FrameWrapper>.Clear() => throw new NotSupportedException();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<FrameWrapper>.IsReadOnly => true;
		bool ICollection<FrameWrapper>.Remove(FrameWrapper item) => throw new NotSupportedException();

		int IList.Add(object value) => throw new NotSupportedException();
		bool IList.Contains(object value) => value is FrameWrapper frame && Contains(frame);
		void IList.Clear() => throw new NotSupportedException();
		int IList.IndexOf(object value) => value is FrameWrapper frame ? IndexOf(frame) : -1;
		void IList.Insert(int index, object value) => throw new NotSupportedException();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize => true;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsReadOnly => true;
		void IList.Remove(object value) => throw new NotSupportedException();
		void IList.RemoveAt(int index) => throw new NotSupportedException();

		object IList.this[int index]
		{
			get => this[index];
			set => throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index) => CopyTo((FrameWrapper[])array, index);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized => false;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot => _frames.SyncRoot;

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly FrameWrapper[] _frames;
	}
}
