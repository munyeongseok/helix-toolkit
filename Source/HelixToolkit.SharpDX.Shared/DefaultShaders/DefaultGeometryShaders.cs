﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX;
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
    namespace Shaders
    {
        using Helper;
        /// <summary>
        /// 
        /// </summary>
        public static class DefaultGSShaderByteCodes
        {
            /// <summary>
            /// 
            /// </summary>
            public static string GSPoint
            {
                get;
            } = "gsPoint";
            /// <summary>
            /// 
            /// </summary>
            public static string GSLine
            {
                get;
            } = "gsLine";
            /// <summary>
            /// Gets the gs line arrow head.
            /// </summary>
            /// <value>
            /// The gs line arrow head.
            /// </value>
            public static string GSLineArrowHead
            {
                get;
            } = "gsLineArrowHead";
            /// <summary>
            /// Gets the gs line arrow tail.
            /// </summary>
            /// <value>
            /// The gs line arrow tail.
            /// </value>
            public static string GSLineArrowHeadTail
            {
                get;
            } = "gsLineArrowHeadTail";
            /// <summary>
            /// 
            /// </summary>
            public static string GSBillboard
            {
                get;
            } = "gsBillboard";

            /// <summary>
            /// 
            /// </summary>
            public static string GSParticle
            {
                get;
            } = "gsParticle";
            /// <summary>
            /// Gets the gs mesh normal vector.
            /// </summary>
            /// <value>
            /// The gs mesh normal vector.
            /// </value>
            public static string GSMeshNormalVector
            {
                get;
            } = "gsMeshNormalVector";

            public static string GSMeshBoneSkinnedOut
            {
                get;
            } = "gsMeshSkinnedOut";

            #region Terrain
            public static string GSTerrain { get; } = "gsTerrain";
            public static string GSTerrainBuildDensity { get; } = "gsTerrainBuildDensity";
            public static string GSTerrainListNonemptyCells { get; } = "gsTerrainListNonemptyCells";
            public static string GSTerrainListVerticesToGenerate { get; } = "gsTerrainListVerticesToGenerate";
            public static string GSTerrainSplatVertexIDs { get; } = "gsTerrainSplatVertexIDs";
            public static string GSTerrainGenerateVertices { get; } = "gsTerrainGenerateVertices";
            public static string GSTerrainGenerateIndices { get; } = "gsTerrainGenerateIndices";
            #endregion
        }


        /// <summary>
        /// Default Geometry Shaders
        /// </summary>
        public static class DefaultGSShaderDescriptions
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSPoint = new ShaderDescription(nameof(GSPoint), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSPoint);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSLine = new ShaderDescription(nameof(GSLine), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSLine);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSLineArrowHead = new ShaderDescription(nameof(GSLineArrowHead), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSLineArrowHead);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSLineArrowHeadTail = new ShaderDescription(nameof(GSLineArrowHeadTail), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSLineArrowHeadTail);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSBillboard = new ShaderDescription(nameof(GSBillboard), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSBillboard);

            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSParticle = new ShaderDescription(nameof(GSParticle), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSParticle);
            /// <summary>
            /// The gs mesh normal vector
            /// </summary>
            public static readonly ShaderDescription GSMeshNormalVector = new ShaderDescription(nameof(GSMeshNormalVector), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSMeshNormalVector);

            /// <summary>
            /// The gs mesh bone skinned out
            /// </summary>
            public static readonly ShaderDescription GSMeshBoneSkinnedOut = new ShaderDescription(nameof(GSMeshBoneSkinnedOut), ShaderStage.Geometry, new ShaderReflector(),
                DefaultGSShaderByteCodes.GSMeshBoneSkinnedOut)
            {
                IsGSStreamOut = true,
                GSSOElement = new global::SharpDX.Direct3D11.StreamOutputElement[]
                {
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "POSITION", 0, 0, 4, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "NORMAL", 0, 0, 3, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "TANGENT", 0, 0, 3, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "BINORMAL", 0, 0, 3, 0),
                },
                GSSOStrides = new int[]
                {
                    DefaultVertex.SizeInBytes
                }
            };

            #region Terrain Geometry Shader Description
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrain = new ShaderDescription(
                nameof(GSTerrain),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrain);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainBuildDensity = new ShaderDescription(
                nameof(GSTerrainBuildDensity),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainBuildDensity);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainListNonemptyCells = new ShaderDescription(
                nameof(GSTerrainListNonemptyCells),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainListNonemptyCells);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainListVerticesToGenerate = new ShaderDescription(
                nameof(GSTerrainListVerticesToGenerate),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainListVerticesToGenerate);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainSplatVertexIDs = new ShaderDescription(
                nameof(GSTerrainSplatVertexIDs),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainSplatVertexIDs);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainGenerateVertices = new ShaderDescription(
                nameof(GSTerrainGenerateVertices),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainGenerateVertices);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription GSTerrainGenerateIndices = new ShaderDescription(
                nameof(GSTerrainGenerateIndices),
                ShaderStage.Geometry,
                new ShaderReflector(),
                DefaultGSShaderByteCodes.GSTerrainGenerateIndices);
            #endregion
        }
    }
}
