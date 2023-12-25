// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        using Render;
        using Utilities;

        public sealed class ProceduralTerrainGenerationBufferModel : IProceduralTerrainGenerationBufferProxy
        {
            /// <summary>
            /// Length 256 (1 * 256 * 4 = 1024 Byte)
            /// </summary>
            public uint[] CaseToNumPolys { get; set; }
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144 Byte)
            /// </summary>
            public Vector3[] EdgeStart { get; set; }
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144 Byte)
            /// </summary>
            public Vector3[] EdgeDir { get; set; }
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144 Byte)
            /// </summary>
            public Vector3[] EdgeEnd { get; set; }
            /// <summary>
            /// Length 12 (1 * 12 * 4 = 48 Byte)
            /// </summary>
            public uint[] EdgeAxis { get; set; }
            /// <summary>
            /// Length 1280 (4 * 1280 * 4 = 20480 Byte)
            /// </summary>
            public Vector4[] TriTable { get; set; }

            public const int LengthCaseToNumPolys = 256;
            public const int LengthEdgeStart = 12;
            public const int LengthEdgeDir = 12;
            public const int LengthEdgeEnd = 12;
            public const int LengthEdgeAxis = 12;
            public const int LengthTriTable = 1280;
            /// <summary>
            /// 1504 Byte
            /// </summary>
            public const int SizeInBytesCaseToNumPolys = (LengthCaseToNumPolys * 4)
                + (LengthEdgeStart * 3 * 4)
                + (LengthEdgeDir * 3 * 4)
                + (LengthEdgeEnd * 3 * 4)
                + (LengthEdgeAxis * 4);
            /// <summary>
            /// 20480 Byte
            /// </summary>
            public const int SizeInBytesEdgeConnectList = (LengthTriTable * 4 * 4);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadToBuffer(IBufferProxy buffer, DeviceContextProxy context)
            {
                ValidateProceduralTerrainGenerationData();

                var dataBox = context.MapSubresource(buffer.Buffer, 0, MapMode.WriteDiscard, MapFlags.None);
                if (!dataBox.IsEmpty)
                {
                    if (buffer.StructureSize == SizeInBytesCaseToNumPolys)
                    {
                        var ptr = UnsafeHelper.Write(dataBox.DataPointer, CaseToNumPolys, 0, CaseToNumPolys.Length);
                        ptr = UnsafeHelper.Write(ptr, EdgeStart, 0, EdgeStart.Length);
                        ptr = UnsafeHelper.Write(ptr, EdgeDir, 0, EdgeDir.Length);
                        ptr = UnsafeHelper.Write(ptr, EdgeEnd, 0, EdgeEnd.Length);
                        UnsafeHelper.Write(ptr, EdgeAxis, 0, EdgeAxis.Length);
                    }
                    else if (buffer.StructureSize == SizeInBytesEdgeConnectList)
                    {
                        UnsafeHelper.Write(dataBox.DataPointer, TriTable, 0, TriTable.Length);
                    }

                    context.UnmapSubresource(buffer.Buffer, 0);
                }
            }

            private void ValidateProceduralTerrainGenerationData()
            {
                if (CaseToNumPolys == null)
                {
                    throw new ArgumentNullException(nameof(CaseToNumPolys));
                }

                if (EdgeStart == null)
                {
                    throw new ArgumentNullException(nameof(EdgeStart));
                }

                if (EdgeDir == null)
                {
                    throw new ArgumentNullException(nameof(EdgeDir));
                }

                if (EdgeEnd == null)
                {
                    throw new ArgumentNullException(nameof(EdgeEnd));
                }

                if (EdgeAxis == null)
                {
                    throw new ArgumentNullException(nameof(EdgeAxis));
                }

                if (TriTable == null)
                {
                    throw new ArgumentNullException(nameof(TriTable));
                }

                if (CaseToNumPolys.Length != LengthCaseToNumPolys)
                {
                    throw new InvalidOperationException($"{nameof(CaseToNumPolys)} Length must be {LengthCaseToNumPolys}");
                }

                if (EdgeStart.Length != LengthEdgeStart)
                {
                    throw new InvalidOperationException($"{nameof(EdgeStart)} Length must be {LengthEdgeStart}");
                }

                if (EdgeDir.Length != LengthEdgeDir)
                {
                    throw new InvalidOperationException($"{nameof(EdgeDir)} Length must be {LengthEdgeDir}");
                }

                if (EdgeEnd.Length != LengthEdgeEnd)
                {
                    throw new InvalidOperationException($"{nameof(EdgeEnd)} Length must be {LengthEdgeEnd}");
                }

                if (EdgeAxis.Length != LengthEdgeAxis)
                {
                    throw new InvalidOperationException($"{nameof(EdgeAxis)} Length must be {LengthEdgeAxis}");
                }

                if (TriTable.Length != LengthTriTable)
                {
                    throw new InvalidOperationException($"{nameof(TriTable)} Length must be {LengthTriTable}");
                }
            }
        }
    }
}
