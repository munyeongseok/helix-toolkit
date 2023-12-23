// The MIT License (MIT)
// Copyright (c) 2018 Helix Toolkit contributors
// See the LICENSE file in the project root for more information.

using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace ProceduralTerrainGenerationDemo
{
    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D TestModel { get; private set; }
        public PBRMaterial TestModelMaterial { get; private set; }
        public Transform3D TestModelTransform { get; private set; }

        public Color AmbientLightColor { get; private set; }
        public Color DirectionalLightColor { get; private set; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera()
            {
                Position = new Point3D(3, 3, 5),
                LookDirection = new Vector3D(-3, -3, -5),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 50000
            };

            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5);
            meshBuilder.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);

            var meshGeometry = meshBuilder.ToMeshGeometry3D();
            meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            TestModel = meshGeometry;
            TestModelMaterial = new PBRMaterial() { AlbedoColor = new Color4(1f, 0, 0, 1f) };
            TestModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        }
    }
}
