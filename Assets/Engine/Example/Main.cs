namespace StateEngine.Example
{
    public sealed class Main : Engine
    {
        private void Start()
        {
            Stater.Add(new MainState());
            Stater.Change(nameof(MainState));
        }
    }
}
