using UnityEngine;

public class Tile : MonoBehaviour {
    
    public enum Type {
        STRAIGHT, JUNCTION, TURN
    }

    public Type type;

    // These variables show if the tile has a path going in a certain direction

    public bool left;

    public bool right;

    public bool down;

    public bool up;

    //Also information about connections with adjacent tiles can be added

    void Awake() {
        if (type == Type.STRAIGHT) {
            left = false;
            right = false;
            down = true;
            up = true;
        }
        else if (type == Type.JUNCTION) {
            left = false;
            right = true;
            down = true;
            up = true;
        }
        else if (type == Type.TURN) {
            left = false;
            right = true;
            down = false;
            up = true;
        }
    }

    public void RotateCW() {
        bool tmp = up;

        up = left;
        left = down;
        down = right;
        right = tmp;
    }

    public void RotateCCW() {
        bool tmp = up;

        up = right;
        right = down;
        down = left;
        left = tmp;
    }

}
