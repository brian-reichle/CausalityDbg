// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.Core.SymbolStoreApi;
using CausalityDbg.Metadata;

namespace CausalityDbg.Core
{
	public sealed class SourceProvider : ISourceProvider, IDisposable
	{
		public SourceProvider()
		{
			_dispenser = (IMetaDataDispenser)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID.CLSID_CorMetaDataDispenser));
			_binder = (ISymUnmanagedBinder)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID.CLSID_CorSymBinder));
			_readers = new Dictionary<MetaModule, ISymUnmanagedReader>();
		}

		public SourceSection GetSourceSection(MetaFunction metaFunction, int ilOffset)
		{
			if (_isDisposed) throw new ObjectDisposedException(nameof(SourceProvider));
			if (metaFunction == null) throw new ArgumentNullException(nameof(metaFunction));

			var reader = GetReader(metaFunction.Module);
			if (reader == null) return null;

			var method = reader.GetMethod(metaFunction.Token);
			if (method == null) return null;

			var size = method.GetSequencePointCount();
			if (size == 0) return null;

			var offsets = ArrayPool<int>.Shared.Rent(size);
			var fromLines = ArrayPool<int>.Shared.Rent(size);
			var toLines = ArrayPool<int>.Shared.Rent(size);
			var fromColumns = ArrayPool<int>.Shared.Rent(size);
			var toColumns = ArrayPool<int>.Shared.Rent(size);
			var documents = ArrayPool<ISymUnmanagedDocument>.Shared.Rent(size);

			method.GetSequencePoints(size, out size, offsets, documents, fromLines, fromColumns, toLines, toColumns);

			var index = Array.BinarySearch(offsets, 0, size, ilOffset);

			if (index < 0)
			{
				index = Math.Max(0, ~index - 1);
			}

			var result = fromLines[index] == HiddenLine
				? null
				: new SourceSection(documents[index].GetUrl(), fromLines[index], fromColumns[index], toLines[index], toColumns[index]);

			ArrayPool<int>.Shared.Return(offsets);
			ArrayPool<int>.Shared.Return(fromLines);
			ArrayPool<int>.Shared.Return(toLines);
			ArrayPool<int>.Shared.Return(fromColumns);
			ArrayPool<int>.Shared.Return(toColumns);
			ArrayPool<ISymUnmanagedDocument>.Shared.Return(documents, true);

			return result;
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				foreach (var reader in _readers.Values)
				{
					if (reader != null)
					{
						((ISymUnmanagedDispose)reader).Destroy();
						Marshal.FinalReleaseComObject(reader);
					}
				}

				_readers.Clear();

				Marshal.FinalReleaseComObject(_binder);
			}
		}

		ISymUnmanagedReader GetReader(MetaModule module)
		{
			if (!_readers.TryGetValue(module, out var reader))
			{
				reader = GetReaderCore(module);
				_readers.Add(module, reader);
			}

			return reader;
		}

		ISymUnmanagedReader GetReaderCore(MetaModule module)
		{
			if ((module.Flags & MetaModuleFlags.IsInMemory) != 0)
			{
				return null;
			}

			var import = _dispenser.OpenScope(module.Name, CorOpenFlags.OfRead, typeof(IMetaDataImport).GUID);

			var hr = _binder.GetReaderForFile(import, module.Name, null, out var reader);

			if (hr == (int)HResults.E_PDB_NOT_FOUND)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return reader;
		}

		const int HiddenLine = 0x00FeeFee;

		bool _isDisposed;
		readonly IMetaDataDispenser _dispenser;
		readonly ISymUnmanagedBinder _binder;
		readonly Dictionary<MetaModule, ISymUnmanagedReader> _readers;
	}
}
