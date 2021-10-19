namespace CardKit.Utils
{
    public interface IPool
    {
        object GetObject();
        void Release(object gameObject);
    }
}