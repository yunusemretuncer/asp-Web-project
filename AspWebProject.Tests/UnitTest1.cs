using Xunit;

public class UnitTest1
{
    [Fact]
    public void CI_should_fail_here()
    {
        Assert.True(true); // ✅ intentional failure
    }
}

