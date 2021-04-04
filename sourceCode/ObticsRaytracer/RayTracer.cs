using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Obtics.Values;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;

namespace ObticsRaytracer
{
    public class RayTracer
    {
        private int screenWidth;
        private int screenHeight;
        private double screenAvg;
        private const int MaxDepth = 5;

        public Action<int, int, System.Windows.Media.Color> setPixel;

        public RayTracer(int screenWidth, int screenHeight, Action<int, int, System.Windows.Media.Color> setPixel)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.screenAvg = Math.Sqrt((double)screenWidth * (double)screenHeight);
            this.setPixel = setPixel;

            DefaultScene = new Scene();

            DefaultScene.Things.Add(
                new Plane() {
                    Norm = Vector.Make(0,1,0),
                    Offset = 0,
                    Surface = ValueProvider.Dynamic(Surfaces.CheckerBoard)
                }
            );

            DefaultScene.Things.Add(
                new Sphere() {
                    Pos = ValueProvider.Dynamic(Vector.Make(0,0,0)),
                    Radius = ValueProvider.Dynamic(1.0),
                    Surface = ValueProvider.Dynamic(Surfaces.Shiny)
                }
            );

            DefaultScene.Things.Add(
                new Sphere()
                {
                    Pos = ValueProvider.Dynamic(Vector.Make(-1, 0, 1.5)),
                    Radius = ValueProvider.Dynamic(.5),
                    Surface = ValueProvider.Dynamic(Surfaces.Mirror)
                }
            );

            DefaultScene.Things.Add(
                new Sphere()
                {
                    Pos = ValueProvider.Dynamic(Vector.Make(0.5, 0, 1.5)),
                    Radius = ValueProvider.Dynamic(.3),
                    Surface = ValueProvider.Dynamic(Surfaces.Blue)
                }
            );


            DefaultScene.Lights.Add(
                new Light() {
                    Pos = ValueProvider.Dynamic(Vector.Make(-2,2.5,0)),
                    Color = ValueProvider.Dynamic(Color.Make(.49,.07,.07))
                }
            );

            DefaultScene.Lights.Add(
                new Light() {
                    Pos = ValueProvider.Dynamic(Vector.Make(1.5,2.5,1.5)),
                    Color = ValueProvider.Dynamic(Color.Make(.07,.07,.49))
                }
            );

            DefaultScene.Lights.Add(
                new Light() {
                    Pos = ValueProvider.Dynamic(Vector.Make(1.5,2.5,-1.5)),
                    Color = ValueProvider.Dynamic(Color.Make(.07,.49,.071))
                }
            );

            DefaultScene.Lights.Add(
                new Light() {
                    Pos = ValueProvider.Dynamic(Vector.Make(0,3.5,0)),
                    Color = ValueProvider.Dynamic(Color.Make(.21,.21,.35))
                }
            );

            var pos = Vector.Make(3, 2, 4);
            var lookAt = Vector.Make(-1, .5, 0);

            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(0, -1, 0);
            Vector right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            DefaultScene.Camera.Forward = forward;
            DefaultScene.Camera.Pos = pos;
            DefaultScene.Camera.Right = right;
            DefaultScene.Camera.Up = up;
        }

        private sealed class Wrap<T>
        {
            public readonly Func<Wrap<T>, T> It;
            public Wrap(Func<Wrap<T>, T> it) { It = it; }
        }

        public static Func<T, U> Y<T, U>(Func<Func<T, U>, Func<T, U>> f)
        {
            Func<Wrap<Func<T, U>>, Func<T, U>> g = wx => f(wx.It(wx));
            return g(new Wrap<Func<T, U>>(wx => f(y => wx.It(wx)(y))));
        }

        sealed class TraceRayArgs
        {
            public readonly Ray Ray;
            public readonly Scene Scene;
            public readonly int Depth;

            public TraceRayArgs(Ray ray, Scene scene, int depth) { Ray = ray; Scene = scene; Depth = depth; }
        }

