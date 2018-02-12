<%@ Page Language="C#" MasterPageFile="~/Master.master" Inherits="ATMWeb.Default" %>

<asp:Content id="Content" ContentPlaceHolderID="Main" runat="server">
    <asp:Panel id="formLogin" Visible="true" runat="server">
        Login
    </asp:Panel>

    <asp:Panel id="formTransaction" runat="server" Visible="false">
        Transaction
    </asp:Panel>
</asp:Content>



