// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using SharpDX;
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
        using ShaderManager;
        using Shaders;
        using Utilities;

        public sealed class ProceduralTerrainGenerationShared : DisposeObject
        {
            private IBufferProxy bufferTerrainCaseToNumPolys;
            private IBufferProxy bufferTerrainEdgeConnectList;
            private readonly ProceduralTerrainGenerationBufferModel bufferModel;

            public ProceduralTerrainGenerationShared(IConstantBufferPool pool)
            {
                bufferTerrainCaseToNumPolys = pool.Register(DefaultBufferNames.TerrainCaseToNumPolysCB, ProceduralTerrainGenerationBufferModel.SizeInBytesCaseToNumPolys);
                bufferTerrainEdgeConnectList = pool.Register(DefaultBufferNames.TerrainEdgeConnectListCB, ProceduralTerrainGenerationBufferModel.SizeInBytesEdgeConnectList);
                bufferModel = new ProceduralTerrainGenerationBufferModel()
                {
                    CaseToNumPolys = ProceduralTerrainGenerationLookupTables.CaseToNumPolys,
                    EdgeStart = ProceduralTerrainGenerationLookupTables.EdgeStart,
                    EdgeDir = ProceduralTerrainGenerationLookupTables.EdgeDir,
                    EdgeEnd = ProceduralTerrainGenerationLookupTables.EdgeEnd,
                    EdgeAxis = ProceduralTerrainGenerationLookupTables.EdgeAxis,
                    TriTable = ProceduralTerrainGenerationLookupTables.TriTable
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadToBuffer(DeviceContextProxy context)
            {
                bufferModel.UploadToBuffer(bufferTerrainCaseToNumPolys, context);
                bufferModel.UploadToBuffer(bufferTerrainEdgeConnectList, context);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                RemoveAndDispose(ref bufferTerrainCaseToNumPolys);
                RemoveAndDispose(ref bufferTerrainEdgeConnectList);
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}