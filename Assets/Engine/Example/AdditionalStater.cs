using StateEngine.Behaviours;

namespace StateEngine.Example
{
    public sealed class AdditionalStater : StaterBehaviour
    {
        protected override void OnInitialize()
        {
            Stater.Add(new AdditionalStaterState());
            Stater.Change(nameof(AdditionalStaterState));
        }
    }
}
