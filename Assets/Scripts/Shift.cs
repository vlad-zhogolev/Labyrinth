public class Shift {

    public enum Orientation {
        HORIZONTAL, VERTICAL
    }

    public enum Direction {
        POSITIVE = 1, NEGATIVE = -1
    }
    
    public Orientation orientation;

    public Direction direction;

    public int index;

    public Shift(Orientation orientation, Direction direction, int index) {
        this.orientation = orientation;
        this.direction = direction;
        this.index = index;
    }

}
