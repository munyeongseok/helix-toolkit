// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using System;
using SharpDX;

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
    namespace Model.Scene
    {
        using Core;
        using System.Collections.Generic;
        using System.Linq;

        /// <summary>
        /// 
        /// </summary>
        public class TerrainNode : SceneNode
        {
            #region Properties

            private Geometry3D geometry;
            private Geometry3D Geometry
            {
                get => geometry;
                set
                {
                    var old = geometry;
                    if (Set(ref geometry, value))
                    {
                        if (IsAttached)
                        {
                            CreateGeometryBuffer();
                        }
                        InvalidateRender();
                    }
                }
            }

            private IList<TerrainInstanceParameter> instances = Enumerable.Repeat(default(TerrainInstanceParameter), 10).ToList();
            private IList<TerrainInstanceParameter> Instances
            {
                get => instances;
                set
                {
                    if (Set(ref instances, value))
                    {
                        instanceBuffer.Elements = value;
                    }
                }
            }

            #endregion

            private IAttachableBufferModel geometryBuffer;
            private IElementsBufferModel<TerrainInstanceParameter> instanceBuffer = new InstanceParamsBufferModel<TerrainInstanceParameter>(TerrainInstanceParameter.SizeInBytes);

            public TerrainNode()
            {
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IEffectsManager effectsManager)
            {
                return effectsManager[DefaultRenderTechniqueNames.Terrain];
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return new TerrainRenderCore() { ParameterBuffer = instanceBuffer };
            }

            protected override bool OnAttach(IEffectsManager effectsManager)
            {
                if (base.OnAttach(effectsManager))
                {
                    CreateGeometryBuffer();
                    instanceBuffer.Initialize();
                    instanceBuffer.Elements = Instances;
                    if (RenderCore is IGeometryRenderCore core)
                    {
                        core.InstanceBuffer = instanceBuffer;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void CreateGeometryBuffer()
            {
                var newGeometryBuffer = EffectsManager.GeometryBufferManager.Register<TerrainBufferModel>(GUID, Geometry);
                RemoveAndDispose(ref geometryBuffer);
                geometryBuffer = newGeometryBuffer;
                if (RenderCore is IGeometryRenderCore core)
                {
                    core.GeometryBuffer = geometryBuffer;
                }
            }

            protected override void OnDetach()
            {
                if (RenderCore is IGeometryRenderCore core)
                {
                    core.GeometryBuffer = null;
                }
                RemoveAndDispose(ref geometryBuffer);
                instanceBuffer.DisposeAndClear();
                base.OnDetach();
            }

            protected override bool CanHitTest(HitTestContext context)
            {
                return false;
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }
}
