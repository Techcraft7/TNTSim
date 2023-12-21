namespace TNTSim.Application.Tests;

internal class TestInput : IInput
{
    private readonly Dictionary<char, bool> keys = [];
    private int scroll = 0;
    private bool leftMouse, rightMouse;

    public void ScrollUp() => scroll = 1;
    public void ScrollDown() => scroll = -1;
    public void ScrollNone() => scroll = 0;
    public void PressKey(char c) => keys[c] = true;
    public void ReleaseKey(char c) => keys[c] = false;
    public void LeftClick() => leftMouse = true;
    public void RightClick() => rightMouse = true;

    public int GetScrollWheel() => scroll;
    public bool IsKeyDown(char c) => keys.GetValueOrDefault(c);
    public bool IsKeyPressed(char c) => keys.GetValueOrDefault(c);
    public bool IsKeyReleased(char c) => !keys.GetValueOrDefault(c);
    public bool IsLeftMouseDown() => leftMouse;
    public bool IsLeftMousePressed() => leftMouse;
    public bool IsLeftMouseReleased() => !leftMouse;
    public bool IsRightMouseDown() => rightMouse;
    public bool IsRightMousePressed() => rightMouse;
    public bool IsRightMouseReleased() => !rightMouse;
}
