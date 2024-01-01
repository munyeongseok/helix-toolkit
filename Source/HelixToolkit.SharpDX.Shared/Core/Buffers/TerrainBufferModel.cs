// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
using SharpDX;
using System;
using System.Linq;
using System.IO;

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
    namespace Core
    {
        using Render;
        using Utilities;

        /// <summary>
        /// Terrain Buffer Model.
        /// </summary>
        public class TerrainBufferModel : MeshGeometryBufferModel<TerrainVertex>
        {
            public TerrainBufferModel()
                : base(PrimitiveTopology.TriangleList,
                      new ImmutableBufferProxy(TerrainVertex.SizeInBytes, BindFlags.VertexBuffer),
                      new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer))
            {
            }

            protected override bool IsVertexBufferChanged(string propertyName, int vertexBufferIndex)
            {
                return base.IsVertexBufferChanged(propertyName, vertexBufferIndex);
            }

            protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
            {
                if (geometry is MeshGeometry3D mesh)
                {
                    switch (bufferIndex)
                    {
                        case 0:
                            if (geometry.Positions != null && geometry.Positions.Count > 0)
                            {
                                var data = BuildVertexArray(mesh);
                                buffer.UploadDataToBuffer(context, data, geometry.Positions.Count, 0, geometry.PreDefinedIndexCount);
                            }
                            else
                            {
                                buffer.UploadDataToBuffer(context, emptyVerts, 0);
                            }
                            break;
                    }
                }
            }

            private TerrainVertex[] BuildVertexArray(MeshGeometry3D geometry)
            {
                var positions = geometry.Positions.GetEnumerator();
                var vertexCount = geometry.Positions.Count;
                var uvs = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.GetEnumerator() : Enumerable.Repeat(Vector2.Zero, vertexCount).GetEnumerator();
                var array = ThreadBufferManager<TerrainVertex>.GetBuffer(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    positions.MoveNext();
                    uvs.MoveNext();
                    array[i].Position = new Vector4(positions.Current, 1f);
                    array[i].UV = uvs.Current;
                }
                positions.Dispose();
                uvs.Dispose();
                return array;
            }
        }
    }
}
