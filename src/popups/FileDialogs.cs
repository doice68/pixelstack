public class FilesRendrer : Popup
{
    protected string[] drives;
    protected int currentDrive = 0;
    protected string[] dir;
    public string currentDir = "";

    public FilesRendrer(List<Popup> popups) : base(popups)
    {
        drives = Directory.GetLogicalDrives();
        dir = Directory.GetDirectories(drives[currentDrive]);
        GoToPath(drives[0]);
    }

    public override void Draw()
    {   
        ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);
        // ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgHovered]);

        ImGui.ArrowButton("##back", ImGuiDir.Left);
        ImGui.PopStyleColor();
        
        ImGui.SameLine();     
        ImGui.Text(currentDir);
        
        ImGui.BeginListBox("##drives", new(100, 0));
        for (int i = 0; i < drives.Length; i++)
        {
            if (ImGui.Selectable(drives[i]))
            {
                GoToPath(drives[i]);
            }
        }
        ImGui.EndListBox();

        ImGui.SameLine();
        ImGui.BeginListBox("##files", new(-1, 0));
        if (ImGui.Selectable(".."))
        {
            if (currentDir != "")
            {
                var path = Directory.GetParent(currentDir);
                if (path != null)
                    GoToPath(path.FullName);
            }            
        }
        ListRender();
        ImGui.EndListBox();
    }
    public virtual void ListRender(){}
    public virtual void OnDriveChanged()
    {
        dir = Directory.GetDirectories(drives[currentDrive]);            
    }
    protected virtual void GoToPath(string name)
    {
        try
        {
            dir = Directory.GetDirectories(name);
            currentDir = name;
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

public class SaveFileDialog : FilesRendrer
{
    Action<string> onSave;
    string fileName = "";

    public SaveFileDialog(List<Popup> popups) : base(popups)
    {
        this.name = "Save";
        this.w = 500;
    }

    public void Show(Action<string> onSave)
    {
        this.onSave = onSave;
        open = true;
    }

    public override void ListRender()
    {
        for (int i = 0; i < dir.Count(); i++)
        {
            if (ImGui.Selectable($"{dir[i]}"))
            {
                GoToPath(dir[i]);
            }
        }
    }
    public override void Draw()
    {
        base.Draw();

        Helpers.WithText("File name", "filename", () => 
        {
            ImGui.SetNextItemWidth(Helpers.GetWindowWidth() - ImGui.CalcTextSize("File name").X - ImGui.GetStyle().WindowPadding.X);
            ImGui.InputText("", ref fileName, 100);
        }, false);
        
        if (ImGui.Button("Save", new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            onSave(currentDir + "/" + fileName);
            open = false;
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel", new Vector2(Helpers.GetWindowWidth() / 2 - 5, 0)))
        {
            open = false;
        }
    }
}
public class OpenFileDialog : FilesRendrer
{
    string[] files;
    string fileFormat = "*";
    Action<string> onOpen;
    public OpenFileDialog(string fileFormat, List<Popup> popups) : base(popups)
    {
        this.name = "Open";
        this.w = 500;
        this.fileFormat = fileFormat;
        this.files = Directory.GetFiles(drives[currentDrive], fileFormat);
    }
    public void Show(Action<string> onOpen)
    {
        this.onOpen = onOpen;
        open = true;
    }
    public override void OnDriveChanged()
    {
        base.OnDriveChanged();
        files = Directory.GetFiles(drives[currentDrive] + "\\" + currentDir, fileFormat);
    }
    public override void ListRender()
    {
        for (int i = 0; i < dir.Count(); i++)
        {
            if (ImGui.Selectable($"{dir[i]}"))
            {
                GoToPath(dir[i]);
            }
        }
        for (int i = 0; i < files.Count(); i++)
        {
            var fileName = files[i].Substring(files[i].LastIndexOf('\\') + 1);

            if (ImGui.Selectable($"{fileName}"))
            {
                onOpen(files[i]);
            }
        }
    }
    protected override void GoToPath(string name)
    {
        try
        {
            files = Directory.GetFiles(name, fileFormat);
            base.GoToPath(name);
        }
        catch (UnauthorizedAccessException)
        {
            
        }
    }
}

