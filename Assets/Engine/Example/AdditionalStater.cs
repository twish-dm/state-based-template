namespace StateEngine.Example
{
    using StateEngine.Behaviours;

    public class AdditionalStater : StaterBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            stater.Add(new AdditionalStaterState());
            stater.Start("AdditionalStaterState");
        }
    }
}