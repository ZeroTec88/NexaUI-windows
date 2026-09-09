using System.ComponentModel;
using System.Windows.Forms;
using NexaUI.Controls;

namespace NexaUI.Tests;

[TestClass]
public sealed class DataControlApiTests
{
    // -------- NexaDataGrid --------

    [TestMethod]
    public void NexaDataGridView_Inherits_From_Native_DataGridView()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.DataGridView).IsAssignableFrom(typeof(NexaDataGridView)));
    }

    [TestMethod]
    public void NexaDataGridView_Default_Properties()
    {
        var grid = new NexaDataGridView();
        Assert.AreEqual(NexaGridStyle.Default, grid.GridStyle);
        Assert.AreEqual(NexaHeaderStyle.Standard, grid.HeaderStyle);
        Assert.IsFalse(grid.ShowRowNumbers);
        Assert.IsTrue(grid.ShowHorizontalGridLines);
        Assert.IsTrue(grid.ShowVerticalGridLines);
        Assert.IsTrue(grid.AlternateRowColors);
        Assert.AreEqual(0, grid.RowCornerRadius);
        Assert.AreEqual(32, grid.HeaderHeight);
        Assert.AreEqual(32, grid.RowHeight);
        grid.Dispose();
    }

    [TestMethod]
    public void NexaDataGridView_Style_Properties_Roundtrip()
    {
        var grid = new NexaDataGridView();
        grid.GridStyle = NexaGridStyle.Compact;
        Assert.AreEqual(NexaGridStyle.Compact, grid.GridStyle);
        grid.HeaderStyle = NexaHeaderStyle.Emphasized;
        Assert.AreEqual(NexaHeaderStyle.Emphasized, grid.HeaderStyle);
        grid.ShowRowNumbers = true;
        Assert.IsTrue(grid.ShowRowNumbers);
        grid.RowCornerRadius = 6;
        Assert.AreEqual(6, grid.RowCornerRadius);
        grid.HeaderHeight = 40;
        Assert.AreEqual(40, grid.HeaderHeight);
        grid.RowHeight = 28;
        Assert.AreEqual(28, grid.RowHeight);
        grid.Dispose();
    }

    [TestMethod]
    public void NexaDataGridView_ThemeSwitch_Does_Not_Throw()
    {
        var grid = new NexaDataGridView();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            grid.Dispose();
        }
    }

    [TestMethod]
    public void NexaDataGridView_Dispose_Does_Not_Throw()
    {
        var grid = new NexaDataGridView();
        grid.Dispose();
        grid.Dispose(); // Double dispose safe
    }

    // -------- NexaListView --------

    [TestMethod]
    public void NexaListView_Inherits_From_Native_ListView()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.ListView).IsAssignableFrom(typeof(NexaListView)));
    }

    [TestMethod]
    public void NexaListView_Default_Properties()
    {
        var list = new NexaListView();
        Assert.AreEqual(NexaListStyle.Default, list.ListStyle);
        Assert.IsTrue(list.FullRowSelect);
        Assert.IsFalse(list.CheckBoxes);
        list.Dispose();
    }

    [TestMethod]
    public void NexaListView_ListStyle_Roundtrips()
    {
        var list = new NexaListView();
        list.ListStyle = NexaListStyle.Spacious;
        Assert.AreEqual(NexaListStyle.Spacious, list.ListStyle);
        list.ListStyle = NexaListStyle.Compact;
        Assert.AreEqual(NexaListStyle.Compact, list.ListStyle);
        list.Dispose();
    }

    [TestMethod]
    public void NexaListView_ThemeSwitch_Does_Not_Throw()
    {
        var list = new NexaListView();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            list.Dispose();
        }
    }

    [TestMethod]
    public void NexaListView_Dispose_Does_Not_Throw()
    {
        var list = new NexaListView();
        list.Dispose();
        list.Dispose();
    }

    // -------- NexaPropertyGrid --------

    [TestMethod]
    public void NexaPropertyGrid_Inherits_From_Native_PropertyGrid()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.PropertyGrid).IsAssignableFrom(typeof(NexaPropertyGrid)));
    }

    [TestMethod]
    public void NexaPropertyGrid_Default_Properties()
    {
        var grid = new NexaPropertyGrid();
        Assert.AreEqual(NexaPropertyGridStyle.Default, grid.GridStyle);
        Assert.IsTrue(grid.ToolbarVisible);
        Assert.IsTrue(grid.HelpVisible);
        Assert.IsTrue(grid.CommandsVisibleIfAvailable);
        Assert.AreEqual(PropertySort.Categorized, grid.PropertySort);
        grid.Dispose();
    }

    [TestMethod]
    public void NexaPropertyGrid_GridStyle_Roundtrips()
    {
        var grid = new NexaPropertyGrid();
        grid.GridStyle = NexaPropertyGridStyle.Comfortable;
        Assert.AreEqual(NexaPropertyGridStyle.Comfortable, grid.GridStyle);
        grid.GridStyle = NexaPropertyGridStyle.Compact;
        Assert.AreEqual(NexaPropertyGridStyle.Compact, grid.GridStyle);
        grid.Dispose();
    }

    [TestMethod]
    public void NexaPropertyGrid_SelectedObject_Roundtrips()
    {
        var grid = new NexaPropertyGrid();
        var obj = new object();
        grid.SelectedObject = obj;
        Assert.AreSame(obj, grid.SelectedObject);
        grid.Dispose();
    }

    [TestMethod]
    public void NexaPropertyGrid_ThemeSwitch_Does_Not_Throw()
    {
        var grid = new NexaPropertyGrid();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            grid.Dispose();
        }
    }

    [TestMethod]
    public void NexaPropertyGrid_Dispose_Does_Not_Throw()
    {
        var grid = new NexaPropertyGrid();
        grid.Dispose();
        grid.Dispose();
    }

    // -------- NexaTreeView --------

    [TestMethod]
    public void NexaTreeView_Inherits_From_Native_TreeView()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.TreeView).IsAssignableFrom(typeof(NexaTreeView)));
    }

    [TestMethod]
    public void NexaTreeView_Default_Properties()
    {
        var tree = new NexaTreeView();
        Assert.AreEqual(NexaTreeStyle.Default, tree.TreeStyle);
        Assert.IsTrue(tree.ShowRootLines);
        Assert.IsTrue(tree.ShowNodeLines);
        Assert.IsTrue(tree.ShowLines);
        Assert.IsTrue(tree.ShowPlusMinus);
        Assert.IsFalse(tree.CheckBoxes);
        tree.Dispose();
    }

    [TestMethod]
    public void NexaTreeView_Style_Properties_Roundtrip()
    {
        var tree = new NexaTreeView();
        tree.TreeStyle = NexaTreeStyle.Spacious;
        Assert.AreEqual(NexaTreeStyle.Spacious, tree.TreeStyle);
        tree.TreeStyle = NexaTreeStyle.Compact;
        Assert.AreEqual(NexaTreeStyle.Compact, tree.TreeStyle);
        tree.CheckBoxes = true;
        Assert.IsTrue(tree.CheckBoxes);
        tree.ShowRootLines = false;
        Assert.IsFalse(tree.ShowRootLines);
        tree.ShowNodeLines = false;
        Assert.IsFalse(tree.ShowNodeLines);
        tree.Dispose();
    }

    [TestMethod]
    public void NexaTreeView_Node_Operations()
    {
        var tree = new NexaTreeView();
        var root = tree.Nodes.Add("Root");
        var child = root.Nodes.Add("Child");
        Assert.AreEqual(1, tree.Nodes.Count);
        Assert.AreEqual(1, root.Nodes.Count);
        Assert.AreEqual("Root", tree.Nodes[0].Text);
        Assert.AreEqual("Child", tree.Nodes[0].Nodes[0].Text);
        tree.Dispose();
    }

    [TestMethod]
    public void NexaTreeView_ThemeSwitch_Does_Not_Throw()
    {
        var tree = new NexaTreeView();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            tree.Dispose();
        }
    }

    [TestMethod]
    public void NexaTreeView_Dispose_Does_Not_Throw()
    {
        var tree = new NexaTreeView();
        tree.Dispose();
        tree.Dispose();
    }

    // -------- Cross-control / regression --------

    [TestMethod]
    public void DataControls_All_Dispose_Cleanly()
    {
        var grid = new NexaDataGridView();
        var list = new NexaListView();
        var prop = new NexaPropertyGrid();
        var tree = new NexaTreeView();
        grid.Dispose();
        list.Dispose();
        prop.Dispose();
        tree.Dispose();
    }
}
