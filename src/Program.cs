SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
InitWindow(1200, 700, "Stacker");

Gui.Setup(true);
MyStyle.SetupImGuiStyle();
ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;

App app = new();

while (app.exit == false)
{
    BeginDrawing();
    ClearBackground(Color.GRAY);
    Gui.Begin();
    
    app.Update();
    
    Gui.End();
    EndDrawing();
}

Gui.Shutdown();
CloseWindow();