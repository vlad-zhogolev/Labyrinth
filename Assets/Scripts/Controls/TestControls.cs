using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LabyrinthGame
{

    namespace Controls
    {

        [System.Serializable]
        public class ShiftEvent : UnityEvent<Labyrinth.Shift> { }

        [System.Serializable]
        public class RotateEvent : UnityEvent<Labyrinth.Tile.RotationDirection> { }

        [System.Serializable]
        public class MoveEvent : UnityEvent<Vector2Int> { }

        public class TestControls : MonoBehaviour
        {
            public ShiftEvent shiftEvent;
            public UnityEvent cancelShift;
            public RotateEvent rotateFreeTile;
            public MoveEvent movePlayerEvent;
            public UnityEvent skipMove;

            // Start is called before the first frame update
            void Start()
            {

            }

            void CheckShiftLineNumber()
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    m_shiftLineNumber = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    m_shiftLineNumber = 3;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    m_shiftLineNumber = 5;
                }
            }

            void CheckShiftCommand()
            {
                Labyrinth.Shift shift = null;

                if (Input.GetKeyDown(KeyCode.A))
                {
                    shift = new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, m_shiftLineNumber);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    shift = new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, m_shiftLineNumber);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    shift = new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical, Labyrinth.Shift.Direction.Negative, m_shiftLineNumber);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    shift = new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical, Labyrinth.Shift.Direction.Positive, m_shiftLineNumber);
                }

                if (shift != null)
                {
                    shiftEvent?.Invoke(shift);
                }
            }

            void CheckCancelShiftCommand()
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    cancelShift?.Invoke();
                }
            }

            void CheckRotationCommand()
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.CounterClockwise);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.Clockwise);
                }
            }

            void CheckMakeMoveCommand()
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    movePlayerEvent?.Invoke(new Vector2Int(m_moveToX, m_moveToY));
                }
            }

            void CheckSkipMoveCommand()
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    skipMove?.Invoke();
                }
            }

            // Update is called once per frame
            void Update()
            {
                CheckShiftLineNumber();
                CheckShiftCommand();
                CheckCancelShiftCommand();
                CheckRotationCommand();
                CheckMakeMoveCommand();
                CheckSkipMoveCommand();
            }

            [SerializeField]
            private int m_shiftLineNumber = 1;

            [SerializeField]
            private int m_moveToX = 0;
            [SerializeField]
            private int m_moveToY = 0;
        }

    } // namespace Controls

} // namespace LabyrinthGame