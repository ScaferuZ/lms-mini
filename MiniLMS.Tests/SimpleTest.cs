using Xunit;

namespace MiniLMS.Tests;

public class SimpleTest
{
    [Fact]
    public void Simple_Test_Should_Pass()
    {
        // Arrange
        var expected = 2;
        
        // Act
        var actual = 1 + 1;
        
        // Assert
        Assert.Equal(expected, actual);
    }
}
