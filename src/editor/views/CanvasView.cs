public class CanvasView : Saveble, IView
{
    public int w, h;
    float zoom = 20;

    public int currentLayer = -1;
    public RenderTexture2D canvas
    {
        get => App.instance.previewView.spriteStack.layers[currentLayer];
        set => App.instance.previewView.spriteStack.layers[currentLayer] = value;
    }
    public List<RenderTexture2D> layers => App.instance.previewView.spriteStack.layers;
    RenderTexture2D grid; bool showGrid = true;
    RenderTexture2D outline;
    RenderTexture2D transparentBg;
    Stack<RenderTexture2D> undoStack = new();
    
    public CanvasView(int w, int h)
    {
        (this.w, this.h) = (w, h);
    }
    public void Init()
    {
        OnLoad();
        AddLayer();
        DrawGrid();
        DrawOutline();
        DrawTransparentBg();
    }
    
    public CanvasView(Texture2D texture2D)
    {
        canvas = LoadRenderTexture(texture2D.width, texture2D.height);
        DrawToRenderTexture(() => 
        {
            DrawTexture(texture2D, 0, 0, Color.WHITE);
        });
        DrawGrid();
        DrawOutline();
    }
    private void DrawTransparentBg()
    {
        // transparentBg = LoadRenderTexture(w, h);
        // BeginTextureMode(transparentBg);

    }

    public void AddLayer()
    {
        var texture = LoadRenderTexture(w, h);
        layers.Add(texture);
        currentLayer++;
    }
    public void RemoveLayer()
    {
        layers.RemoveAt(currentLayer);
        if (currentLayer > layers.Count - 1)
            currentLayer--;
    }    
    public void Update()
    {
        ImGui.Begin("Canvas", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.MenuBar);
        
        ImGui.BeginMenuBar();
        Helpers.WithText("Show grid", "showgrid", () => 
        {
            ImGui.Checkbox("", ref showGrid);
        }, false);
        
        ImGui.EndMenuBar();
        ImGui.TextDisabled("Right click:  Draw \nLeft click:  Erase \nMiddle click:  Color picker");
        
        if (ImGui.IsWindowHovered())
        {
            zoom += GetMouseWheelMove();
            if (zoom < 0) zoom = 0;
        }

        var cw = (canvas.texture.width) * zoom;
        var ch = (canvas.texture.height) * zoom;

        var x = Helpers.GetWindowWidth() / 2 - cw / 2;
        var y = Helpers.GetWindowHeight() / 2 - ch / 2;
        
        DrawToCanvas(x, y);

        ImGui.SetCursorPos(new Vector2(x, y));
        if (currentLayer >= 1)
        {
            var t = layers[currentLayer - 1];
            Gui.ImageRect(t.texture, (int)cw, (int)ch, new Rectangle(0, 0, t.texture.width, -t.texture.height));
        }
        ImGui.SetCursorPos(new Vector2(x, y));
        ImGui.Image((IntPtr)outline.texture.id, new Vector2(cw, ch));


        ImGui.SetCursorPos(new Vector2(x, y));
        Gui.ImageRect(canvas.texture, (int)cw, (int)ch, new Rectangle(0, 0, canvas.texture.width, -canvas.texture.height));

        if (ImGui.IsItemHovered())
        {
            if (IsMouseButtonReleased(0))
            {
                Console.WriteLine("asfsf");
                var rt = LoadRenderTexture(w, h);
                BeginTextureMode(rt);
                DrawTexture(canvas.texture, 0, 0, Color.WHITE);
                EndTextureMode();

                undoStack.Push(rt);
            }
        }

        if (showGrid)
        {
            ImGui.SetCursorPos(new Vector2(x, y));
            ImGui.Image((IntPtr)grid.texture.id, new Vector2(cw, ch));
        }
        ImGui.End();

        if (IsKeyPressed(KeyboardKey.KEY_Z))
        {
            if (undoStack.Count != 0)
            {
                Console.WriteLine("z");
                var texture = undoStack.Pop();
                UnloadTexture(canvas.texture);
                UnloadRenderTexture(canvas);
                canvas = LoadRenderTexture(w, h);
                BeginTextureMode(canvas);
                DrawTexture(texture.texture, 0, 0, Color.WHITE);
                EndTextureMode();
            }
        }
    }
    void DrawGrid()
    {
        var scale = 25;
        var lineThickness = 2;

        int w = canvas.texture.width;
        int h = canvas.texture.height;

        grid = LoadRenderTexture(w * scale, h * scale);
        BeginTextureMode(grid);
        var c = new Color(80, 80, 80, 250);
        for (int i = 0; i < w; i++)
        {
            DrawRectangle(i * scale - lineThickness / 2, 0, lineThickness, h * scale, c);
        }
        for (int i = 0; i < h; i++)
        {
            DrawRectangle(0, i * scale - lineThickness / 2, w * scale, lineThickness, c);
        }
        DrawRectangleLines(0, 0, w * scale, h * scale, Color.WHITE);
        EndTextureMode();
    }
    
    void DrawToRenderTexture(Action action)
    {
        BeginTextureMode(canvas);
        action();
        EndTextureMode();
    }
    
    void DrawOutline()
    {
        int w = canvas.texture.width;
        int h = canvas.texture.height;

        outline = LoadRenderTexture(w * 2, h * 2);
        BeginTextureMode(outline);
        ClearBackground(new Color(255, 255, 255, 150));
        var k = 0;
        for (int i = 0; i < w * 2; i++)
        {
            for (int j = 0; j < h * 2; j++)
            {
                if (k % 2 == 0)
                    DrawPixel(i, j, new Color(100, 100, 100, 150));
                k++;
            }
            k++;
        }
        EndTextureMode();
    
    }
    void DrawToCanvas(float x, float y)
    {
        if (ImGui.IsWindowHovered() == false)
            return;

        DrawToRenderTexture(() => 
        {
            var mx = (GetMouseX() - x - ImGui.GetWindowPos().X - zoom / 2) / zoom;
            var my = (GetMouseY() - y - ImGui.GetWindowPos().Y - zoom / 2) / zoom;
            
            if (IsMouseButtonDown(0))
            {
                DrawPixel(
                    (int)Math.Round(mx), 
                    (int)Math.Round(my), 
                    App.instance.colorsView.color.ToColor());
            }
            else if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                BeginScissorMode((int)Math.Round(mx), (int)Math.Round(my), 1, 1);
                ClearBackground(Color.BLANK);
                EndScissorMode();
            }
            else if(IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_MIDDLE))
            {
                var img = LoadImageFromTexture(canvas.texture);
                ImageFlipVertical(ref img);
                App.instance.colorsView.color = GetImageColor(img, (int)Math.Round(mx), (int)Math.Round(my)).ToVec();
            }
        });
    }

    public void Duplicate()
    {
        var rt = LoadRenderTexture(w, h);
        
        BeginTextureMode(rt);
        DrawTexturePro(
            canvas.texture, 
            new(0, 0, w, -h), 
            new(0, 0, w, h),
            Vector2.Zero,
            0,
            Color.WHITE);
        EndTextureMode();

        layers.Insert(currentLayer, rt);
        currentLayer++; 
    }
}