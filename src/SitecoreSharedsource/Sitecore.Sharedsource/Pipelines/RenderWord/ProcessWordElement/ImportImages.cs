namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using System;
  using System.IO;

  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Resources.Media;

  public class ImportImages : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Database, "args.Database");
      Assert.ArgumentNotNull(args.Element, "args.Element");
      Assert.ArgumentNotNull(args.Path, "args.Path");
      Assert.ArgumentNotNull(args.Name, "args.Name");

      if (args.Element.Name != "img")
      {
        return;
      }

      MediaCreatorOptions options = new MediaCreatorOptions();
      options.Database = args.Database;
      string src = args.Element.GetAttributeValue("src", string.Empty);
      Assert.IsNotNullOrEmpty(src, "src");
      string name = src;
      int i = name.IndexOf('/');

      if (i > -1)
      {
        name = name.Substring(i + 1);
      }

      i = name.IndexOf('\\');

      if (i > -1)
      {
        name = name.Substring(i + 1);
      }

      i = name.LastIndexOf('.');

      if (i > -1)
      {
        name = name.Substring(0, i);
      }

      ////TODO: determine and implement logic to determine path to new media item
      Item mediaFolder = args.Database.GetItem("/sitecore/media library/images");
      Assert.IsTrue(mediaFolder != null, "mediaFolder");
      MediaItem image = null;

      // path to file on disk
//      string fullPath = MainUtil.MapPath(Sitecore.IO.TempFolder.Folder + "/" + src);
      string fullPath = src;

      // attempt to locate existing media item with same binary content
      Stream compareFile = null;
      Stream compareMedia = null;

      // Trying to ensure the streams get closed
      try
      {
        compareFile = File.OpenRead(fullPath);

        foreach (Item item in args.Database.SelectItems(
          "fast:" + mediaFolder.Paths.FullPath + "//*[@size='" + compareFile.Length + "']"))
        {
          compareFile.Seek(0, SeekOrigin.Begin);
          MediaItem compare = new MediaItem(item);
          compareMedia = compare.GetMediaStream();

          do
          {
            if (compareMedia.ReadByte() != compareFile.ReadByte())
            {
              break;
            }
          }
          while (compareMedia.Position != compareMedia.Length && compareFile.Position != compareFile.Length);

          if (compareMedia.Position == compareMedia.Length)
          {
            image = compare;
            ////TODO: these lines are redundant. If it's OK to check Position on a closed stream, 
            //// move these lines before this if condition and remove the duplication that follows
            compareMedia.Close();
            compareMedia = null;
            break;
          }

          compareMedia.Close();
          compareMedia = null;
        }
      }
      finally
      {
        if (compareFile != null)
        {
          compareFile.Close();
        }

        if (compareMedia != null)
        {
          compareMedia.Close();
        }
      }

      // if a matching image media item does not exist, create it
      if (image == null)
      {
        if (name.Contains("/"))
        {
          name = name.Substring(name.LastIndexOf("/") + 1);
        }

        if (name.Contains("\\"))
        {
          name = name.Substring(name.LastIndexOf("\\") + 1);
        }

        name = args.Name + "." + name + "." + new Random().Next();

        ////TODO: determine and implement logic to determine path and name for new media item
        options.Destination = mediaFolder.Paths.FullPath + '/'
          + args.Path + '/'
          + name.Replace('.', '-');
        image = new MediaCreator().CreateFromFile(fullPath, options);
      }

      args.Element.SetAttributeValue("src", "~/media/" + image.ID.ToShortID() + ".ashx");
    }
  }
}