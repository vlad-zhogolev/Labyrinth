using UnityEngine;

public class Labyrinth : MonoBehaviour {

    GameObject[,] tiles = new GameObject[7, 7];

    GameObject[] fixedTiles = new GameObject[16];

    GameObject[] movingTiles = new GameObject[34];

    GameObject freeTile = null;

    Shift[] shifts = new Shift[12];

    int peviousShiftIndex = 0;

    int currentShiftIndex = 0;

    public void PositionTiles() {
        // Position the tiles rundomly
    }

    public void ShiftTiles(Shift shift) {
        // Shift the tiles in the 2D array, assign a new free tile, animate the tiles
        // If the shift orientation is horizontal, shift a row
        // If the shift orientation is 
    }

}
