using UnityEngine;
using UnityEngine.Events;

namespace LabyrinthGame
{

    namespace Controls
    {

        [System.Serializable]
        public class RotateEvent : UnityEvent<Labyrinth.Tile.RotationDirection> { }

        public class TestControls : MonoBehaviour
        {
            public UnityEvent shiftEvent;
            public UnityEvent cancelShift;
            public RotateEvent rotateFreeTile;
            public RotateEvent moveFreeTile;
            public UnityEvent movePlayerEvent;
            public UnityEvent skipMove;

            public static bool IsGamePaused;
            void Awake()
            {
                SetIsGamePaused(false);
            }

            // Update is called once per frame
            void Update()
            {
                if (InputEnabled)
                {
                    CheckShiftCommand();
                    CheckCancelShiftCommand();
                    CheckRotationCommand();
                    CheckMoveTileCommand();
                    CheckMakeMoveCommand();
                    CheckSkipMoveCommand();
                }
            }

            void CheckShiftCommand()
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) shiftEvent?.Invoke();
            }

            void CheckCancelShiftCommand()
            {
                if (Input.GetKeyDown(KeyCode.C)) cancelShift?.Invoke();
            }

            void CheckRotationCommand()
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.CounterClockwise);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.Clockwise);
                }
            }

            void CheckMoveTileCommand()
            {
                if (Input.GetKeyDown(KeyCode.D)) moveFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.Clockwise);
                else if (Input.GetKeyDown(KeyCode.A)) moveFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.CounterClockwise);
            }

            void CheckMakeMoveCommand()
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    movePlayerEvent?.Invoke();
                }
            }

            void CheckSkipMoveCommand()
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    skipMove?.Invoke();
                }
            }

            public void RotateTileCW()
            {
                if (InputEnabled) rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.Clockwise);
            }

            public void RotateTileCCW()
            {
                if (InputEnabled) rotateFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.CounterClockwise);
            }

            public void MoveTileCW()
            {
                if (InputEnabled) moveFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.Clockwise);
            }

            public void MoveTileCCW()
            {
                if (InputEnabled) moveFreeTile?.Invoke(Labyrinth.Tile.RotationDirection.CounterClockwise);
            }

            public void Shift()
            {
                if (InputEnabled) shiftEvent?.Invoke();
            }

            public void CancelShift()
            {
                if (InputEnabled) cancelShift?.Invoke();
            }

            public void SetIsGamePaused(bool value)
            {
                IsGamePaused = value;
                Time.timeScale = IsGamePaused ? 0 : 1;
                m_EndGamePanel.SetActive(IsGamePaused);

                var buttons = m_ButtonsGroup.GetComponentsInChildren<UnityEngine.UI.Button>();
                foreach (var button in buttons)
                {
                    button.interactable = !IsGamePaused;
                }
                m_viewButton.interactable = !IsGamePaused;
                m_showItemButton.interactable = !IsGamePaused;
                m_endGameButton.interactable = !IsGamePaused;
            }

            [SerializeField]
            public bool InputEnabled { get { return m_inputEnabled; } set { m_inputEnabled = value; } }

            [SerializeField]
            private bool m_inputEnabled = true;

            [SerializeField]
            private GameObject m_EndGamePanel;

            [SerializeField]
            private GameObject m_ButtonsGroup;

            [SerializeField]
            private UnityEngine.UI.Button m_viewButton;

            [SerializeField]
            private UnityEngine.UI.Button m_showItemButton;

            [SerializeField]
            private UnityEngine.UI.Button m_endGameButton;
        }

    } // namespace Controls

} // namespace LabyrinthGame
