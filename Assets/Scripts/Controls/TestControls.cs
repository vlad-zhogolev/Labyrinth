using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LabyrinthGame {

namespace Controls {

[System.Serializable]
public class ShiftEvent : UnityEvent<Labyrinth.Shift> {}

public class TestControls : MonoBehaviour
{
    public ShiftEvent shiftEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CheckShiftLineNumber()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            m_shiftLineNumber = 1;
        }
        else if ( Input.GetKeyDown(KeyCode.Alpha3))
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

    // Update is called once per frame
    void Update()
    {
        CheckShiftLineNumber();
        CheckShiftCommand();
    }

    [SerializeField]
    private int m_shiftLineNumber = 1;
}

} // namespace Controls

} // namespace LabyrinthGame