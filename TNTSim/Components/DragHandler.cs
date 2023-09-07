namespace TNTSim.Components;

internal class DragHandler
{
    public bool IsDragging { get; private set; } = false;
    public int DeltaX { get; private set; }
    public int DeltaY { get; private set; }

    private int startX = 0;
    private int startY = 0;

    public void Update(Func<bool>? startCondition = null)
    {
        bool isDown = IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);

        // Start dragging on press
        if (isDown && !IsDragging && (startCondition?.Invoke() ?? true))
        {
            startX = GetMouseX();
            startY = GetMouseY();
            IsDragging = true;
        }

        // Stop dragging on release
        if (!isDown)
        {
            IsDragging = false;
        }

        // Update deltas while dragging
        if (IsDragging)
        {
            DeltaX = IsDragging ? GetMouseX() - startX : 0;
            DeltaY = IsDragging ? GetMouseY() - startY : 0;
        }
    }

    public void FakeRestartDrag()
    {
        startX += DeltaX;
        startY += DeltaY;
    }
}