        struct Pixel
        {
            public int X;
            public int Y;
            public IValueProvider<Color> Color;
        }


        static Func<Func<TraceRayArgs, IValueProvider<Color>>, TraceRayArgs, IValueProvider<Color>> _ComputeTraceRayF =
            ExpressionObserver.Compile(
                (Func<TraceRayArgs, IValueProvider<Color>> f, TraceRayArgs traceRayArgs) =>
                     (from isect in
                          (
                            from iisect in
                            (
                                from thing in traceRayArgs.Scene.Things
                                select thing.Intersect(traceRayArgs.Ray).Value
                            )
                            where iisect != null
                            orderby iisect.Dist
                            select iisect
                          ).Take(1)
                          let pos = isect.Dist * isect.Ray.Dir + isect.Ray.Start
                          let surface = isect.Thing.Surface
                          let posNormal = isect.Thing.Normal(pos)
                          select
                            (
                                from light in traceRayArgs.Scene.Lights
                                let lightPos = light.Pos.Value
                                let lightDirection = (lightPos - pos).Direction
                                where
                                    (
                                        from thing in traceRayArgs.Scene.Things
                                        select thing.Intersect(new Ray() { Start = pos, Dir = lightDirection }).Value
                                    )
                                        .Aggregate(ISect.Remote, (ISect closest, ISect inter) => inter != null && inter.Dist < closest.Dist ? inter : closest)
                                        .Dist > (lightPos - pos).Length //!isInShadow
                                
                                select
                                   Color.Plus(
                                       Color.Times(surface.Value.Diffuse(pos), Vector.Dot(lightDirection, posNormal.Value) > 0 ? Color.Times(Vector.Dot(lightDirection, posNormal.Value), light.Color.Value) : Color.Make(0, 0, 0)),
                                       Color.Times(surface.Value.Specular(pos), Vector.Dot(lightDirection, ((isect.Ray.Dir - (2 * Vector.Dot(posNormal.Value, isect.Ray.Dir) * posNormal.Value))).Direction) > 0
                                       ? Color.Times(
                                           Math.Pow(Vector.Dot(lightDirection, (isect.Ray.Dir - (2 * Vector.Dot(posNormal.Value, isect.Ray.Dir) * posNormal.Value)).Direction), surface.Value.Roughness),
                                           light.Color.Value
                                       )
                                       : Color.Make(0, 0, 0))
                                   )
                            ).Aggregate(
                                traceRayArgs.Depth >= MaxDepth ? Color.Make(.0, .0, .0)
                                : Color.Times(
                                    surface.Value.Reflect(pos + .001 * (isect.Ray.Dir - 2 * Vector.Dot(posNormal.Value, isect.Ray.Dir) * posNormal.Value)),
                                    f(
                                        new TraceRayArgs(
                                            new Ray()
                                            {
                                                Start = pos + .001 * (isect.Ray.Dir - 2 * Vector.Dot(posNormal.Value, isect.Ray.Dir) * posNormal.Value),
                                                Dir = isect.Ray.Dir - 2 * Vector.Dot(posNormal.Value, isect.Ray.Dir) * posNormal.Value
                                            },
                                            traceRayArgs.Scene,
                                            traceRayArgs.Depth + 1
                                        )
                                    ).Value
                                ),
                                (color, natColor) => Color.Plus(color, natColor)
                            )
                         ).DefaultIfEmpty(Color.Background).First()
            );

