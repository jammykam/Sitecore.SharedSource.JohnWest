namespace Removed.Components.ContentSearch.ComputedFields
{
  using System.Data.SqlClient;
  using Sitecore.Analytics.Data.DataAccess.DataAdapters;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  public class HitCount : TopicComputedField
  {
    private readonly object locker = new object();
    private SqlConnection Connection { get; set; }
    private SqlCommand Command { get; set; }

    protected override object GetDefaultValue()
    {
      return 0;
    }

    protected override object GetComputedFieldValue(Item item)
    {
      lock (locker)
      {
        string itemId = item.ID.ToString().Trim('{', '}');
        PrepareSqlCommandToExecute(itemId);

        try
        {
          OpenConnection();
          int hitCount = ExecuteCommand();
          CloseConnection();

          return hitCount;
        }
        catch (SqlException ex)
        {
          LogError(ex.Message);
          return 0;
        }
        finally
        {
          CloseConnection();
        }
      }
    }

    protected virtual void PrepareSqlCommandToExecute(string itemId)
    {
      Connection = new SqlConnection(DataAdapterManager.ConnectionStrings.Analytics);
      string cmdText = string.Format("SELECT count('x') FROM Pages WHERE ItemId = '{0}' GROUP BY ItemId", itemId);
      Command = new SqlCommand(cmdText, Connection);
    }

    protected virtual void OpenConnection()
    {
      Connection.Open();
    }

    protected virtual void CloseConnection()
    {
      Connection.Close();
    }

    protected virtual int ExecuteCommand()
    {
      return (int)Command.ExecuteScalar();
    }

    protected virtual void LogError(string message)
    {
      Log.Error(message, this);
    }
  }
}