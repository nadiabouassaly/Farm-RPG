public enum TileState { Normal, Tilled, Watered, NotPlantable }

public class TileData
{
    public TileState state = TileState.Normal;
    public PlantedCrop crop;
}