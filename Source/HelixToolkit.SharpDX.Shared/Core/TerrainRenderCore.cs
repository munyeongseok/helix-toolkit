// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using SharpDX;
using SharpDX.Direct3D11;

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
        using Shaders;
        using Utilities;
        using Components;

        public class TerrainRenderCore : GeometryRenderCore, ITerrainRenderParams
        {
            private IElementsBufferModel parameterBuffer;
            public IElementsBufferModel ParameterBuffer
            {

                get => parameterBuffer;
                set
                {
                    var old = parameterBuffer;
                    if (SetAffectsCanRenderFlag(ref parameterBuffer, value))
                    {
                        if (old != null)
                        {
                            old.ElementChanged -= OnElementChanged;
                        }
                        if (parameterBuffer != null)
                        {
                            parameterBuffer.ElementChanged += OnElementChanged;
                        }
                    }
                }
            }

            private RasterizerStateProxy terrainRasterState;
            private ShaderPass terrainShaderPass;
            private bool needsAssignChunkVariables = true;
            private bool needsAssignLodVariables = true;
            private bool needsAssignGlobalRockVariables = true;
            private readonly ConstantBufferComponent terrainChunkCB;
            private readonly ConstantBufferComponent terrainLodCB;
            private readonly ConstantBufferComponent terrainGlobalRockCB;

            public TerrainRenderCore()
            {
                terrainChunkCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.TerrainChunkCB, 123)));
                terrainLodCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.TerrainLodCB, 123)));
                terrainGlobalRockCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.TerrainGlobalRock, 123)));
            }

            protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
            {
                if (base.CreateRasterState(description, force))
                {
                    var newTerrainRasterState = EffectTechnique.EffectsManager.StateManager.Register(new RasterizerStateDescription()
                    {
                        FillMode = FillMode.Solid,
                        CullMode = CullMode.None,
                        DepthBias = description.DepthBias,
                        DepthBiasClamp = description.DepthBiasClamp,
                        SlopeScaledDepthBias = description.SlopeScaledDepthBias,
                        IsDepthClipEnabled = description.IsDepthClipEnabled,
                        IsFrontCounterClockwise = description.IsFrontCounterClockwise,
                        IsMultisampleEnabled = false,
                        IsScissorEnabled = false
                    });
                    RemoveAndDispose(ref terrainRasterState);
                    terrainRasterState = newTerrainRasterState;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                if (base.OnAttach(technique))
                {
                    needsAssignChunkVariables = true;
                    needsAssignLodVariables = true;
                    needsAssignGlobalRockVariables = true;
                    terrainShaderPass = technique[DefaultPassNames.Default];
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref terrainRasterState);
                base.OnDetach();
            }

            protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
            {
                if (base.OnAttachBuffers(context, ref vertStartSlot))
                {
                    ParameterBuffer?.AttachBuffer(context, ref vertStartSlot);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
            {
                WriteChunkVariables(deviceContext);
                WriteLodVariables(deviceContext);
                WriteGlobalRockVariables(deviceContext);

                deviceContext.SetRasterState(terrainRasterState);
                terrainShaderPass.BindShader(deviceContext);
                terrainShaderPass.BindStates(deviceContext, DefaultStateBinding);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);
            }

            private void WriteChunkVariables(DeviceContextProxy deviceContext)
            {
                if (needsAssignChunkVariables)
                {
                    lock (terrainChunkCB)
                    {
                        // Write chunk variables to terrainChunkCB

                        needsAssignChunkVariables = true;
                    }
                }

                terrainChunkCB.Upload(deviceContext);
            }

            private void WriteLodVariables(DeviceContextProxy deviceContext)
            {
                if (needsAssignLodVariables)
                {
                    lock (terrainLodCB)
                    {
                        // Write lod variables to terrainLodCB

                        needsAssignLodVariables = true;
                    }
                }

                terrainLodCB.Upload(deviceContext);
            }

            private void WriteGlobalRockVariables(DeviceContextProxy deviceContext)
            {
                if (needsAssignGlobalRockVariables)
                {
                    lock (terrainGlobalRockCB)
                    {
                        // Write global rock variables to terrainGlobalRockCB

                        needsAssignGlobalRockVariables = true;
                    }
                }

                terrainGlobalRockCB.Upload(deviceContext);
            }

            protected override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
            {
                // Do Nothing
            }

            protected override void OnRenderDepth(RenderContext context, DeviceContextProxy deviceContext, ShaderPass customPass)
            {
                // Do Nothing
            }

            protected override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
            {
                // Do Nothing
            }
        }
    }
}