        //static Func<Func<TraceRayArgs, IValueProvider<Color>>, TraceRayArgs, IValueProvider<Color>> _ComputeTraceRayF =
        //    ExpressionObserver.Compile(
        //        (Func<TraceRayArgs, IValueProvider<Color>> f, TraceRayArgs traceRayArgs) =>
        //                 (from isect in
        //                      (
        //                        from thing in traceRayArgs.Scene.Things
        //                        let iisect = thing.Intersect(traceRayArgs.Ray).Value
        //                        where iisect != null
        //                        orderby iisect.Dist
        //                        select iisect
        //                      ).Take(1)
        //                  let pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start)
        //                  let normal = isect.Thing.Normal(pos).Value
        //                  let reflectDir = Vector.Minus(isect.Ray.Dir, Vector.Times(2 * Vector.Dot(normal, isect.Ray.Dir), normal))
        //                  let naturalColors =
        //                      from light in traceRayArgs.Scene.Lights
        //                      let ldis = Vector.Minus(light.Pos.Value, pos)
        //                      let livec = Vector.Norm(ldis)
        //                      let testIsect =
        //                        (
        //                            from thing in traceRayArgs.Scene.Things
        //                            select thing.Intersect(new Ray() { Start = pos, Dir = livec }).Value
        //                        ).Aggregate(null, (ISect closest, ISect inter) => closest == null || inter != null && inter.Dist < closest.Dist ? inter : closest)
        //                      where testIsect == null || testIsect.Dist > Vector.Mag(ldis) //!isInShadow
        //                      let illum = Vector.Dot(livec, normal)
        //                      let lcolor = illum > 0 ? Color.Times(illum, light.Color.Value) : Color.Make(0, 0, 0)
        //                      let specular = Vector.Dot(livec, Vector.Norm(reflectDir))
        //                      let scolor =
        //                        specular > 0
        //                            ? Color.Times(
        //                                Math.Pow(specular, isect.Thing.Surface.Value.Roughness),
        //                                light.Color.Value
        //                            )
        //                            : Color.Make(0, 0, 0)
        //                      select
        //                        Color.Plus(
        //                            Color.Times(isect.Thing.Surface.Value.Diffuse(pos), lcolor),
        //                            Color.Times(isect.Thing.Surface.Value.Specular(pos), scolor)
        //                        )
        //                  let reflectPos = Vector.Plus(pos, Vector.Times(.001, reflectDir))
        //                  let reflectColor =
        //                        traceRayArgs.Depth >= MaxDepth
        //                              ? Color.Make(.0, .0, .0)
        //                              : Color.Times(
        //                                    isect.Thing.Surface.Value.Reflect(reflectPos),
        //                                    f(
        //                                        new TraceRayArgs(
        //                                            new Ray()
        //                                            {
        //                                                Start = reflectPos,
        //                                                Dir = reflectDir
        //                                            },
        //                                            traceRayArgs.Scene,
        //                                            traceRayArgs.Depth + 1
        //                                        )
        //                                    ).Value
        //                                )

        //                  select
        //                    naturalColors.Aggregate(
        //                        reflectColor,
        //                        (color, natColor) => Color.Plus(color, natColor)
        //                    )
        //                 ).DefaultIfEmpty(Color.Background).First()
        //    );

        static Func<RayTracer, Scene, IValueProvider<IEnumerable<IEnumerable<Pixel>>>> BuildPixelsQuery =
            ExpressionObserver.Compile(
                (RayTracer t, Scene scene) =>
                    from y in Enumerable.Range(0,t.screenHeight).Reverse()
                    let recenterY = -(y - (t.screenHeight / 2.0)) / (2.0 * t.screenAvg)
                    select 
                        from x in Enumerable.Range(0, t.screenWidth)
                        let recenterX = (x - (t.screenWidth / 2.0)) / (2.0 * t.screenAvg)                       
                        select 
                            new Pixel { 
                                X = x, 
                                Y = y, 
                                Color = 
                                    Y(
                                        (Func<Func<TraceRayArgs, IValueProvider<Color>>, Func<TraceRayArgs, IValueProvider<Color>>>) (
                                            f => traceRayArgs =>
                                                _ComputeTraceRayF(f, traceRayArgs)
                                        )
                                    )( 
                                        new TraceRayArgs(
                                            new Ray() { 
                                                Start = scene.Camera.Pos, 
                                                Dir = 
                                                    Vector.Norm(
                                                        Vector.Plus(
                                                            scene.Camera.Forward,
                                                            Vector.Plus( 
                                                                Vector.Times(recenterX, scene.Camera.Right),
                                                                Vector.Times(recenterY, scene.Camera.Up)
                                                            )
                                                        )
                                                    ) 
                                            }, 
                                            scene, 
                                            0
                                        )
                                    ) 
                            }
            );


