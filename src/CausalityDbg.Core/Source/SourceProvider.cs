// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
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

			var offsets = new int[size];
			var fromLines = new int[size];
			var toLines = new int[size];
			var fromColumns = new int[size];
			var toColumns = new int[size];
			var documents = new ISymUnmanagedDocument[size];

			method.GetSequencePoints(size, out size, offsets, documents, fromLines, fromColumns, toLines, toColumns);

			var index = Array.BinarySearch(offsets, ilOffset);

			if (index < 0)
			{
				index = Math.Max(0, ~index - 1);
			}

			if (fromLines[index] == HiddenLine) return null;

			return new SourceSection(documents[index].GetUrl(), fromLines[index], fromColumns[index], toLines[index], toColumns[index]);
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
