using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace ObticsRaytracer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        struct Pixel
        {
            public int X;
            public int Y;
            public System.Windows.Media.Color Color;
        }

        public Window1()
        { InitializeComponent(); }

        //dimensions of the viewport
        //reduce for faster rendering
        const int width = 180;
        const int height = 101;

        //Colors to choose from
        Color[] colors = new Color[] {
            Color.Make(.07,.49,.071),
            Color.Make(.07, .07, .49),
            Color.Make(.49, .07, .07),
            Color.Make(.21,.21,.35),
            Color.Make(.07,.49,.071),
            Color.Make(.07, .07, .49),
            Color.Make(.49, .07, .07),
            Color.Make(.35,.21,.21),
            Color.Make(.49,.49,.071),
            Color.Make(.49,.49,.49),
            Color.Make(.49,.49,.49),
            Color.Make(.49,.49,.49),
            Color.Make(.29,.49,.49),
            Color.Make(.49,.29,.49),
            Color.Make(.49,.49,.29),
            Color.Make(.49,.29,.19),
            Color.Make(.19,.29,.49),
            Color.Make(.29,.29,.29),
            Color.Make(.07, .49, .49),
            Color.Make(.49, .07, .49),
            Color.Make(.21,.35,.21),
            Color.Make(.15,.21,.11),
            Color.Make(.29,.29,.071),
            Color.Make(.07, .29, .49),
            Color.Make(.29, .07, .49),
            Color.Make(.45,.35,.07),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
            Color.Make(0, 0, 0),
        };

        //light positions to choose from
        Vector[] positions = new Vector[] {
            Vector.Make(-2,2.5,0),
            Vector.Make(1.5,2.5,1.5),
            Vector.Make(1.5,2.5,-1.5),
            Vector.Make(0,3.5,0),
            Vector.Make(1.5,3.5,1.0),
            Vector.Make(-1.0,2.0,3),
            Vector.Make(1.5,1,2),
            Vector.Make(-2,2.5,-2.5),
            Vector.Make(-4,2.5,0),
            Vector.Make(2,2.5,3),
            Vector.Make(-3,2.5,2),
            Vector.Make(0,5,1),
        };

        Surface[] objectSurfaces = new Surface[] {
            Surfaces.Blue,
            Surfaces.Green,
            Surfaces.Mirror,
            Surfaces.Mirror,
            Surfaces.Red,
            Surfaces.Shiny,
            Surfaces.Shiny,
            Surfaces.Shiny
        };
        Surface[] planeSurfaces = new Surface[] {
            Surfaces.CheckerBoard,
            Surfaces.CheckerBoard2,
            Surfaces.Mirror,
            Surfaces.Shiny
        };

        Vector[] objectPositions = new Vector[] {
            Vector.Make(0,0,0),
            Vector.Make(-1,0,1.5),
            Vector.Make(1.5,0,.5),
            Vector.Make(0.5,0,1.5),
            Vector.Make(-2,0,.5)
        };

        double[] objectRadi = new double[] {
            1.0,
            0.5,
            0.2,
            0.6,
            0.4,
            0.7
        };

        Random _Rnd = new Random(100);
        RayTracer RayTracer;


        void ChangeSpherePosition()
        {
            //Scene objects except the plane
            var spheres = RayTracer.DefaultScene.Things.OfType<Sphere>().ToArray();

            //select one object randomly
            var sphere = spheres[_Rnd.Next(spheres.Length)];

            var freePositions = objectPositions.Except(spheres.Select(s => s.Pos.Value)).ToArray();

            var newPosition = freePositions[_Rnd.Next(freePositions.Length)];

            sphere.Pos.Value = newPosition;
        }

        void ChangeSphereRadius()
        {
            //Scene objects except the plane
            var spheres = RayTracer.DefaultScene.Things.OfType<Sphere>().ToArray();

            //select one object randomly
            var sphere = spheres[_Rnd.Next(spheres.Length)];

            var freeRadi = objectRadi.Except(spheres.Select(s => s.Radius.Value)).ToArray();

            var newRadius = freeRadi[_Rnd.Next(freeRadi.Length)];

            sphere.Radius.Value = newRadius;
        }

        void ChangeObjectSurface()
        {
            //Scene objects except the plane
            var objects = RayTracer.DefaultScene.Things.Where(t => !(t is Plane)).ToArray();

            //select one object randomly
            var obj = objects[_Rnd.Next(objects.Length)];

            //its current surface
            var currentSurface = obj.Surface.Value;

            //select those surfaces that are not its current surface
            var otherSurfaces = objectSurfaces.Where(s => s != currentSurface).ToArray();

            //choose the new surface randomly
            var newSurface = otherSurfaces[_Rnd.Next(otherSurfaces.Length)];

            //set the surface. The view will automatically redraw itself
            obj.Surface.Value = newSurface;
        }

        void ChangePlaneSurface()
        {
            var plane = RayTracer.DefaultScene.Things.Where(t => t is Plane).First();

            //its current surface
            var currentSurface = plane.Surface.Value;

            //select those surfaces that are not its current surface
            var otherSurfaces = planeSurfaces.Where(s => s != currentSurface).ToArray();

            //choose the new surface randomly
            var newSurface = otherSurfaces[_Rnd.Next(otherSurfaces.Length)];

            //set the surface. The view will automatically redraw itself
            plane.Surface.Value = newSurface;        
        }

        //change the color of a light
        void ChangeLightColor()
        {
            var lights = RayTracer.DefaultScene.Lights;

            //choose a light
            var light = lights[_Rnd.Next(lights.Count)];

            var currentColor = light.Color.Value;

            //create list of colors other than currentColor
            List<Color> otherColors = colors.Where(c => !c.Equals(currentColor)).ToList();

            //pick a new color
            var newColor = otherColors[_Rnd.Next(otherColors.Count)];

            //set the color. The view will automatically redraw itself
            light.Color.Value = newColor;
        }

        //reposition a light
        void ChangeLightPosition()
        {
            var lights = RayTracer.DefaultScene.Lights;

            //create list of positions where there is no lights positioned yet (free positions)
            var freePositions = positions.Except(lights.Select(l => l.Pos.Value)).ToList();

            var off = Color.Make(0, 0, 0) ;

            //create a list of lights that are on (color is not black)
            var onLights = lights.Where(l => !l.Color.Value.Equals(off)).ToList();

            //if that list is empty do a ChangeLightColor instead.
            if (onLights.Count == 0)
            {
                ChangeLightColor();
                return;
            }

            var light = onLights[_Rnd.Next(onLights.Count)];
            var newPosition = freePositions[_Rnd.Next(freePositions.Count)];

            //set the new position. The view will automatically redraw itself
            light.Pos.Value = newPosition;
        }

        //change the scene randomly in some way
        void DoRandomSceneChange()
        {
            //3 out of 4 change the light color. 1 out of 4 change a light position
            switch (_Rnd.Next(28))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    ChangeLightColor();
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    ChangeLightPosition();
                    break;
                case 21 :
                case 22:
                case 23:
                    ChangeObjectSurface();
                    break;
                case 24:
                case 25:
                    ChangePlaneSurface();
                    break;
                case 26:
                    ChangeSpherePosition();
                    break;
                case 27:
                    ChangeSphereRadius();
                    break;
            }
        }

        //set true to stop changing the scene
        bool _Stop = false;

        //the total number of changes
        int _ChangeCount = 0;

        //the time a change and redraw started
        DateTime _ChangeStartTime;

        //last time GC.Collect was called.
        DateTime _LastGCCollect;

        //the average time a change and redraw took
        double _AvgTime;

        //the bitmap we are going to draw on
        WriteableBitmap _Bitmap;

        //que of pixels ready to be rendered
        System.Collections.Concurrent.ConcurrentQueue<Pixel> _PixelQueue;

        //true if a dump of pixels from the queue to the bitmap is pending
        bool _RenderPending;

        //DumpPixelsFromQueueToBitmap
        void DumpPixelsFromQueueToBitmap()
        {
            _RenderPending = false;

            _Bitmap.Lock();

            Pixel pixel;

            //dump pixels
            while (_PixelQueue.TryDequeue(out pixel))
                _Bitmap.WritePixels(new Int32Rect(pixel.X, pixel.Y, 1, 1), new uint[] { 0xff000000 | ((uint)pixel.Color.R << 16) | ((uint)pixel.Color.G << 8) | ((uint)pixel.Color.B) }, 4, 0);

            _Bitmap.Unlock();
        }

        //QueuePixel
        private void QueuePixel(int x, int y, System.Windows.Media.Color color)
        {
            _PixelQueue.Enqueue(new Pixel { X = x, Y = y, Color = color });

            //if no dump is pending. Invoke one.. delayed
            if (!_RenderPending)
            {
                _RenderPending = true;

                Dispatcher.BeginInvoke(
                    new Action( DumpPixelsFromQueueToBitmap ),
                    DispatcherPriority.Background
                );
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            //stop rendering
            _Stop = true;
        }

        //ChangeLoop.. Invokes itself 'recursively' until _Stop is true.
        void ChangeLoop()
        {
            //going to set WPF properties so use dispatcher
            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        //calculate time since last call and set total redraw count and average time.
                        if (_ChangeCount != 0)
                        {
                            double time = (DateTime.Now - _ChangeStartTime).TotalMilliseconds;

                            if (_ChangeCount == 1)
                                _AvgTime = time;
                            else
                            {
                                double changeCount = _ChangeCount;

                                _AvgTime = (_AvgTime * (changeCount - 1) / changeCount) + (time / changeCount);
                            }

                            AverageTotal.Content = ((int)Math.Round(_AvgTime / 1000)).ToString() + "s";
                            RedrawCount.Content = _ChangeCount.ToString();
                        }

                        //increase change count
                        _ChangeCount += 1;

                        //reset time
                        _ChangeStartTime = DateTime.Now;

                        if (!_Stop)
                        {
                            //if (DateTime.Now - _LastGCCollect > TimeSpan.FromMinutes(1.0))
                            //{
                            //    GC.Collect();
                            //    _LastGCCollect = DateTime.Now;
                            //}

                            //change scene on different thread.
                            var task = Task.Factory.StartNew(() => DoRandomSceneChange());

                            //when change finished call ourselves.
                            task.ContinueWith((prmObj) => ChangeLoop());
                        }
                    }
                ),
                DispatcherPriority.ContextIdle
            );
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //create bitmap
            _Bitmap = new WriteableBitmap(width, height, 300, 300, PixelFormats.Bgra32, null);

            //assign to picture box
            PictureBox.Source = _Bitmap;

            //que of pixels ready to be rendered
            _PixelQueue = new System.Collections.Concurrent.ConcurrentQueue<Pixel>();

            //not yet.
            _RenderPending = false;

            _LastGCCollect = DateTime.Now;

            RayTracer = new RayTracer( width, height, QueuePixel );

            //Start scene rendering and loop of changes via a different thread.
            Task.Factory.StartNew(
                () =>
                {
                    RayTracer.Render(RayTracer.DefaultScene);
                    ChangeLoop();
                }
            );
        }

    }
}
