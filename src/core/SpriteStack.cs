public class SpriteStack : ICloneable
{
    public List<RenderTexture2D> layers = new();
    public Vector2 size; 
    public Vector2 spriteSize; 
    public bool showCurrentLayer;
    public float angle = 45;
    public float zoom = 2f;
    public float squashAmount = 1;
    public int spread = 2;
    public bool fillInBetween = false;

    public SpriteStack(Vector2 canvasSize, Vector2 spriteSize)
    {
        this.size = canvasSize;
        this.spriteSize = spriteSize;
    }

    public void Render()
    {        
        // BeginTextureMode(texture2D);
        // ClearBackground(previewColor.ToColor());
        
        Rlgl.rlPushMatrix();
        
        var xPos = size.X / 2;
        var yPos = size.Y / 2 - layers.Count * spread / 2 * zoom;
        Rlgl.rlTranslatef(xPos, yPos, 0);
        Rlgl.rlScalef(zoom, squashAmount * zoom, 1);
        
        for (int i = 0; i < layers.Count; i++)
        {
            RenderTexture2D layer = layers[i];
            for (int k = 0; k < spread; k++)
            {
                float w = spriteSize.X;
                float h = spriteSize.Y;

                var c = Color.WHITE;
                //TODO: Hard coded, fix this
                if (showCurrentLayer == true)
                {
                    c = App.instance.canvasView.currentLayer == i ? new Color(200, 200, 200, 255) : Color.WHITE;
                }
                // Console.WriteLine(i * spread + k);
                DrawLayer(i, layer, (int)(i * spread + k), w, h, c);

                if (fillInBetween == false)
                    break;
            }
        }

        Rlgl.rlPopMatrix();
        // EndTextureMode();
    }

    public void DrawLayer(int i, RenderTexture2D layer, int y, float w, float h, Color c)
    {
        DrawTexturePro(
            layer.texture,
            new Rectangle(0, 0, layer.texture.width, layer.texture.height),
            new Rectangle(0, y, w, h),
            new Vector2(w / 2, h / 2),
            angle,
            c);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}