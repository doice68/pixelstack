using Newtonsoft.Json;

class App
{
    public static App instance;
    
    public bool exit;
    public int width = 8, height = 8;
    float roundness = 0;
    public Action onSave;
    public NotificationManager notificationManager = new();

    #region views
        List<IView> views;
        public CanvasView canvasView;
        public LayersView layersView;
        public ColorsView colorsView;
        public PreviewView previewView;
    #endregion
    
    #region popups
        List<Popup> popups = new();
        public SaveFileDialog saveFileDialog;
        public OpenFileDialog openFileDialog;
        public NewFilePopup newFilePopup;
        public ConfirmPopup exitPopup;

    #endregion

    public App()
    {
        instance = this;
        onSave = () => {};
        
        newFilePopup = new(popups);
        openFileDialog = new OpenFileDialog("*.png", popups);
        saveFileDialog = new(popups);
        exitPopup = new ConfirmPopup("Warning", "close the window?", "Yes", "No", () =>
        {
            exit = true;
        }, popups);

        LoadSettings();
        InitViews();
    }
    void LoadSettings()
    {
        if (File.Exists("Settings") == false) return;

        var jsonText = File.ReadAllText("Settings");
        Saveble.settings = JsonConvert.DeserializeObject<Settings>(jsonText);
    }
    public void InitViews()
    {   
        previewView = new(width, height);
        canvasView = new(width, height);
        layersView = new();
        colorsView = new();
        
        views = new List<IView>()
        {
            previewView,
            canvasView,
            layersView,
            colorsView,
        };

        foreach (var view in views)
        {
            view.Init();            
        }
    }
    public void NewFile()
    {
        InitViews();
        notificationManager.Add(new(Color.BLUE.ToVec(), "Created", "yayyyyy"));
    }
    void SaveFile(string dir)
    {
        GifExporter exporter = new();
        exporter.ExportAndSave(previewView.spriteStack, dir + ".gif");
        // ExportImage(image, dir);
    }
    void SaveAsSpriteSheet()
    {
        
    }

    void OpenFile(string path)
    {
        var t = LoadTexture(path);
        width = t.width;
        height = t.height;
        InitViews();
    }

    public void SaveSettings()
    {
        onSave.Invoke();
        var jsonText = JsonConvert.SerializeObject(Saveble.settings, Formatting.Indented);
        File.WriteAllTextAsync("Settings", jsonText);
    }
    
    public void LateUpdate()
    {
    }
    
    public void Update()
    {
        if (WindowShouldClose())
        {
            SaveSettings();
            exit = true;
            exitPopup.open = true;
        }
        foreach (var p in popups)
        {
            p.Update();
        }
        
        ImGui.DockSpaceOverViewport();
        
        MenuBar();
        foreach (var view in views)
        {
            view.Update();
        }
        notificationManager.Update();
    }
    void MenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl + N"))
                {
                    newFilePopup.open = true;
                }
                if (ImGui.MenuItem("Open", "Ctrl + O"))
                {
                    openFileDialog.Show(OpenFile);
                }
                if (ImGui.MenuItem("Save"))
                {
                    saveFileDialog.Show(SaveFile);
                }
                if (ImGui.MenuItem("Exit"))
                {
                    exitPopup.open = true;
                }

                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Export"))
            {
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Prefrences"))
            {
                ImGui.SliderFloat("Ui roundness", ref roundness, 0f, 10f);

                var style = ImGui.GetStyle();
                style.WindowRounding = roundness;
                style.TabRounding = roundness;
                style.FrameRounding = roundness;
                style.ScrollbarRounding = roundness;
                style.ChildRounding = roundness;
                style.GrabRounding = roundness;
                
                ImGui.EndMenu();
            }
            // if (ImGui.BeginMenu("Debug"))
            // {
            //     ImGui.Text($"fps: {GetFPS()}");
            //     ImGui.Text($"zoom: {zoom}");
            //     ImGui.EndMenu();
            // }

            ImGui.EndMainMenuBar();
        }
    }   
}