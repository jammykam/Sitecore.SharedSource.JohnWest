namespace Removed.FakeDB.Tests.Components.ContentSearch.ComputedFields
{
  using System.Collections.Generic;
  using Removed.Components.ContentSearch.ComputedFields;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.FakeDb;

  [TestClass]
  public class HitCountTests
  {
    private Db FakeDb { get; set; }
    private ID topicId;

    [TestInitialize]
    public void TestInit()
    {
      topicId = new ID("{34D65C57-0F23-4C00-833A-C3FDB6023F8D}");

      FakeDb = new Db
      {
        new DbItem("Fake Topic", topicId)
      };

      Sitecore.Context.Database = FakeDb.Database;
    }

    [TestCleanup]
    public void CleanupTest()
    {
      if (FakeDb != null)
      {
        FakeDb.Dispose();
        FakeDb = null;
      }
    }

    [TestMethod]
    public void GetComputedFieldValue()
    {
      // Arrange
      var computedField = new FakeHitCount();
      var topic = Sitecore.Context.Database.GetItem(topicId);

      // Act
      var computedFieldValue = computedField.GetComputedFieldValue(topic);

      // Assert
      computedField.HasConnectionBeenOpened.Should().BeTrue();
      computedField.HasConnectionBeenClosed.Should().BeTrue();
      computedField.HasCommandBeenExecuted.Should().BeTrue();
      computedFieldValue.Should().Be(57);
    }
  }

  public class FakeHitCount : HitCount
  {
    public Dictionary<string, int> FakePageTable { get; set; }
    public bool HasConnectionBeenOpened { get; set; }
    public bool HasConnectionBeenClosed { get; set; }
    public bool HasCommandBeenExecuted { get; set; }
    private string ItemId { get; set; }

    public FakeHitCount()
    {
      FakePageTable = new Dictionary<string, int>
                        {
                          { "D02087E5-0CFF-48A1-8ACA-0A5AB7FFBD27", 99 },
                          { "C79B2BBC-82A4-4E07-96A2-072EC22ECA97", 11 },
                          { "34D65C57-0F23-4C00-833A-C3FDB6023F8D", 57 }
                        };

      HasConnectionBeenOpened = false;
      HasConnectionBeenClosed = false;
      HasCommandBeenExecuted = false;
    }

    public new object GetComputedFieldValue(Item item)
    {
      return base.GetComputedFieldValue(item);
    }

    protected override void PrepareSqlCommandToExecute(string itemId)
    {
      ItemId = itemId;
    }

    protected override void OpenConnection()
    {
      HasConnectionBeenOpened = true;
    }

    protected override void CloseConnection()
    {
      HasConnectionBeenClosed = true;
    }

    protected override int ExecuteCommand()
    {
      HasCommandBeenExecuted = true;

      return FakePageTable.ContainsKey(ItemId) ? FakePageTable[ItemId] : 0;
    }

    protected override void LogError(string message)
    {
      HasCommandBeenExecuted = false;
    }
  }
}
