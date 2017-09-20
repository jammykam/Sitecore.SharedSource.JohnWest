namespace Sitecore.Sharedsource
{
  using System.IO;

  using DocumentFormat.OpenXml.Packaging;

  using Sitecore.Sharedsource.Pipelines.RenderWord;

  using Sitecore.Data.Items;
  using Sitecore.Pipelines;

  public static class WordUtil
  {
    /// <summary>
    /// Process the individual Word document from the FDA field.
    /// </summary>
    /// <param name="textBlock">The text block item to update with data from the Word document.</param>
    /// <param name="mediaItem">Media item representing the Word document.</param>
    public static void HandleWordDocument(Item textBlock, MediaItem mediaItem)
    {
      Stream word = null;

      // Trying to ensure the stream gets closed
      try
      {
        word = mediaItem.GetMediaStream();

        using (WordprocessingDocument wordDoc =
          WordprocessingDocument.Open(word, false /*isEditable*/))
        {
          RenderWordArgs args = new RenderWordArgs(
            mediaItem,
            wordDoc, 
            textBlock.Database,
            mediaItem.Name,
            textBlock.Fields[TextBlockUtil.TitleField],
            textBlock.Fields[TextBlockUtil.TokenMapField],
            textBlock.Fields[TextBlockUtil.HtmlField],
            textBlock.Paths.FullPath.Replace("/sitecore/content", string.Empty));
          CorePipeline.Run("renderWord", args);
        }
      }
      finally
      {
        if (word != null)
        {
          word.Close();
        }
      }
    }
  }
}