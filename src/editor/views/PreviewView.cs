using System.Reflection;
using System.Runtime.CompilerServices;

public class PreviewView : Saveble, IView
{
    public SpriteStack spriteStack;
    RenderTexture2D previewTexture;
    bool rotate = false;
    float rotation = 45;
    float rotationSpeed = 30;
    public Vector3 previewColor  = Vector3.Zero;
    bool showCurrentLayer;
    private bool mouseDown;

    public PreviewView(int width, int height)
    {
        spriteStack = new(Vector2.Zero, new(width * 40, height * 40));
    }

    public void Init()
    {
        OnLoad();
    }

    public override void OnLoad()
    {
        rotate = settings.rotate;
        previewColor = settings.previewColor;
        
        spriteStack.zoom = settings.zoom;
        spriteStack.showCurrentLayer = settings.showCurrentLayer;
        spriteStack.fillInBetween = settings.fillInBetween;
        spriteStack.spread = settings.spread;
        spriteStack.squashAmount = settings.squashAmount;
    }

    public override void OnSave()
    {
        settings.rotate = rotate;
        settings.previewColor = previewColor;
        
        settings.zoom = spriteStack.zoom;
        settings.showCurrentLayer = spriteStack.showCurrentLayer;
        settings.fillInBetween = spriteStack.fillInBetween;
        settings.spread = spriteStack.spread;
        settings.squashAmount = spriteStack.squashAmount;
    }
    public void Update()
    {
        ImGui.Begin("Preview", ImGuiWindowFlags.MenuBar);
        if (spriteStack.size != ImGui.GetContentRegionAvail())
        {
            spriteStack.size = ImGui.GetContentRegionAvail();
            UnloadRenderTexture(previewTexture);
            previewTexture = LoadRenderTexture((int)spriteStack.size.X, (int)spriteStack.size.Y);
        }

        if (ImGui.IsWindowHovered())
        {
            spriteStack.zoom += GetMouseWheelMove() * 0.2f;
            spriteStack.zoom = Math.Clamp(spriteStack.zoom, 0.1f, 2);

            if (IsMouseButtonPressed(0)) 
                mouseDown = true;
        }
        
        if (IsMouseButtonUp(0)) 
            mouseDown = false;
        
        if (rotate == false && mouseDown)
                rotation += GetMouseDelta().X;
        ImGui.BeginMenuBar();

        if (ImGui.BeginMenu("Settings"))
        {
            ImGui.ColorEdit3("Bg color", ref previewColor, ImGuiColorEditFlags.NoInputs); 
            ImGui.Checkbox("Show current layer", ref showCurrentLayer);

            ImGui.Checkbox("Auto Rotate", ref rotate);
            ImGui.DragFloat("Squash", ref spriteStack.squashAmount, 0.01f, 0.1f, 1);
            ImGui.DragFloat("Zoom", ref spriteStack.zoom, 0.01f, 0.1f, 20);
            ImGui.DragInt("Spread", ref spriteStack.spread, 0.01f, 1, 30);
            ImGui.Checkbox("Fill in betweens", ref spriteStack.fillInBetween);

            ImGui.EndMenu();
        }

        ImGui.EndMenuBar();
        
        spriteStack.angle = rotate ? (float)GetTime() * rotationSpeed : rotation; 
        var canvas = App.instance.canvasView;
        
        BeginTextureMode(previewTexture);
        ClearBackground(previewColor.ToColor());
        spriteStack.Render();
        EndTextureMode();

        Gui.ImageSize(previewTexture.texture, (int)spriteStack.size.X, (int)spriteStack.size.Y);

        ImGui.End();        
    }
}