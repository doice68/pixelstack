public class LayersView : Saveble, IView
{
    string[] placementmodes = 
    {
        "Horizontal",
        "Vertical",
    };
    int placementmode = 0;
    const int horizontal = 0;
    const int vertical = 1;

    public LayersView()
    {
    }
    public void Init()
    {
        OnLoad();
    }
    public override void OnLoad()
    {
        placementmode = settings.placementmode;
    }
    public override void OnSave()
    {
        settings.placementmode = placementmode;
    }

    public void Update()
    {
        var canvasView = App.instance.canvasView;
        ImGui.Begin("Layers", ImGuiWindowFlags.HorizontalScrollbar);
        
        ImGui.SetNextItemWidth(-1);
        ImGui.Combo("###Placemenet", ref placementmode, placementmodes, placementmodes.Length);
        
        if (ImGui.Button("+"))
            canvasView.AddLayer();
        
        ImGui.SameLine();        
        if (ImGui.Button("Copy"))
            canvasView.Duplicate();
        
        ImGui.SameLine();   
        ImGui.BeginDisabled(canvasView.layers.Count == 1);     
        
        if (ImGui.Button("-"))
            canvasView.RemoveLayer();
        
        ImGui.EndDisabled();

        for (int i = 0; i < canvasView.layers.Count; i++)
        {
            ImGui.RadioButton($"##layer{i}", ref canvasView.currentLayer, i);
            if (ImGui.IsItemHovered())
            {
                var t = canvasView.layers[i].texture;
                ImGui.BeginTooltip();
                Gui.ImageRect(t, 40, 40, new(0, 0, t.width, -t.height));
                ImGui.EndTooltip();
            }
            if (placementmode == horizontal) ImGui.SameLine();
        }
        ImGui.End();
    }
}