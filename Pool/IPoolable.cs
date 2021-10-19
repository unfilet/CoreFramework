namespace CardKit.Utils
{
    public interface IPoolable
    {
        IPool Pool { get; }
        void InitializeTemplate(IPool pool);
    }
}