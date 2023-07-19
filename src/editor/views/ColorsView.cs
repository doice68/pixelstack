public class ColorsView : Saveble, IView
{
    public Vector3 color = Vector3.One;
    public Vector3[] colors = new Vector3[]
    {
        Color.BLUE.ToVec(),
        Color.GOLD.ToVec(),
        Color.PURPLE.ToVec(),
        Color.BROWN.ToVec()
    };
    public ColorsView()
    {
    }

    public void Init()
    {
        OnLoad();
    }

    public override void OnLoad()
    {
        colors = settings.colors;
    }
    public override void OnSave()
    {
        settings.colors = colors;
    }
    void LoadPallete(string dir)
    {
        var img = LoadImage(dir);
        List<Vector3> c = new();
        for (int i = 0; i < img.height; i++)
        {
            for (int k = 0; k < img.width; k++)
            {
                c.Add(GetImageColor(img, k, i).ToVec());
            }    
        }
        colors = c.ToArray();
    }

    public void Update()
    {        
        ImGui.Begin("Colors");
        ImGui.ColorPicker3("color", ref color);
        
        ImGui.Dummy(new(0, 20));
        
        if (ImGui.Button("Load pallete"))
        {
            App.instance.openFileDialog.Show(LoadPallete);
        }

        //not important for now        
        // ImGui.SameLine();
        // ImGui.Button("+");

        ImGui.BeginChild("colors", new(-1f, -1f), true);
        float v = 30 + ImGui.GetStyle().FramePadding.X;
        float xPos = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            if (color == colors[i])
            {
                var padding = 5;
                var x = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new Vector2(padding, padding);
                var y = ImGui.GetWindowPos() + ImGui.GetCursorPos() + new Vector2(30 + padding, 30 + padding);
                ImGui.GetWindowDrawList()
                .AddRect(x, y, ImGui.ColorConvertFloat4ToU32(new(1, 1, 1, 1)), 0, ImDrawFlags.None, 1.5f);
            }
            var colorButtonClicked = ImGui.ColorButton(
                $"##color{i}",
                new Vector4(colors[i], i),
                ImGuiColorEditFlags.None,
                new(30, 30));

            if (colorButtonClicked)
            {
                color = colors[i];
                Console.WriteLine("p");
            }
            
            xPos += v;
            if (ImGui.GetContentRegionAvail().X > xPos)
                ImGui.SameLine();
            else
                xPos = 0;
        }
        ImGui.EndChild();

        ImGui.End();    
    }
}