        object _Pixels;

        internal void Render(Scene scene)
        {
            var pixelsQuery = BuildPixelsQuery(this,scene).Cascade();

            var pixels = new IValueProvider<Color>[screenWidth, screenHeight];

            System.Threading.Tasks.Parallel.ForEach(
                pixelsQuery.SelectMany(row => row),
                pixel =>
                {
                    var colorProvider = pixel.Color.OnException( (Exception ex) => Color.Background );
                    var px = pixel.X;
                    var py = pixel.Y;

                    pixels[px, py] = colorProvider;
                    var npc = colorProvider as INotifyPropertyChanged;

                    if (npc != null)
                        npc.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                        {
                            if (args.PropertyName == "Value")
                                setPixel(px, py, colorProvider.Value.ToDrawingColor());
                        };

                    setPixel(px, py, colorProvider.Value.ToDrawingColor());
                }
            );

            _Pixels = pixels;            
        }

        internal readonly Scene DefaultScene ;
    }

    

    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard =
            new Surface()
            {
                Diffuse = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? Color.Make(1, 1, 1)
                                    : Color.Make(0, 0, 0),
                Specular = pos => Color.Make(1, 1, 1),
                Reflect = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? .1
                                    : .7,
                Roughness = 150
            };

        static double WFloor(double v) { return Math.Floor(v * 0.66666); }
        static double sin60 = Math.Sin(Math.PI * 2.0 / 6.0);
        static double cos60 = Math.Cos(Math.PI * 2.0 / 6.0);

        public static readonly Surface CheckerBoard2 =
            new Surface()
            {
                Diffuse = pos => ((WFloor(pos.Z) + WFloor(sin60 * pos.X + cos60 * pos.Z) + WFloor(sin60 * pos.X - cos60 * pos.Z)) % 2 != 0)
                                    ? Color.Make(1, 1, 1)
                                    : Color.Make(0, 0, 0),
                Specular = pos => Color.Make(1, 1, 1),
                Reflect = pos => ((WFloor(pos.Z) + WFloor(sin60 * pos.X + cos60 * pos.Z) + WFloor(sin60 * pos.X - cos60 * pos.Z)) % 2 != 0)
                                    ? .1
                                    : .7,
                Roughness = 150
            };


        public static readonly Surface Shiny =
            new Surface()
            {
                Diffuse = pos => Color.Make(1, 1, 1),
                Specular = pos => Color.Make(.5, .5, .5),
                Reflect = pos => .6,
                Roughness = 50
            };

        public static readonly Surface Mirror =
            new Surface()
            {
                Diffuse = pos => Color.Make(.1, .1, .1),
                Specular = pos => Color.Make(.9, .9, .9),
                Reflect = pos => .9,
                Roughness = 200
            };

        public static readonly Surface Blue =
            new Surface()
            {
                Diffuse = pos => Color.Make(.3, .3, 1),
                Specular = pos => Color.Make(.8, .8, .3),
                Reflect = pos => .1,
                Roughness = 120
            };

        public static readonly Surface Red =
            new Surface()
            {
                Diffuse = pos => Color.Make(1, .3, .3),
                Specular = pos => Color.Make(.3, .8, .8),
                Reflect = pos => .1,
                Roughness = 120
            };

        public static readonly Surface Green =
            new Surface()
            {
                Diffuse = pos => Color.Make(.3, 1, .3),
                Specular = pos => Color.Make(.8, .3, .8),
                Reflect = pos => .1,
                Roughness = 120
            };

    }

    struct Vector : IEquatable<Vector>
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public Vector(double x, double y, double z) { X = x; Y = y; Z = z; }

        public static Vector Make(double x, double y, double z) { return new Vector(x, y, z); }
        public static Vector Times(double n, Vector v)
        {
            return new Vector(v.X * n, v.Y * n, v.Z * n);
        }

        public static Vector operator *(double n, Vector v)
        {
            return Times(n, v);
        }

        public static Vector Minus(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector operator -(Vector v1, Vector v2)
        { return Minus(v1, v2); }

        public static Vector Plus(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector operator +(Vector v1, Vector v2)
        { return Plus(v1, v2); }

        public static double Dot(Vector v1, Vector v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        public static double operator & (Vector v1, Vector v2)
        { return Dot(v1, v2); }

        public static double Mag(Vector v) { return Math.Sqrt(Dot(v, v)); }

        public static double operator !(Vector v)
        { return Mag(v); }

        public double Length { get { return Mag(this); } }

        public static Vector Norm(Vector v)
        {
            double mag = Mag(v);
            double div = mag == 0 ? double.PositiveInfinity : 1 / mag;
            return Times(div, v);
        }

        public Vector Direction { get { return Norm(this); } }

        public static Vector Cross(Vector v1, Vector v2)
        {
            return new Vector(((v1.Y * v2.Z) - (v1.Z * v2.Y)),
                              ((v1.Z * v2.X) - (v1.X * v2.Z)),
                              ((v1.X * v2.Y) - (v1.Y * v2.X)));
        }

        public static Vector operator ^(Vector v1, Vector v2)
        { return Cross(v1, v2); }

        public static bool Equals(Vector v1, Vector v2)
        {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }

        public static bool operator ==(Vector v1, Vector v2)
        { return Equals(v1, v2); }

        public static bool operator !=(Vector v1, Vector v2)
        { return !Equals(v1, v2); }


        #region IEquatable<Vector> Members

        public bool Equals(Vector other)
        { return Vector.Equals(this,other); }

        #endregion

        public override bool Equals(object obj)
        { return obj is Vector && Equals((Vector)obj); }

        public override int GetHashCode()
        {
            unchecked
            { return X.GetHashCode() * 37282 + Y.GetHashCode() * 11234 + Z.GetHashCode() * 33271; };
        }
    }

    public struct Color : IEquatable<Color>
    {
        public double R;
        public double G;
        public double B;

        public Color(double r, double g, double b) { R = r; G = g; B = b; }

        public static Color Make(double r, double g, double b) { return new Color(r, g, b); }

        public static Color Times(double n, Color v)
        {
            return new Color(n * v.R, n * v.G, n * v.B);
        }
        public static Color Times(Color v1, Color v2)
        {
            return new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);
        }

        public static Color Plus(Color v1, Color v2)
        {
            return new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
        }
        public static Color Minus(Color v1, Color v2)
        {
            return new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
        }

        public static readonly Color Background = Make(0, 0, 0);
        public static readonly Color DefaultColor = Make(0, 0, 0);

        private double Legalize(double d)
        {
            return d > 1 ? 1 : d;
        }

        public System.Windows.Media.Color ToDrawingColor()
        {
            unchecked
            {
                return System.Windows.Media.Color.FromRgb((byte)(Legalize(R) * 255), (byte)(Legalize(G) * 255), (byte)(Legalize(B) * 255));
            }
        }

        #region IEquatable<Color> Members

        public bool Equals(Color other)
        { return R == other.R && G == other.G && B == other.B; }

        #endregion

        public override bool Equals(object obj)
        { return obj is Color && Equals((Color)obj); }

        public override int GetHashCode()
        {
            unchecked
            { return R.GetHashCode() * 22373 + G.GetHashCode() * 12321 + B.GetHashCode() * 333333; };
        }

    }

    sealed class Ray : IEquatable<Ray>
    {
        public Vector Start;
        public Vector Dir;

        #region IEquatable<Ray> Members

        public bool Equals(Ray other)
        {
            return object.ReferenceEquals(this,other) || other != null && Start.Equals(other.Start) && Dir.Equals(other.Dir);
        }

        #endregion

        public override bool Equals(object obj)
        { return Equals(obj as Ray); }

        public override int GetHashCode()
        {
            unchecked
            { return Start.GetHashCode() * 44321 + Dir.GetHashCode(); }
        }
    }

    sealed class ISect
    {
        public SceneObject Thing;
        public Ray Ray;
        public double Dist;

        public override bool Equals(object obj)
        {
            var other = obj as ISect;
            return other != null && object.Equals(Thing,other.Thing) && object.Equals(Ray,other.Ray) && Dist == other.Dist ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 
                    (Thing == null ? 0 : Thing.GetHashCode() * 373282) 
                    + (Ray == null ? 0 : Ray.GetHashCode() * 33281)
                    + Dist.GetHashCode() * 94828;
            }
        }

        public static readonly ISect Remote = new ISect { Dist = double.MaxValue };
    }

    sealed class Surface
    {
        public Func<Vector, Color> Diffuse;
        public Func<Vector, Color> Specular;
        public Func<Vector, double> Reflect;
        public double Roughness;
    }

    sealed class Camera
    {
        public Vector Pos;
        public Vector Forward;
        public Vector Up;
        public Vector Right;

        public static Camera Create(Vector pos, Vector lookAt)
        {
            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(0, -1, 0);
            Vector right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }

    sealed class Light
    {
        public IValueProvider<Vector> Pos;
        public IValueProvider<Color> Color;
    }

    abstract class SceneObject
    {
        public IValueProvider<Surface> Surface;
        public abstract IValueProvider<ISect> Intersect( Ray ray);
        public abstract IValueProvider<Vector> Normal(Vector pos);
    }

    sealed class Sphere : SceneObject
    {
        public IValueProvider<Vector> Pos;
        public IValueProvider<double> Radius;

        ISect intersect(Vector pos, double radius, Ray ray)
        {
            Vector center = Vector.Plus(pos, Vector.Make(.0, radius, .0)); 
            Vector eo = Vector.Minus(center, ray.Start);
            double v = Vector.Dot(eo, ray.Dir);
            double dist;
            if (v < 0)
            {
                dist = 0;
            }
            else
            {
                double disc = Math.Pow(radius, 2) - (Vector.Dot(eo, eo) - Math.Pow(v, 2));
                dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
            }
            if (dist == 0) return null;
            return new ISect()
            {
                Thing = this,
                Ray = ray,
                Dist = dist
            };
        }

        public override IValueProvider<ISect> Intersect(Ray ray)
        {
            return Pos.Select(Radius, ValueProvider.Static(ray), (Func<Vector,double,Ray,ISect>)intersect);
        }

        public override IValueProvider<Vector> Normal(Vector pos)
        {
            return Pos.Select(Radius, ValueProvider.Static(pos), (pint,radius,pext) => Vector.Norm(Vector.Minus(pext, Vector.Plus(pint, Vector.Make(.0, radius, .0)))));
        }
    }

    sealed class Plane : SceneObject
    {
        public Vector Norm;
        public double Offset;

        public override IValueProvider<ISect> Intersect(Ray ray)
        {
            double denom = Vector.Dot(Norm, ray.Dir);
            if (denom > 0) return null;
            return ValueProvider.Static( new ISect()
                {
                    Thing = this,
                    Ray = ray,
                    Dist = (Vector.Dot(Norm, ray.Start) + Offset) / (-denom)
                }
            );
        }

        public override IValueProvider<Vector> Normal(Vector pos)
        {
            return ValueProvider.Static(Norm);
        }
    }

    sealed class Scene
    {
        ObservableCollection<SceneObject> _Things = new ObservableCollection<SceneObject>();
        public ObservableCollection<SceneObject> Things { get { return _Things; } }

        ObservableCollection<Light> _Lights = new ObservableCollection<Light>();
        public ObservableCollection<Light> Lights { get { return _Lights; } }

        Camera _Camera = new Camera();
        public Camera Camera { get { return _Camera; } }
    }

    public delegate void Action<T, U, V>(T t, U u, V v);

}
