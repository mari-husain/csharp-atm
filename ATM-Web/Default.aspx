<%@ Page Language="C#" MasterPageFile="~/Master.master" Inherits="ATMWeb.Default" Title="My ATM" EnableEventValidation="false" %>

<asp:Content id="Content" ContentPlaceHolderID="Main" runat="server">
    

    <asp:Panel id="formLogin" runat="server" Visible="true" CssClass="container">
            <div class="row" style="padding-top:20px">
                <div class="col-md-8 col-md-offset-2">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Login
                        </div>
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
                                    <asp:TextBox runat="server" CssClass="form-control" id="PINText" TextMode="Password" />
                                </div>
                                <asp:Button runat="server" CssClass="btn btn-default" Text="Login" OnClick="Login_Click"></asp:Button>
                                <asp:Button runat="server" CssClass="btn btn-default" Text="Sign Up" OnClick="Signup_Click"></asp:Button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
    </asp:Panel>

    <asp:Panel id="formSignUp" runat="server" Visible="false" CssClass="container">
            <div class="row" style="padding-top:20px">
                <div class="col-md-8 col-md-offset-2">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Sign Up
                        </div>
                        <div class="panel-body">
                            <asp:Panel id="formSignupError" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Label id="lblSignupError" runat="server" Text="Message"></asp:Label>
                            </asp:Panel>
                            <form>
                                <div class="form-group">
                                    <label for="firstNameSignupText">First Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="firstNameSignupText" />
                                </div>
                                <div class="form-group">
                                    <label for="lastNameSignupText">Last Name</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="lastNameSignupText" />
                                </div>
                                <div class="form-group">
                                    <label for="PINSignupText">PIN</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="PINSignupText" TextMode="Password" />
                                </div>
                                <div class="form-group">
                                    <label for="PINSignupConfirmText">Confirm PIN</label>
                                    <asp:TextBox runat="server" CssClass="form-control" id="PINSignupConfirmText" TextMode="Password" />
                                </div>
                                <asp:Button runat="server" CssClass="btn btn-default" Text="Back" OnClick="SignupBack_Click" />
                                <asp:Button runat="server" CssClass="btn btn-default" Text="Sign Up" OnClick="SignupSubmit_Click" />
                            </form>
                        </div>
                    </div>
                </div>
            </div>
    </asp:Panel>

    <asp:Panel id="formTransaction" runat="server" Visible="false" CssClass="container">
        <div class="row" style="padding-top:20px">
            <div class="col-md-8 col-md-offset-2">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        ATM Transaction
                    </div>
                    <div class="panel-body text-center">
                        <div class="row" style="padding:5px">
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Deposit" OnClick="SignupSubmit_Click" />
                        </div>
                        <div class="row" style="padding:5px">
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Withdraw" OnClick="SignupSubmit_Click"/>
                        </div>
                        <div class="row" style="padding:5px">
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Balance Inquiry" OnClick="SignupSubmit_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>



