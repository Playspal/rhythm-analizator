namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public interface IProcess<out TResult>
    {
        TResult Process();
    }
}