// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

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
            private ShaderPass listNonemptyCellsPass;
            private ShaderPass listVerticesToGenerate;
            private SamplerStateProxy linearRepeatSampler;
            private SamplerStateProxy nearestClampSampler;
            private int noiseVolumeTBSlot;
            private int densityVolumeTBSlot;
            private int linearRepeatSamplerSlot;
            private int nearestClampSamplerSlot;
            private ShaderResourceViewProxy randomNoiseVolumeTexture;
            private ShaderResourceViewProxy densityTexture;

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
                    listNonemptyCellsPass = technique[ProceduralTerrainGenerationPassNames.ListNonemptyCells];
                    listVerticesToGenerate = technique[ProceduralTerrainGenerationPassNames.ListVerticesToGenerate];

                    // Sampler State
                    linearRepeatSampler = technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerWrapAni1);
                    nearestClampSampler = technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1);

                    // Shader Resource View
                    randomNoiseVolumeTexture = CreateRandomNoiseVolumeTextureSRV();
                    densityTexture = CreateDensityTextureSRV();

                    // Bind Slot
                    noiseVolumeTBSlot = buildDensityPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.TerrainNoiseVolumeTB);
                    densityVolumeTBSlot = listNonemptyCellsPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.TerrainDensityVolumeTB);
                    linearRepeatSamplerSlot = buildDensityPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.TerrainLinearRepeatSampler);
                    nearestClampSamplerSlot = listNonemptyCellsPass.VertexShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.TerrainNearestClampSampler);

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
                var currentRenderTarget = deviceContext.GetRenderTargets(1)[0];

                // Render Pass 1: Build Density
                deviceContext.SetRenderTarget(densityTexture);
                buildDensityPass.PixelShader.BindTexture(deviceContext, noiseVolumeTBSlot, randomNoiseVolumeTexture);
                buildDensityPass.PixelShader.BindSampler(deviceContext, linearRepeatSamplerSlot, linearRepeatSampler);
                buildDensityPass.BindShader(deviceContext, bindConstantBuffer: false);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);

                // Render Pass 2: List Nonempty Cells
                deviceContext.SetRenderTarget(null);
                listNonemptyCellsPass.VertexShader.BindTexture(deviceContext, densityVolumeTBSlot, densityTexture);
                listNonemptyCellsPass.VertexShader.BindSampler(deviceContext, nearestClampSamplerSlot, nearestClampSampler);
                listNonemptyCellsPass.BindShader(deviceContext, bindConstantBuffer: false);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);

                // Render Pass 3: List Vertices To Generate
                deviceContext.SetRenderTarget(null);
                listVerticesToGenerate.BindShader(deviceContext, bindConstantBuffer: false);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);



                deviceContext.SetRenderTarget(currentRenderTarget);
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

            private ShaderResourceViewProxy CreateRandomNoiseVolumeTextureSRV()
            {
                var width = 16;
                var height = 16;
                var depth = 16;
                var pixels = new byte[width * height * depth];
                var format = Format.R8_UNorm;
                var createSRV = true;
                var generateMipMaps = false;
                var random = new Random();
                random.NextBytes(pixels);
                return ShaderResourceViewProxy.CreateViewFromPixelData(EffectTechnique.EffectsManager.Device, pixels, width, height, depth, format, createSRV, generateMipMaps);
            }

            private ShaderResourceViewProxy CreateDensityTextureSRV()
            {
                var width = 256;
                var height = 256;
                var depth = 256;
                var pixels = new Half4[width * height * depth];
                var format = Format.R16G16B16A16_Float;
                var createSRV = true;
                var generateMipMaps = false;
                return ShaderResourceViewProxy.CreateViewFromPixelData(EffectTechnique.EffectsManager.Device, pixels, width, height, depth, format, createSRV, generateMipMaps);
            }
        }
    }
}
