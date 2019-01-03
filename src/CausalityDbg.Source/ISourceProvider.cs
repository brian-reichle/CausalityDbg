// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Metadata;

namespace CausalityDbg.Source
{
	public interface ISourceProvider
	{
		SourceSection GetSourceSection(MetaFunction metaFunction, int ilOffset);
	}
}
