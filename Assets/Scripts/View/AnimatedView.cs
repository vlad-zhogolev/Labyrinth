using System.Threading.Tasks;
using UnityEngine;
using System.Threading.Tasks;

namespace LabyrinthGame
{
    namespace View
    {
        public class AnimatedView : MonoBehaviour
        {

            const float MOVEMENT_SPEED = 1;

            const float ROTATION_SPEED = 360;

            public async Task MoveTo(Vector3 position, float speed = MOVEMENT_SPEED)
            {
                while (transform.position != position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);

                    await Task.Yield();
                }
            }

            public async Task RotateTo(Quaternion rotation, float speed = ROTATION_SPEED)
            {
                while (transform.rotation != rotation)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);

                    await Task.Yield();
                }
            }
        }
    } // namespace View
} // namespace LabyrinthGame
