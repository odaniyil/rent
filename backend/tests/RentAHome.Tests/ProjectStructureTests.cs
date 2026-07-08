namespace RentAHome.Tests;

public class ProjectStructureTests
{
    [Fact]
    public void Domain_Assembly_Is_Loadable()
    {
        Assert.Equal("RentAHome.Domain", typeof(RentAHome.Domain.AssemblyMarker).Assembly.GetName().Name);
    }
}
