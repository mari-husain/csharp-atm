<%@ Page Language="C#" MasterPageFile="~/Master.master" Inherits="ATMWeb.Default" Title="My ATM" EnableEventValidation="false" %>

<asp:Content id="Content" ContentPlaceHolderID="Main" runat="server">
    

    <asp:Panel id="formLogin" runat="server" Visible="true" CssClass="container">
            <div class="row" style="padding-top:20px">
                <div class="col-md-8 col-md-offset-2">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <asp:Panel id="formLoginError" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Label id="lblLoginError" runat="server" Text="Message"></asp:Label>
                            </asp:Panel>
                            <form>
                                <div class="form-group">
                                    <label for="firstNameText">First Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="firstNameText" />
                                </div>
                                <div class="form-group">
                                    <label for="lastNameText">Last Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="lastNameText" />
                                </div>
                                <div class="form-group">
                                    <label for="PINText">PIN</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="PINText" />
                                </div>
                                <asp:Button runat="server" CssClass="btn btn-default" id="Login" Text="Login" OnClick="Login_Click"></asp:Button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
    </asp:Panel>

    <asp:Panel id="formTransaction" runat="server" Visible="false" CssClass="container">
        <div class="row">
            <div class="col-md-8 col-md-offset-2">
                <div class="panel panel-default">
                    <div class="panel-body">
                        Transaction
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>



