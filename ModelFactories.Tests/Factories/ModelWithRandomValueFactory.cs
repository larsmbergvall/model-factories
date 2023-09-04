using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class ModelWithRandomValueFactory : ModelFactory<ModelWithRandomValue>
{
    private Random random = new Random();

    protected override void Definition()
    {
        Property(m => m.Value, () => random.Next(Int32.MinValue, Int32.MaxValue));
    }
}
