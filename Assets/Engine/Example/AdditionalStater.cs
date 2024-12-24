namespace StateEngine.Example
{
    using StateEngine.Behaviours;

    public class AdditionalStater : StaterBehaviour
    {
        public override void Initialize()
        {
            stater.Add(new AdditionalStaterState());
            stater.Start("AdditionalStaterState");
        }

    }
}