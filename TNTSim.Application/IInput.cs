namespace TNTSim.Application;

public interface IInput
{
	public bool IsKeyDown(char c);
	public bool IsKeyPressed(char c);
	public bool IsKeyReleased(char c);

	public bool IsLeftMouseDown();
	public bool IsLeftMousePressed();
	public bool IsLeftMouseReleased();

	public bool IsRightMouseDown();
	public bool IsRightMousePressed();
	public bool IsRightMouseReleased();

	public int GetScrollWheel();
}
