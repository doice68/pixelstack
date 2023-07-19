public class Notification
{
    public Vector3 color;
    public string title;
    public string content;
    public float opacity = 1;
    float speed = 0.2f;
    float y = 0;
    ImGuiWindowFlags flags = 
        ImGuiWindowFlags.NoMove | 
        ImGuiWindowFlags.AlwaysAutoResize |
        ImGuiWindowFlags.NoDecoration;

    public Notification(Vector3 color, string title, string content)
    {
        this.color = color;
        this.title = title;
        this.content = content;
    }
    public void Update(int i)
    {
        if (speed > 0)
            speed -= GetFrameTime() * 0.2f;
        else
            speed = 0;

        y += speed;
        opacity -= GetFrameTime();

        ImGui.SetNextWindowBgAlpha(opacity);
        ImGui.Begin($"{title}", flags);
        ImGui.SetWindowPos(new(GetScreenWidth() / 2 - ImGui.GetWindowWidth() / 2, GetScreenHeight() - 10 - y));
        
        DrawContent();
        
        ImGui.End();
    }
    public virtual void DrawContent()
    {
        ImGui.Text(content);
    }
}
public class NotificationManager
{
    List<Notification> notifications = new();
    public void Update()
    {
        for (int i = 0; i < notifications.Count; i++)
        {
            if (notifications[i].opacity <= 0)
                notifications.RemoveAt(i);
            else
                notifications[i].Update(i);            
        }
    }
    public void Add(Notification notification)
    {
        notifications.Add(notification);
    }

}