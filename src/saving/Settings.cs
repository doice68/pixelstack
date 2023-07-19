public class Settings
{

    public bool rotate { get; set; } = true;
    public float zoom { get; set; } = 1;
    public Vector3 previewColor { get; set; } = Color.DARKPURPLE.ToVec();
    public bool showCurrentLayer { get; set; }
    public bool fillInBetween { get; set; }
    public int spread { get; set; } = 2;
    public float squashAmount { get; set; } = 1;
    public int placementmode { get; set; }
    public Vector3[] colors { get; set; }
}