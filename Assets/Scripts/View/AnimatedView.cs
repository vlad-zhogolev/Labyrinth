using System.Collections;
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

            public Coroutine MoveTo(Vector3 position, float speed = MOVEMENT_SPEED)
            {
                return StartCoroutine(MoveToCoroutine(position, speed));
            }

            IEnumerator MoveToCoroutine(Vector3 position, float speed)
            {
                while (transform.position != position)
                {
                    MoveToStep(position, speed);

                    yield return null;
                }
            }

            public void MoveToStep(Vector3 position, float speed = MOVEMENT_SPEED)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            }

            public async Task MoveToAsync(Vector3 position, float speed = MOVEMENT_SPEED)
            {
                while (transform.position != position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, position, speed* Time.deltaTime);
                    await Task.Yield();
                }
            }

            public Coroutine RotateTo(Quaternion rotation, float speed = ROTATION_SPEED)
            {
                return StartCoroutine(RotateToCoroutine(rotation, speed));
            }

            IEnumerator RotateToCoroutine(Quaternion rotation, float speed)
            {
                while (transform.rotation != rotation)
                {
                    RotateToStep(rotation, speed);

                    yield return null;
                }
            }

            public void RotateToStep(Quaternion rotation, float speed = ROTATION_SPEED)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);
            }

        }
    }
}
