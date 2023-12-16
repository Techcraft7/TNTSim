namespace TNTSim;

internal sealed class RaylibInput : IInput
{
	public int GetScrollWheel() => Raylib.GetMouseWheelMoveV().Y switch
	{
		< 0 => -1,
		> 0 => 1,
		_ => 0
	};

	public bool IsKeyDown(char c) => Raylib.IsKeyDown(CharToRaylibKey(c));
	public bool IsKeyPressed(char c) => Raylib.IsKeyPressed(CharToRaylibKey(c));
	public bool IsKeyReleased(char c) => Raylib.IsKeyReleased(CharToRaylibKey(c));
	
	public bool IsLeftMouseDown() => Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
	public bool IsLeftMousePressed() => Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
	public bool IsLeftMouseReleased() => Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);

	public bool IsRightMouseDown() => Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT);
	public bool IsRightMousePressed() => Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT);
	public bool IsRightMouseReleased() => Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT);

	private static KeyboardKey CharToRaylibKey(char c) => c switch
	{
		'\n' => KeyboardKey.KEY_ENTER,
		'\t' => KeyboardKey.KEY_TAB,
		'\x7F' => KeyboardKey.KEY_BACKSPACE,
		_ => (KeyboardKey)c
	};
}
