using AssetPass.Api.Controllers;
using Xunit;

namespace AssetPass.Api.Tests.Common;

public class ApiResponseTests
{
    [Fact]
    public void Ok_CreatesSuccessfulResponseWithData()
    {
        var data = "test-payload";
        var response = ApiResponse.Ok(data);

        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Null(response.Error);
        Assert.Empty(response.ValidationErrors);
    }

    [Fact]
    public void Fail_CreatesFailedResponseWithErrorAndValidationList()
    {
        var error = "Validation error";
        var validationErrors = new List<string> { "Field 1 is required", "Field 2 is invalid" };
        var response = ApiResponse.Fail(error, validationErrors);

        Assert.False(response.Success);
        Assert.Equal(error, response.Error);
        Assert.Equal(2, response.ValidationErrors.Count);
    }
}
