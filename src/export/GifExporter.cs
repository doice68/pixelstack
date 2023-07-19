using AnimatedGif;
using Image = System.Drawing.Image;

public class GifExporter : IExporter<object>
{
    public object Export(SpriteStack spriteStack)
    {
        throw new NotImplementedException();
    }

    public void ExportAndSave(SpriteStack spriteStack, string dir)
    {
        using (var gif = AnimatedGif.AnimatedGif.Create("output.gif", 5))
        {
            var scale = (int)Math.Max(spriteStack.spriteSize.X, spriteStack.spriteSize.Y);
            var rt = LoadRenderTexture(scale, scale);
 
            var sp = (SpriteStack)spriteStack.Clone();
            sp.size = new(scale, scale);
            sp.zoom = 0.5f;

            for (var i = 0; i < 360; i += 10)
            {
                sp.angle = i;
                BeginTextureMode(rt);
                ClearBackground(App.instance.previewView.previewColor.ToColor());

                sp.Render();
                EndTextureMode();

                var img = LoadImageFromTexture(rt.texture);
                ExportImage(img, "layer.png");
                
                Image gifImage = Image.FromFile("layer.png");
                gif.AddFrame(gifImage);
                
                gifImage.Dispose();
                File.Delete("layer.png");
            }
        }
    }
}