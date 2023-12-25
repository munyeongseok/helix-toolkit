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
            /// Length 256 (1 * 256 * 4 = 1024)
            /// </summary>
            public uint[] CaseToNumPolys { get; set; } = new uint[256];
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144)
            /// </summary>
            public Vector3[] EdgeStart { get; set; } = new Vector3[12];
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144)
            /// </summary>
            public Vector3[] EdgeDir { get; set; } = new Vector3[12];
            /// <summary>
            /// Length 12 (3 * 12 * 4 = 144)
            /// </summary>
            public Vector3[] EdgeEnd { get; set; } = new Vector3[12];
            /// <summary>
            /// Length 12 (1 * 12 * 4 = 48)
            /// </summary>
            public uint[] EdgeAxis { get; set; } = new uint[12];
            /// <summary>
            /// Length 1280 (4 * 1280 * 4 = 20480)
            /// </summary>
            public Vector4[] TriTable { get; set; } = new Vector4[1280];

            public const int SizeInBytesCaseToNumPolys = 1024 + 144 + 144 + 144 + 48;
            public const int SizeInBytesEdgeConnectList = 20480;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadToBuffer(IBufferProxy buffer, DeviceContextProxy context)
            {
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
        }
    }
}
