public class SpriteSheetExporter : IExporter<Image>
{
    public Image Export(SpriteStack spriteStack)
    {
        int width = (int)App.instance.width;
        int height = (int)(App.instance.height);
        
        var texture = LoadRenderTexture(width * spriteStack.layers.Count, height);
        BeginTextureMode(texture);
        for (int i = 0; i < spriteStack.layers.Count; i++)
        {
            DrawTexture(spriteStack.layers[i].texture, i * width, 0, Color.WHITE);
        }
        EndTextureMode();

        return LoadImageFromTexture(texture.texture);
    }

    public void ExportAndSave(SpriteStack spriteStack, string dir)
    {
        ExportImage(Export(spriteStack), dir);
    }
}