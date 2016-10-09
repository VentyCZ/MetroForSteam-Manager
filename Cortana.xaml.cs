using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MetroSkinToolkit
{
    public partial class Cortana : UserControl
    {
        public Cortana()
        {
            InitializeComponent();
        }

        public void FillUp(int percentage)
        {
            if (!(percentage <= 100 && percentage >= 0)) { return; }

            if (Dispatcher.CheckAccess())
            {
                var heightOfPercent = ActualHeight / 100;
                percentage = 100 - percentage; //Reverse
                Filler.Height = heightOfPercent * percentage;
            }
            else
            {
                Dispatcher.Invoke(delegate () { FillUp(percentage); });
            }
        }

        public void TestAnim()
        {
            new Thread(delegate ()
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
            }).Start();
        }

        public void SetInnerBrush(Color clr)
        {
            if (Dispatcher.CheckAccess())
            {
                Circle.Fill = new SolidColorBrush(clr);
            }
            else
            {
                Dispatcher.Invoke(delegate () { SetInnerBrush(clr); });
            }
        }

        public void StartRotateAnim(double secRotateTime = 0.75)
        {
            if (Dispatcher.CheckAccess())
            {
                var rotationTime = TimeSpan.FromSeconds(secRotateTime);

                var anim = new DoubleAnimation(0, 360, new Duration(rotationTime));
                anim.RepeatBehavior = RepeatBehavior.Forever;
                CenterForm.BeginAnimation(RotateTransform.AngleProperty, anim);
            }
            else
            {
                Dispatcher.Invoke(delegate () { StartRotateAnim(secRotateTime); });
            }
        }

        public void StopRotateAnim()
        {
            if (Dispatcher.CheckAccess())
            {
                CenterForm.BeginAnimation(RotateTransform.AngleProperty, null);
            }
            else
            {
                Dispatcher.Invoke(delegate () { StopRotateAnim(); });
            }
        }

        public void StartPulseAnim(int min = 5, int max = 10, double secPulse = 0.5)
        {
            if (Dispatcher.CheckAccess())
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
            }
            else
            {
                Dispatcher.Invoke(delegate () { StartPulseAnim(max, min, secPulse); });
            }
        }

        public void StopPulseAnim()
        {
            if (Dispatcher.CheckAccess())
            {
                Circle.BeginAnimation(Shape.StrokeThicknessProperty, null);
            }
            else
            {
                Dispatcher.Invoke(delegate () { StopPulseAnim(); });
            }
        }
    }
}
