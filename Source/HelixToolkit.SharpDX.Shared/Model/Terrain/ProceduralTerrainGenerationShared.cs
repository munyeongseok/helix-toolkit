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
            public readonly ProceduralTerrainGenerationBufferModel BufferModel = new ProceduralTerrainGenerationBufferModel();

            private IBufferProxy bufferTerrainCaseToNumPolys;
            private IBufferProxy bufferTerrainEdgeConnectList;

            public ProceduralTerrainGenerationShared(IConstantBufferPool pool)
            {
                bufferTerrainCaseToNumPolys = pool.Register(DefaultBufferNames.TerrainCaseToNumPolysCB, ProceduralTerrainGenerationBufferModel.SizeInBytesCaseToNumPolys);
                bufferTerrainEdgeConnectList = pool.Register(DefaultBufferNames.TerrainEdgeConnectListCB, ProceduralTerrainGenerationBufferModel.SizeInBytesEdgeConnectList);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadToBuffer(DeviceContextProxy context)
            {
                BufferModel.UploadToBuffer(bufferTerrainCaseToNumPolys, context);
                BufferModel.UploadToBuffer(bufferTerrainEdgeConnectList, context);
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