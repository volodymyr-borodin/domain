using Domain;
using NUnit.Framework;

namespace Boberneprotiv.Domain.Tests;

[TestFixture]
public class DomainResultTest
{
    [Test]
    public void DomainResult_Should_Be_Success_When_No_Errors()
    {
        var result = DomainResult.Success;

        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void DomainResult_Should_Be_Failure_When_Errors_Exist()
    {
        var result = DomainResult.Error("Error");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Error", result.Message);
    }

    [Test]
    public void DomainResult_Const_Message_Merge()
    {
        var result1 = DomainResult.Error("Error 1");
        var result2 = DomainResult.Error("Error 2");

        var result = DomainResult.MergeErrors(new []{result1, result2}, DomainResult.ConstMessageStrategy(), DomainResult.FirstDetailsStrategy);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("There are several problems", result.Message);
    }

    [Test]
    public void DomainResult_Concat_Message_Merge()
    {
        var result1 = DomainResult.Error("Error 1");
        var result2 = DomainResult.Error("Error 2");

        var result = DomainResult.MergeErrors(new []{result1, result2}, DomainResult.ConcatMessageStrategy(), DomainResult.FirstDetailsStrategy);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Error 1. Error 2", result.Message);
    }

    [Test]
    public void DomainResult_First_Details_Merge()
    {
        var result1 = DomainResult.Error("Error 1", new Dictionary<string, object>
        {
            ["key1"] = "value1"
        });
        var result2 = DomainResult.Error("Error 2", new Dictionary<string, object>
        {
            ["key1"] = "value2"
        });

        var result = DomainResult.MergeErrors(new []{result1, result2}, DomainResult.ConstMessageStrategy(), DomainResult.FirstDetailsStrategy);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("There are several problems", result.Message);
        Assert.AreEqual("value1", result.Details["key1"]);
    }

    [Test]
    public void DomainResult_Last_Details_Merge()
    {
        var result1 = DomainResult.Error("Error 1", new Dictionary<string, object>
        {
            ["key1"] = "value1"
        });
        var result2 = DomainResult.Error("Error 2", new Dictionary<string, object>
        {
            ["key1"] = "value2"
        });

        var result = DomainResult.MergeErrors(new []{result1, result2}, DomainResult.ConstMessageStrategy(), DomainResult.LastDetailsStrategy);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("There are several problems", result.Message);
        Assert.AreEqual("value2", result.Details["key1"]);
    }

    [Test]
    public void DomainResult_List_Details_Merge()
    {
        var result1 = DomainResult.Error("Error 1", new Dictionary<string, object>
        {
            ["key1"] = "value1"
        });
        var result2 = DomainResult.Error("Error 2", new Dictionary<string, object>
        {
            ["key1"] = "value2"
        });

        var result = DomainResult.MergeErrors(new []{result1, result2}, DomainResult.ConstMessageStrategy(), DomainResult.ListDetailsStrategy);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("There are several problems", result.Message);
        Assert.AreEqual(new List<string> { "value1", "value2" }, result.Details["key1"]);
    }
}
