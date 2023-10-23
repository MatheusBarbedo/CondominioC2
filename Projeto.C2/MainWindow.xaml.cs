using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CriarCena();
            CriarCamera();
            CriarLuz();
            CriarAnimacao();
        }

        private void CriarCena()
        {
            Model3DGroup modelo = new Model3DGroup();

            // Chão com textura
            GeometryModel3D chaoModel = new GeometryModel3D();
            MeshGeometry3D chaoMesh = new MeshGeometry3D();
            chaoMesh.Positions.Add(new Point3D(-100, 0, -100));
            chaoMesh.Positions.Add(new Point3D(100, 0, -100));
            chaoMesh.Positions.Add(new Point3D(100, 0, 100));
            chaoMesh.Positions.Add(new Point3D(-100, 0, 100));
            chaoMesh.TextureCoordinates.Add(new Point(0, 0));
            chaoMesh.TextureCoordinates.Add(new Point(10, 0));
            chaoMesh.TextureCoordinates.Add(new Point(10, 10));
            chaoMesh.TextureCoordinates.Add(new Point(0, 10));
            chaoMesh.TriangleIndices.Add(0);
            chaoMesh.TriangleIndices.Add(1);
            chaoMesh.TriangleIndices.Add(2);
            chaoMesh.TriangleIndices.Add(0);
            chaoMesh.TriangleIndices.Add(2);
            chaoMesh.TriangleIndices.Add(3);

            BitmapImage fotoChao = new BitmapImage(new Uri("chao.jpg", UriKind.Relative));
            ImageBrush textura = new ImageBrush(fotoChao);
            textura.TileMode = TileMode.Tile;
            textura.Viewport = new Rect(0, 0, 10, 10);
            textura.Viewbox = new Rect(0, 0, 1, 1);
            chaoModel.Material = new DiffuseMaterial(textura);
            chaoModel.Geometry = chaoMesh;

            modelo.Children.Add(chaoModel);

            // Edifícios com janelas
            Color janela = Colors.Red; // Cor para as janelas
            Color predio = Colors.Gray; // Cor para os predios
            modelo.Children.Add(CriarPredio(4, -20, predio, janela));
            modelo.Children.Add(CriarPredio(5, -15, predio, janela));
            modelo.Children.Add(CriarPredio(6, -10, predio, janela));
            modelo.Children.Add(CriarPredio(7, -5, predio, janela));

            ModelVisual3D visualModel = new ModelVisual3D();
            visualModel.Content = modelo;
            viewport3D.Children.Add(visualModel);
        }

        private Model3DGroup CriarPredio(int andares, double z, Color corPredio, Color corJanela)
        {
            Model3DGroup modelGroup = new Model3DGroup();

            // Geometria do prédio
            MeshGeometry3D buildingMesh = new MeshGeometry3D();
            double width = andares * 3;
            double height = andares * 5;
            double depth = 2;

            buildingMesh.Positions.Add(new Point3D(-width / 2, 0, z));
            buildingMesh.Positions.Add(new Point3D(width / 2, 0, z));
            buildingMesh.Positions.Add(new Point3D(width / 2, height, z));
            buildingMesh.Positions.Add(new Point3D(-width / 2, height, z));
            buildingMesh.TriangleIndices.Add(0);
            buildingMesh.TriangleIndices.Add(1);
            buildingMesh.TriangleIndices.Add(2);
            buildingMesh.TriangleIndices.Add(0);
            buildingMesh.TriangleIndices.Add(2);
            buildingMesh.TriangleIndices.Add(3);

            // Adicione duas janelas pequenas ao prédio (uma ao lado da outra)
            double windowWidth = width / 10; // Largura da janela reduzida
            double windowHeight = height / andares / 5; // Altura da janela reduzida

            double x0 = -windowWidth * 1.5;
            double x1 = windowWidth * 1.5;
            double y0 = height - windowHeight * 1.2;
            double y1 = height - windowHeight * 0.2;
            double z0 = z + 0.1;
            double z1 = z0 + 0.1;

            for (int i = 0; i < 2; i++)
            {
                buildingMesh.Positions.Add(new Point3D(x0, y0, z0));
                buildingMesh.Positions.Add(new Point3D(x1, y0, z0));
                buildingMesh.Positions.Add(new Point3D(x1, y1, z0));
                buildingMesh.Positions.Add(new Point3D(x0, y1, z0));

                buildingMesh.Positions.Add(new Point3D(x0, y0, z1));
                buildingMesh.Positions.Add(new Point3D(x1, y0, z1));
                buildingMesh.Positions.Add(new Point3D(x1, y1, z1));
                buildingMesh.Positions.Add(new Point3D(x0, y1, z1));

                x0 = -x0; // Posicione a próxima janela ao lado da primeira
                x1 = -x1;

                int baseIndex = 8 * i;
                buildingMesh.TriangleIndices.Add(baseIndex);
                buildingMesh.TriangleIndices.Add(baseIndex + 1);
                buildingMesh.TriangleIndices.Add(baseIndex + 2);
                buildingMesh.TriangleIndices.Add(baseIndex);
                buildingMesh.TriangleIndices.Add(baseIndex + 2);
                buildingMesh.TriangleIndices.Add(baseIndex + 3);
            }

            // Material do prédio
            DiffuseMaterial buildingMaterial = new DiffuseMaterial(new SolidColorBrush(corPredio));

            // Material das janelas
            DiffuseMaterial windowMaterial = new DiffuseMaterial(new SolidColorBrush(corJanela));

            // Crie um modelo para o prédio
            GeometryModel3D buildingModel = new GeometryModel3D
            {
                Geometry = buildingMesh,
                Material = buildingMaterial
            };

            // Crie modelos para as janelas
            GeometryModel3D windowModel1 = new GeometryModel3D
            {
                Geometry = buildingMesh,
                Material = windowMaterial
            };

            GeometryModel3D windowModel2 = new GeometryModel3D
            {
                Geometry = buildingMesh,
                Material = windowMaterial
            };

            modelGroup.Children.Add(buildingModel);
            modelGroup.Children.Add(windowModel1);
            modelGroup.Children.Add(windowModel2);

            return modelGroup;
        }


    private void CriarCamera()
        {
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 40, 100);
            camera.LookDirection = new Vector3D(0, -0.5, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 60;
            viewport3D.Camera = camera;
        }

        private void CriarLuz()
        {
            Model3DGroup modelGroup = new Model3DGroup();

            AmbientLight ambientLight = new AmbientLight(Colors.Gray);
            modelGroup.Children.Add(ambientLight);

            Vector3D lightDirection = new Vector3D(1, -1, -1);
            Color lightColor = Colors.White;
            DirectionalLight directionalLight = new DirectionalLight(lightColor, lightDirection);
            modelGroup.Children.Add(directionalLight);
        }

        private void CriarAnimacao()
        {
            Point3DAnimation cameraAnimation = new Point3DAnimation
            {
                From = new Point3D(0, 40, 100),
                To = new Point3D(0, 40, -100),
                Duration = TimeSpan.FromSeconds(20),
                RepeatBehavior = RepeatBehavior.Forever
            };

            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            RotateTransform3D rotateTransform = new RotateTransform3D(rotation);
            viewport3D.Camera.Transform = rotateTransform;

            DoubleAnimation rotationAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(20),
                RepeatBehavior = RepeatBehavior.Forever
            };
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotationAnimation);

            Vector3DAnimation lightAnimation = new Vector3DAnimation
            {
                From = new Vector3D(1, -1, -1),
                To = new Vector3D(-1, -1, 1),
                Duration = TimeSpan.FromSeconds(20),
                RepeatBehavior = RepeatBehavior.Forever
            };

            DirectionalLight directionalLight = new DirectionalLight(Colors.White, new Vector3D(1, -1, -1));
            directionalLight.BeginAnimation(DirectionalLight.DirectionProperty, lightAnimation);
        }
    }
}
