public static class Helpers
{
    static int trimLength = 45;
    public static void Clone(this RenderTexture2D texture, RenderTexture2D other)
    {
        BeginTextureMode(texture);
        DrawTexture(other.texture, 0, 0, Color.WHITE);
        ClearBackground(Color.BLACK);
        EndTextureMode();        
    }
    public static Vector3 LerpTo(this Vector3 self, Vector3 dest, float by)
    {
        return new Vector3(Lerp(self.X, dest.X, by), Lerp(self.Y, dest.Y, by), Lerp(self.Z, dest.Z, by));
    }
    public static float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }
    public static float GetWindowWidth()
    {
        return ImGui.GetWindowWidth() - ImGui.GetStyle().WindowPadding.X * 2;
    }
    public static float GetWindowHeight()
    {
        return ImGui.GetWindowHeight() - ImGui.GetStyle().WindowPadding.Y * 2;
    }
    public static void WithText(string text, string id, Action action, bool adjustY = false)
    {
        var h = ImGui.CalcTextSize("a").Y;
        
        if (adjustY)
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + h / 2);
        ImGui.Text(text);

        ImGui.SameLine();
        if (adjustY)
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - h / 2);
        
        ImGui.PushID(id);
        action();
        ImGui.PopID();
    }
    public static void CenteredText(string text)
    {
        var txt = text.Length > trimLength ? text.Substring(0, trimLength) + "..." : text;
        var WindowW = ImGui.GetWindowWidth();
        var textW = ImGui.CalcTextSize(txt).X;
        ImGui.SetCursorPosX((WindowW - textW) * 0.5f);
        ImGui.Text(txt);
    }
    public static string TimeFormat(int time) {
        string hr, min;
        min = Convert.ToString(time % 60);
        hr = Convert.ToString(time / 60);
        if (hr.Length == 1) hr = "0" + hr;
        if (min.Length == 1) min = "0" + min;
        return hr + ":" + min;
    }
    public static Color ToColor(this Vector3 color)
    {
        var x = Math.Clamp((int)(color.X * 255), 0, 255);
        var y = Math.Clamp((int)(color.Y * 255), 0, 255);
        var z = Math.Clamp((int)(color.Z * 255), 0, 255);

        return new Color(x, y, z, 255);
    }
    public static Vector3 ToVec(this Color color)
    {
        return new Vector3(color.r / 255f, color.g / 255f, color.b / 255f);
    } 
    public static class Easing
    {
        // Adapted from source : http://www.robertpenner.com/easing/

        public static float Ease(float linearStep, float acceleration, EasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) : 
                              acceleration < 0 ? EaseOut(linearStep, type) : 
                              (float) linearStep;

            return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
        }

        public static float EaseIn(float linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseIn(linearStep);
                case EasingType.Quadratic:  return Power.EaseIn(linearStep, 2);
                case EasingType.Cubic:      return Power.EaseIn(linearStep, 3);
                case EasingType.Quartic:    return Power.EaseIn(linearStep, 4);
                case EasingType.Quintic:    return Power.EaseIn(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(float linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseOut(linearStep);
                case EasingType.Quadratic:  return Power.EaseOut(linearStep, 2);
                case EasingType.Cubic:      return Power.EaseOut(linearStep, 3);
                case EasingType.Quartic:    return Power.EaseOut(linearStep, 4);
                case EasingType.Quintic:    return Power.EaseOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(float linearStep, EasingType easeInType, EasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }
        public static float EaseInOut(float linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step:       return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear:     return (float)linearStep;
                case EasingType.Sine:       return Sine.EaseInOut(linearStep);
                case EasingType.Quadratic:  return Power.EaseInOut(linearStep, 2);
                case EasingType.Cubic:      return Power.EaseInOut(linearStep, 3);
                case EasingType.Quartic:    return Power.EaseInOut(linearStep, 4);
                case EasingType.Quintic:    return Power.EaseInOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        static class Sine
        {
            public static float EaseIn(float s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }
            public static float EaseOut(float s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi);
            }
            public static float EaseInOut(float s)
            {
                return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }

        static class Power
        {
            public static float EaseIn(float s, int power)
            {
                return (float)Math.Pow(s, power);
            }
            public static float EaseOut(float s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }
            public static float EaseInOut(float s, int power)
            {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }

    public enum EasingType
    {
        Step,
        Linear,
        Sine,
        Quadratic,
        Cubic,
        Quartic,
        Quintic
    }
    
    public static class MathHelper
    {
        public const float Pi = (float)Math.PI;
        public const float HalfPi = (float)(Math.PI / 2);

        public static float Lerp(float from, float to, float step)
        {
            return (float)((to - from) * step + from);
        }
    }
}