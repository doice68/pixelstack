public class Popup
{
    protected string name = "";
    public bool open = false;
    protected int w = 200;

    public Popup(List<Popup> popups)
    {
        popups.Add(this);
    }
    public void Update()
    {
        if (open == false) return;
        ImGui.OpenPopup(name);

        var flags = 
            ImGuiWindowFlags.NoResize | 
            ImGuiWindowFlags.NoCollapse | 
            ImGuiWindowFlags.NoDocking |
            ImGuiWindowFlags.NoMove;
            
        if (ImGui.BeginPopupModal(name, ref open, flags))
        {
            ImGui.SetWindowSize(new Vector2(w, -1));

            var x = GetScreenWidth() / 2 - ImGui.GetWindowWidth() / 2;
            var y = GetScreenHeight() / 2 - ImGui.GetWindowHeight() / 2;
            ImGui.SetWindowPos(new Vector2(x, y));


            Draw();
            ImGui.SetWindowSize(new(w, ImGui.GetCursorPosY() + ImGui.GetStyle().WindowPadding.Y));
            ImGui.EndPopup();
        }
    }
    public virtual void Draw()
    {

    }
}
class NewFilePopup : Popup
{
    public NewFilePopup(List<Popup> popups) : base(popups)
    {
        this.name = "New file";
        this.w = 300;
    }

    public override void Draw()
    {
        // this.h = ImGui.GetStyle().
        ImGui.PushItemWidth(Helpers.GetWindowWidth());
        ImGui.SliderInt("Width", ref App.instance.width, 0, 200);
        ImGui.SliderInt("Height", ref App.instance.height, 0, 200);
        ImGui.PopItemWidth();

        if (ImGui.Button("Create", new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            App.instance.NewFile();
            open = false;
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel", new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            open = false;
        }
    }
}
class ConfirmPopup : Popup
{
    string msg;
    string option1 = "Yes", option2 = "No";
    Action option1Selected;
    Action? option2Selected;

    public ConfirmPopup(string title, string msg, string option1, string option2, Action option1Selected, List<Popup> popups, Action option2Selected = null) : base(popups)
    {
        this.w = 250;

        this.name = title;
        this.msg = msg;
        this.option1 = option1;
        this.option2 = option2;
        this.option1Selected = option1Selected;
        this.option2Selected = option2Selected == null ? () => {} : option2Selected;
    }

    public override void Draw()
    {
        ImGui.Text(msg);
        if (ImGui.Button(option1, new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            option1Selected();
            open = false;
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel", new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            option2Selected();
            open = false;
        }
    }
}