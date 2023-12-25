// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using SharpDX;
using System.Collections.Generic;
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
    using Render;
    using System;
    using Utilities;

    public interface IProceduralTerrainGenerationBufferProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public uint[] CaseToNumPolys { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3[] EdgeStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3[] EdgeDir { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3[] EdgeEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint[] EdgeAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector4[] TriTable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="context"></param>
        void UploadToBuffer(IBufferProxy buffer, DeviceContextProxy context);
    }
}
