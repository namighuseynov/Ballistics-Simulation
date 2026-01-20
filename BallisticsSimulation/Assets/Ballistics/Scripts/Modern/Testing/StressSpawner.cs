using UnityEngine;
using UnityEngine.Profiling;

namespace BallisticsSimulation.Test {
    public class StressSpawner : MonoBehaviour
    {
        [SerializeField] private WeaponController controller;
        public int count = 1000;


        void Start()
        {
            Shot();
            Debug.Log($"{count} bullets spawned.");
        }

        private void Shot()
        {
            for (int i = 0; i < count; i++)
            {

                controller.Shot();
            }
        }

        float timer;
        void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 10f)
            {
                float avgMs = Time.timeSinceLevelLoad * 1000f / Time.frameCount;
                float rssMb = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
                Debug.Log($"Stress test {count}: AvgFPS={(1000 / avgMs):F1}  RSS={rssMb:F1} MB");
                enabled = false;
            }
        }
    }
}