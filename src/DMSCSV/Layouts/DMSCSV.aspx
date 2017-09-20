<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DMSCSV.aspx.cs" Inherits="Sitecore.Sharedsource.Web.UI.Layouts.DMSCSV" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:Literal runat="server" ID="message"></asp:Literal><br />
      <asp:CheckBox runat="server" Text="CSV" ID="csv"/> <asp:Button runat="server" Text="Submit"/><br />
      <asp:TextBox Height="800" Width="100%" runat="server" ID="query" TextMode="MultiLine"></asp:TextBox>
    </div>
    </form>
</body>
</html>
