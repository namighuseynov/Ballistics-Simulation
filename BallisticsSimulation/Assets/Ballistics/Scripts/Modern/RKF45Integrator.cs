using System;
using System.Collections.Generic;

namespace BallisticsSimulation
{
    /// <summary>
    /// Fehlberg 4/5 adaptive Runge–Kutta integrator (RKF45)  
    /// • err – оценивается только по координатам (X,Y,Z)  
    /// • шаг ограничен диапазоном [hMin, hMax]  
    /// • если err == 0, шаг увеличивается в 2 раза, но не выше hMax  
    /// </summary>
    public class RKF45Integrator : IIntegrator
    {
        public List<State> Calculate(
            in State initState,
            double step,          // initial step size
            int maxSteps,
            BallisticsHandler handler,
            double eps = 1e-4,  // tolerance (м)
            double hMin = 1e-4,  // minimum step (с)
            double hMax = 0.01)  // maximum step (с)
        {
            var trajectory = new List<State>(1024);

            State state = new State(initState);
            trajectory.Add(new State(state));

            double h = step;
            int counter = 0;

            while (counter < maxSteps && state.Y >= 0.0)
            {
                //=== k1 … k6 ===
                State k1 = handler.Derivatives(state);

                State k2 = handler.Derivatives(state.Add(
                           k1.Dot(0.25 * h)));

                State k3 = handler.Derivatives(state.Add(
                           k1.Dot(3.0 / 32.0 * h)
                        .Add(k2.Dot(9.0 / 32.0 * h))));

                State k4 = handler.Derivatives(state.Add(
                           k1.Dot(1932.0 / 2197.0 * h)
                        .Add(k2.Dot(-7200.0 / 2197.0 * h))
                        .Add(k3.Dot(7296.0 / 2197.0 * h))));

                State k5 = handler.Derivatives(state.Add(
                           k1.Dot(439.0 / 216.0 * h)
                        .Add(k2.Dot(-8.0 * h))
                        .Add(k3.Dot(3680.0 / 513.0 * h))
                        .Add(k4.Dot(-845.0 / 4104.0 * h))));

                State k6 = handler.Derivatives(state.Add(
                           k1.Dot(-8.0 / 27.0 * h)
                        .Add(k2.Dot(2.0 * h))
                        .Add(k3.Dot(-3544.0 / 2565.0 * h))
                        .Add(k4.Dot(1859.0 / 4104.0 * h))
                        .Add(k5.Dot(-11.0 / 40.0 * h))));

                //=== 4‑й и 5‑й порядок ===
                State y4 = state.Add(
                           k1.Dot(25.0 / 216.0 * h)
                        .Add(k3.Dot(1408.0 / 2565.0 * h))
                        .Add(k4.Dot(2197.0 / 4104.0 * h))
                        .Add(k5.Dot(-1.0 / 5.0 * h)));

                State y5 = state.Add(
                           k1.Dot(16.0 / 135.0 * h)
                        .Add(k3.Dot(6656.0 / 12825.0 * h))
                        .Add(k4.Dot(28561.0 / 56430.0 * h))
                        .Add(k5.Dot(-9.0 / 50.0 * h))
                        .Add(k6.Dot(2.0 / 55.0 * h)));

                //=== локальная погрешность только по координатам ===
                double dx = y5.X - y4.X;
                double dy = y5.Y - y4.Y;
                double dz = y5.Z - y4.Z;
                double err = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                bool acceptStep = (err <= eps) || (h <= hMin);

                if (acceptStep)
                {
                    state = y5;
                    if (state.Y < 0)
                    {
                        break;
                    }
                    trajectory.Add(new State(state));
                    counter++;
                }

                //=== новый шаг (ограничен) ===
                double s = (err == 0.0) ? 2.0
                                        : 0.9 * Math.Pow(eps / err, 0.2);
                h = Math.Clamp(h * s, hMin, hMax);

                // защита от зацикливания
                if (h < hMin * 0.5)
                    break;
            }

            return trajectory;
        }
    }
}
