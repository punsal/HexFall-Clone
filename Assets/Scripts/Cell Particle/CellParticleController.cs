using UnityEngine;
using Utility.Extension;

namespace Cell_Particle
{
    [RequireComponent(typeof(ParticleSystem))]
    public class CellParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem system;
        
        private void OnValidate()
        {
            if (system == null)
            {
                system = GetComponent<ParticleSystem>();
            }

            var systemMain = system.main;
            systemMain.playOnAwake = false;
        }

        public void Play(Vector3 position, Color color)
        {
            transform.position = position.AddZ(-2f);
            
            var systemMain = system.main;
            systemMain.startColor = color;

            system.Play();
        }
    }
}
