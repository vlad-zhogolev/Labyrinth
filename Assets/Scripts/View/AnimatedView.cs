using System.Collections.Generic;
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

            const float CARRYING_HEIGHT = 1;

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

            public async Task FollowWaypoints(List<Vector3> waypoints, float speed = MOVEMENT_SPEED)
            {
                foreach (var waypoint in waypoints)
                {
                    await MoveTo(waypoint, speed);
                }
            }

            public async Task CarryTo(Vector3 position, float height = CARRYING_HEIGHT, float speed = MOVEMENT_SPEED)
            {
                var heightVector = new Vector3(0, height, 0);

                var waypoints = new List<Vector3>
                {
                    transform.position + heightVector,
                    position + heightVector,
                    position
                };

                await FollowWaypoints(waypoints, speed);
            }

        }

    } // namespace View

} // namespace LabyrinthGame
