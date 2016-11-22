using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MetroSkinToolkit.Components
{
    public partial class Cortana : UserControl
    {
        public Cortana()
        {
            InitializeComponent();
        }

        public void FillUp(int percentage)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                if (!(percentage <= 100 && percentage >= 0))
                    return;

                var heightOfPercent = ActualHeight / 100;
                percentage = 100 - percentage; //Reverse
                Filler.Height = heightOfPercent * percentage;
            });
        }

        public Thread TestAnim()
        {
            var th = new Thread(delegate ()
            {
                for (int x = 0; x <= 100; x++)
                {
                    FillUp(x);

                    Thread.Sleep(100);
                }

                SetInnerBrush(Colors.Transparent);

                StartPulseAnim();

                Thread.Sleep(3500);

                StopPulseAnim();

                StartRotateAnim(1.25);

                Thread.Sleep(5 * 1000);

                StopRotateAnim();

                SetInnerBrush(Colors.Green);
            });

            th.Start();

            return th;
        }

        public void SetInnerBrush(Color clr)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                Circle.Fill = new SolidColorBrush(clr);
            });
        }

        public void StartRotateAnim(double secRotateTime = 0.75)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                var anim = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(secRotateTime)));
                anim.RepeatBehavior = RepeatBehavior.Forever;
                CenterForm.BeginAnimation(RotateTransform.AngleProperty, anim);
            });
        }

        public void StopRotateAnim()
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                CenterForm.BeginAnimation(RotateTransform.AngleProperty, null);
            });
        }

        public void StartPulseAnim(int min = 5, int max = 10, double secPulse = 0.5)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                TimeSpan pulseTime = TimeSpan.FromSeconds(secPulse);

                Storyboard sb = new Storyboard() { RepeatBehavior = RepeatBehavior.Forever };

                var anim = new DoubleAnimation(min, max, new Duration(pulseTime));
                var anim_reverse = new DoubleAnimation(max, min, new Duration(pulseTime)) { BeginTime = pulseTime };

                sb.Children.Add(anim);
                sb.Children.Add(anim_reverse);

                Storyboard.SetTarget(anim, Circle);
                Storyboard.SetTarget(anim_reverse, Circle);

                Storyboard.SetTargetProperty(anim, new PropertyPath(Shape.StrokeThicknessProperty));
                Storyboard.SetTargetProperty(anim_reverse, new PropertyPath(Shape.StrokeThicknessProperty));

                sb.Begin();
            });
        }

        public void StopPulseAnim()
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                Circle.BeginAnimation(Shape.StrokeThicknessProperty, null);
            });
        }
    }
}
