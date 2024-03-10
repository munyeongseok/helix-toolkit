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
        using System;

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
            private ShaderPass buildDensityPass;
            private SamplerStateProxy linearRepeatSampler;
            private int noiseVolumeTBSlot;
            private int linearRepeatSamplerSlot;
            private ShaderResourceViewProxy randomNoiseVolumeTexture;

            public TerrainRenderCore()
            {
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
                    // Shader Pass
                    terrainShaderPass = technique[DefaultPassNames.Default];
                    buildDensityPass = technique[ProceduralTerrainGenerationPassNames.BuildDensity];

                    // Sampler State
                    linearRepeatSampler = technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerWrapAni1);

                    // Shader Resource View
                    randomNoiseVolumeTexture = CreateRandomNoiseVolumeTextureSRV(16, 16, 16);
                    
                    // Bind Slot
                    noiseVolumeTBSlot = buildDensityPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.TerrainNoiseVolumeTB);
                    linearRepeatSamplerSlot = buildDensityPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.TerrainLinearRepeatSampler);

                    // Rendering test code. Remove after testing. ---------------------------------------------------------
                    //terrainShaderPass = technique[ProceduralTerrainGenerationPassNames.BuildDensity];
                    // ----------------------------------------------------------------------------------------------------
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
                buildDensityPass.PixelShader.BindTexture(deviceContext, noiseVolumeTBSlot, randomNoiseVolumeTexture);
                buildDensityPass.PixelShader.BindSampler(deviceContext, linearRepeatSamplerSlot, linearRepeatSampler);
                buildDensityPass.BindShader(deviceContext, false);
                //DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);



                deviceContext.SetRasterState(terrainRasterState);
                // Rendering test code. Remove after testing. ---------------------------------------------------------
                //terrainShaderPass.PixelShader.BindTexture(deviceContext, noiseVolumeTBSlot, randomNoiseVolumeTexture);
                //terrainShaderPass.PixelShader.BindSampler(deviceContext, linearRepeatSamplerSlot, linearRepeatSampler);
                // ----------------------------------------------------------------------------------------------------
                terrainShaderPass.BindShader(deviceContext);
                terrainShaderPass.BindStates(deviceContext, DefaultStateBinding);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);
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

            private ShaderResourceViewProxy CreateRandomNoiseVolumeTextureSRV(int width, int height, int depth)
            {
                var random = new Random();
                var pixels = new byte[width * height * depth];
                random.NextBytes(pixels);
                return ShaderResourceViewProxy.CreateViewFromPixelData(EffectTechnique.EffectsManager.Device, pixels, width, height, depth, global::SharpDX.DXGI.Format.R8_UNorm, true, false);
            }
        }
    }
}
