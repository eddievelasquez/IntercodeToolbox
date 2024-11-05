// Module Name: Int32PrimitiveSourceGenerationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using Xunit.Abstractions;

public class BytePrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<byte>( output );
public class SBytePrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<sbyte>( output );
public class Int16PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<short>( output );
public class UInt16PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<ushort>( output );
public class Int32PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<int>( output );
public class UInt32PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<uint>( output );
public class Int64PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<long>( output );
public class UInt64PrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<ulong>( output );
public class SinglePrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<float>( output );
public class DoublePrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<double>( output );
public class DecimalPrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<double>( output );
public class DateTimePrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<DateTime>( output );
public class DateTimeOffsetPrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<DateTimeOffset>( output );
public class TimeSpanPrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<TimeSpan>( output );
public class GuidPrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<Guid>( output );
public class UriPrimitiveSourceGenerationTests( ITestOutputHelper output ): PrimitiveSourceGenerationTestBase<Uri>( output );
