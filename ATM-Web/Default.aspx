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
                                <asp:Button runat="server" CssClass="btn btn-default" Text="Back" OnClick="ToHome_Click" />
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
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Deposit" OnClick="InitDeposit_Click" />
                        </div>
                        <div class="row" style="padding:5px">
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Withdraw" OnClick="InitWithdraw_Click"/>
                        </div>
                        <div class="row" style="padding:5px">
                            <asp:Button runat="server" CssClass="btn btn-lg btn-default" Width="200px" Text="Balance Inquiry" OnClick="InitInquiry_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    
    <asp:Panel id="formDeposit" runat="server" Visible="false" CssClass="container">
        <div class="row" style="padding-top:20px">
            <div class="col-md-8 col-md-offset-2">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Deposit
                    </div>
                    <div class="panel-body text-center">
                        <asp:Panel id="formDepositError" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Label id="lblDepositError" runat="server" Text="Message"></asp:Label>
                        </asp:Panel>
                        <asp:Panel id="formDepositSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Label id="lblDepositSuccess" runat="server" Text="Deposit successful."></asp:Label>
                        </asp:Panel>
                        <form>
                            <div class="form-group">
                                <label for="amountDepositText">Amount to Deposit</label>
                                <asp:TextBox runat="server" CssClass="form-control" id="amountDepositText" />
                            </div>
                            
                            <asp:Button runat="server" CssClass="btn btn-default" Text="Back" OnClick="ToHome_Click" />
                            <asp:Button runat="server" CssClass="btn btn-default" Text="Deposit" OnClick="Deposit_Click" />
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel id="formWithdraw" runat="server" Visible="false" CssClass="container">
        <div class="row" style="padding-top:20px">
            <div class="col-md-8 col-md-offset-2">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Withdraw
                    </div>
                    <div class="panel-body">
                        <asp:Panel id="formWithdrawError" runat="server" Visible="false" CssClass="alert alert-danger">
                            <asp:Label id="lblWithdrawError" runat="server" Text="Message"></asp:Label>
                        </asp:Panel>
                        <asp:Panel id="formWithdrawSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                            <asp:Label id="lblWithdrawSuccess" runat="server" Text="Withdrawal successful."></asp:Label>
                        </asp:Panel>
                        <div class="container-fluid text-center">
                            <div class="col-md-4 col-md-offset-4">
                                <div class="row" style="padding-bottom:5px">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            <asp:Button runat="server" CssClass="btn btn-default" Text="-" OnClick="Withdraw_Decr" />
                                        </span>
                                        <asp:Textbox runat="server" CssClass="form-control" id="amountWithdrawText" Enabled="false" Text = "20" />
                                        <span class="input-group-btn">
                                            <asp:Button runat="server" CssClass="btn btn-default" Text="+" OnClick="Withdraw_Incr" />
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:Button runat="server" CssClass="btn btn-default" Text="Back" OnClick="ToHome_Click" />
                                    <asp:Button runat="server" CssClass="btn btn-default" Text="Withdraw" OnClick="Withdraw_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
   
    <asp:Panel id="formInquiry" runat="server" Visible="false" CssClass="container">
        <div class="row" style="padding-top:20px">
            <div class="col-md-8 col-md-offset-2">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Balance Inquiry
                    </div>
                    <div class="panel-body text-center">
                        <div class="row">
                            <label for="balanceLbl">Balance</label>
                        </div>
                        <div class="row">
                            <asp:Label runat="server" id="balanceLbl" Text="Balance" />
                        </div>
                        <div class="row">
                            <asp:Button runat="server" CssClass="btn btn-default" Text="Back" OnClick="ToHome_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    
</asp:Content>



