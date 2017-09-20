<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DatabaseBrowser.aspx.cs" Inherits="Sitecore.Sharedsource.Web.UI.Layouts.DatabaseBrowser" %>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Sitecore Database Browser</title>
    <link rel="stylesheet" href="/default.css" />
  </head>
  <body>
    <form id="form1" runat="server">
      <div id="content">
        <h3 runat="server" id="Heading"></h3>
        <ul runat="server" id="LinkList" />
        <table runat="server" id="Metadata" border="1" />
      </div>
    </form>
  </body>
</html>
