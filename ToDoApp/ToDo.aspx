<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="ToDo.aspx.cs" Inherits="ToDoApp.ToDo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ToDo App</title>

    <link href="~/Content/style.css" rel="stylesheet" />
    <%--<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>--%>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script src="https://cdn.jsdelivr.net/gh/ajmalafzal/jscolor@2.4.5/jscolor.min.js"></script>

    <script src="<%= ResolveUrl("~/Scripts/ToDo.js") %>"></script>
</head>
<body>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <asp:HiddenField ID="hfEditItemId" runat="server" />
        <asp:HiddenField ID="hfIsEdit" runat="server" Value="false" />
        <asp:HiddenField ID="hdnSelectedColor" runat="server" />
        <div id="page-wrap">
            <div id="header">
                <h1><a href="#">ToDo Application</a></h1>
            </div>

            <div id="main">


                <ul id="list">
                    <asp:UpdatePanel ID="upList" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Repeater ID="rptItems" runat="server" OnItemCommand="rptItems_ItemCommand">
                                <ItemTemplate>
                                    <li id='<%# Eval("ItemId") %>'
                                        data-itemid='<%# Eval("ItemId") %>'
                                        data-order='<%# Eval("DisplayOrder") %>' class="todo-tile">

                                        <span id='task_<%# Eval("ItemId") %>'
                                            class="task-text <%# (Convert.ToBoolean(Eval("IsDone"))) ? "done" : "" %>"
                                            data-id='<%# Eval("ItemId") %>'
                                            style='background-color: <%# SafeColor(Eval("ItemColor")) %>;'>
                                            <%# Eval("ItemText") %>
                                        </span>

                                        <div class="draggertab tab">
                                        </div>

                                        <div class="colortab tab" title="Change color">
                                            <div class="color-icon" aria-hidden="true"></div>

                                            <input type="color"
                                                class="hidden-color"
                                                data-itemid='<%# Eval("ItemId") %>'
                                                value='<%# SafeColor(Eval("ItemColor")) %>'
                                                style="display: none;" />
                                            <asp:HiddenField ID="hfItemColor" runat="server" Value='<%# SafeColor(Eval("ItemColor")) %>' />
                                            <asp:LinkButton ID="lnkChangeColor" runat="server" Style="display: none;"
                                                CommandName="ChangeColor" CommandArgument='<%# Eval("ItemId") %>' />
                                        </div>

                                        <div class="deletetab tab">
                                            <asp:LinkButton ID="lnkDelete" runat="server" Style="display: none;"
                                                CommandName="DeleteItem" CommandArgument='<%# Eval("ItemId") %>' />
                                        </div>

                                        <div class="donetab tab">

                                            <asp:LinkButton ID="lnkToggleDone" runat="server"
                                                Style="display: none;"
                                                CommandName="ToggleDone"
                                                CommandArgument='<%# Eval("ItemId") %>'
                                                Enabled='<%# !(Convert.ToBoolean(Eval("IsDone"))) %>'/>
                                            <asp:HiddenField ID="hfToggleDoneUniqueId" runat="server"
                                                Value='<%# ((LinkButton)Container.FindControl("lnkToggleDone")).UniqueID %>' />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:HiddenField ID="hfOrderData" runat="server" />
                            <asp:Button ID="btnUpdateOrder" runat="server" Style="display: none;" OnClick="btnUpdateOrder_Click" />
                        </ContentTemplate>
                    </asp:UpdatePanel>





                </ul>

                <!-- Add New Item -->
                <div id="add-new" class="add-item-section">
                    <div>
                        <asp:TextBox ID="txtNewItem" runat="server" CssClass="txtItem" />
                        <asp:Button ID="btnAddItem" runat="server" Text="Add" CssClass="button btnAddItem" OnClick="btnAddItem_Click" />
                    </div>
                    <asp:Label ID="lblError" runat="server"
                        ForeColor="Red"
                        Font-Size="12px"
                        Style="display: block; margin-top: 20px; margin-left: 300px;"></asp:Label>
                </div>

            </div>
        </div>
        <script>

            var hfOrderDataId = "<%= hfOrderData.ClientID %>";
            var btnUpdateOrderUniqueId = "<%= btnUpdateOrder.UniqueID %>";


            $(document).ready(function () {

                $(document).on("click", ".donetab", function () {
                    if ($(this).siblings(".deletetab").hasClass("confirm"))
                    {
                        $(this).siblings(".deletetab").find("a")[0].click();
                    }
                    else {
                            var $li = $(this).closest("li");
                            var btn = $li.find("a[id*='lnkToggleDone']")[0];
                            if (btn) btn.click(); 
                    }
                });

                $(document).on("dblclick", ".task-text", function () {
                    var itemId = $(this).data("id");
                    var text = $(this).text().trim();
                    var $txtBox = $("#<%= txtNewItem.ClientID %>");
                    $txtBox.val(text);
                    $("#<%= hfEditItemId.ClientID %>").val(itemId);
                    $("#<%= hfIsEdit.ClientID %>").val("true");
                    $("#<%= btnAddItem.ClientID %>").val("Update");
                    $txtBox.focus().select();
                });

                $(document).on("click", ".colortab", function () {
                    var $colorInput = $(this).find(".hidden-color");
                    var offset = $(this).offset();
                    $colorInput.css({
                        "position": "absolute",
                        "z-index": 9999,
                        "display": "block",   
                        "opacity": 0,         
                        "width": "30px",
                        "height": "30px",
                        "cursor": "pointer"
                    })[0].click();

                    $colorInput.on("blur", function () {
                        $(this).css("display", "none");
                    });
                });

                $(document).on("change", ".hidden-color", function () {
                    var color = $(this).val();
                    var $li = $(this).closest("li");
                    var itemId = $li.data("itemid");
                    $("#task_" + itemId).css("background-color", color);
                    var hf = $li.find("input[type=hidden][id*='hfItemColor']");
                    hf.val(color);
                    hf.trigger('change');
                    var btn = $li.find("a[id*='lnkChangeColor']")[0];
                    if (btn) btn.click();

                });
            });

            var $txt = $("#<%= txtNewItem.ClientID %>");
            var $btn = $("#<%= btnAddItem.ClientID %>");

            function toggleButtonState() {
                if ($txt.val().trim().length > 0) {
                    $btn.css({
                        "background": "url(Content/images/button-bg.png) repeat-x",
                        "opacity": "1",
                        "cursor": "pointer"
                    });
                } else {
                    $btn.css({
                        "background": "transparent",
                        "opacity": "0.5",     
                        "cursor": "default"   
                    });
                }
            }
            toggleButtonState();
            $txt.on("input", toggleButtonState);

            $(document).on("keydown", function (e) {
                if (e.key === "Escape" || e.keyCode === 27) {
                    $("#<%= txtNewItem.ClientID %>").val("");           
                    $("#<%= hfEditItemId.ClientID %>").val("");            
                    $("#<%= hfIsEdit.ClientID %>").val("false");          
                    $("#<%= btnAddItem.ClientID %>").val("Add");           
                    toggleButtonState();                                  
                }
            });

        </script>
        <script type="text/javascript">
            function initSortable() {
                if ($("#list").data("ui-sortable")) {
                    try { $("#list").sortable("destroy"); } catch (e)
                    {
                        console.log(e);
                    }
                }

                $("#list").sortable({
                    handle: ".draggertab",
                    items: "li",
                    placeholder: "sortable-placeholder",
                    update: function () {
                        var ids = [];
                        $("#list li").each(function () {
                            ids.push($(this).data("itemid"));
                        });

                        $("#<%= hfOrderData.ClientID %>").val(ids.join(","));

                        __doPostBack("<%= btnUpdateOrder.UniqueID %>", "");
                    }
                }).disableSelection();
            }

            $(function () {
                initSortable();
            });

            if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initSortable();
                });
            }
        </script>

    </form>
</body>
</html>
