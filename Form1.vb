Imports EdmLib
Imports System.Collections.Generic

Imports Microsoft.WindowsAPICodePack
Imports Microsoft.WindowsAPICodePack.Shell
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.IO.Compression


Public Class Form1
    Dim vault As IEdmVault11 = New EdmVault5
    'Dim collectfiles() As String
    Dim fileCollectionOnlyParent As New List(Of String)
    Dim fileCollectionInDepth As New List(Of String)
    Dim fileCollectionOnlyChanged As New List(Of String)
    Dim changesTxt As New List(Of String)
    Dim resetSettings As Boolean = False

    Private Function GetWindowsPreviewBitMap(strFullFilePath As String) As System.Drawing.Bitmap
        Dim BitMapResult As System.Drawing.Bitmap = Nothing
        Dim ShellItem As ShellObject = Nothing
        If System.IO.File.Exists(strFullFilePath) Then
            ShellItem = ShellFile.FromFilePath(strFullFilePath)
            BitMapResult = ShellItem.Thumbnail.ExtraLargeBitmap
        End If
        Return BitMapResult
    End Function

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If RememverColumnWidthAndOrderToolStripMenuItem.Checked = True And resetSettings = False Then
            'ListView1
            My.Settings.List1Order = ""
            My.Settings.List1Width = ""
            For i = 0 To LViewWUsed.Columns.Count - 1
                My.Settings.List1Order = My.Settings.List1Order & LViewWUsed.Columns.Item(i).DisplayIndex & ","
                My.Settings.List1Width = My.Settings.List1Width & LViewWUsed.Columns.Item(i).Width & ","
            Next
            'ListView2
            My.Settings.List2Order = ""
            My.Settings.List2Width = ""
            For i = 0 To LViewSResults.Columns.Count - 1
                My.Settings.List2Order = My.Settings.List2Order & LViewSResults.Columns.Item(i).DisplayIndex & ","
                My.Settings.List2Width = My.Settings.List2Width & LViewSResults.Columns.Item(i).Width & ","
            Next
            'ListView3
            My.Settings.List3Order = ""
            My.Settings.List3Width = ""
            For i = 0 To LViewCollect.Columns.Count - 1
                My.Settings.List3Order = My.Settings.List3Order & LViewCollect.Columns.Item(i).DisplayIndex & ","
                My.Settings.List3Width = My.Settings.List3Width & LViewCollect.Columns.Item(i).Width & ","
            Next
            'ListView4
            My.Settings.List4Order = ""
            My.Settings.List4Width = ""
            For i = 0 To LViewHistory.Columns.Count - 1
                My.Settings.List4Order = My.Settings.List4Order & LViewHistory.Columns.Item(i).DisplayIndex & ","
                My.Settings.List4Width = My.Settings.List4Width & LViewHistory.Columns.Item(i).Width & ","
            Next
            'ListView5
            My.Settings.List5Order = ""
            My.Settings.List5Width = ""
            For i = 0 To LViewContains.Columns.Count - 1
                My.Settings.List5Order = My.Settings.List5Order & LViewContains.Columns.Item(i).DisplayIndex & ","
                My.Settings.List5Width = My.Settings.List5Width & LViewContains.Columns.Item(i).Width & ","
            Next
            'TabControl2
            My.Settings.TabControl2 = ""
            For Each tb As TabPage In TabControl2.TabPages
                My.Settings.TabControl2 = My.Settings.TabControl2 & tb.Name & ","
            Next
            'TabControl3
            My.Settings.TabControl3 = ""
            For Each tb As TabPage In TabControl3.TabPages
                My.Settings.TabControl3 = My.Settings.TabControl3 & tb.Name & ","
            Next
        End If
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        'Splash Screen με ιστορικό αλλαγών.
        If e.Control = True And e.KeyCode = Keys.I Then
            showaboutbox()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim views() As EdmViewInfo = {}
        'ComboLanguage.Items.Add("")
        ComboLanguage.Items.Add("GR")
        ComboLanguage.Items.Add("EN")
        ComboLanguage.SelectedIndex = 0
        TextboxCollectPath.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\MDC"
        vault.GetVaultViews(views, False)
        For Each View As EdmViewInfo In views
            LoginToVaultToolStripMenuItem.DropDownItems.Add(View.mbsVaultName)
        Next
        DateTimePicker1.Text = Now.Date()
        MenuItem1.Checked = My.Settings.Menu1
        MenuItem2.Checked = My.Settings.Menu2
        MenuItem3.Checked = My.Settings.Menu3
        MenuItem4.Checked = My.Settings.Menu4
        MenuItem5.Checked = My.Settings.Menu5
        MenuItem6.Checked = My.Settings.Menu6
        CheckBox3.Checked = My.Settings.Comments
        CheckBox4.Checked = My.Settings.RevComments
        AutoUpdatePreviewToolStripMenuItem.Checked = My.Settings.UpdatePreview
        SplitContainer3.Panel2Collapsed = My.Settings.SplitContainer

        RememverColumnWidthAndOrderToolStripMenuItem.Checked = My.Settings.KeepListChanges
        Dim columnorder As String()
        Dim columnwidth As String()
        Dim taborder As String()
        'ListView1
        columnorder = My.Settings.List1Order.Split(",")
        columnwidth = My.Settings.List1Width.Split(",")
        If columnorder.Length > 1 Then
            For i = 0 To LViewWUsed.Columns.Count - 1
                LViewWUsed.Columns.Item(i).DisplayIndex = columnorder(i)
            Next
        End If
        If columnwidth.Length > 1 Then
            For i = 0 To LViewWUsed.Columns.Count - 1
                LViewWUsed.Columns.Item(i).Width = columnwidth(i)
            Next
        End If
        'ListView2
        columnorder = My.Settings.List2Order.Split(",")
        columnwidth = My.Settings.List2Width.Split(",")
        If columnorder.Length > 1 Then
            For i = 0 To LViewSResults.Columns.Count - 1
                LViewSResults.Columns.Item(i).DisplayIndex = columnorder(i)
            Next
        End If
        If columnwidth.Length > 1 Then
            For i = 0 To LViewSResults.Columns.Count - 1
                LViewSResults.Columns.Item(i).Width = columnwidth(i)
            Next
        End If
        'ListView3
        columnorder = My.Settings.List3Order.Split(",")
        columnwidth = My.Settings.List3Width.Split(",")
        If columnorder.Length > 1 Then
            For i = 0 To LViewCollect.Columns.Count - 1
                LViewCollect.Columns.Item(i).DisplayIndex = columnorder(i)
            Next
        End If
        If columnwidth.Length > 1 Then
            For i = 0 To LViewCollect.Columns.Count - 1
                LViewCollect.Columns.Item(i).Width = columnwidth(i)
            Next
        End If
        'ListView4
        columnorder = My.Settings.List4Order.Split(",")
        columnwidth = My.Settings.List4Width.Split(",")
        If columnorder.Length > 1 Then
            For i = 0 To LViewHistory.Columns.Count - 1
                LViewHistory.Columns.Item(i).DisplayIndex = columnorder(i)
            Next
        End If
        If columnwidth.Length > 1 Then
            For i = 0 To LViewHistory.Columns.Count - 1
                LViewHistory.Columns.Item(i).Width = columnwidth(i)
            Next
        End If
        'ListView5
        columnorder = My.Settings.List5Order.Split(",")
        columnwidth = My.Settings.List5Width.Split(",")
        If columnorder.Length > 1 Then
            For i = 0 To LViewContains.Columns.Count - 1
                LViewContains.Columns.Item(i).DisplayIndex = columnorder(i)
            Next
        End If
        If columnwidth.Length > 1 Then
            For i = 0 To LViewContains.Columns.Count - 1
                LViewContains.Columns.Item(i).Width = columnwidth(i)
            Next
        End If
        'TabControl2
        taborder = My.Settings.TabControl3.Split(",")
        If taborder.Length > 1 Then
            For Each tb As TabPage In TabControl2.TabPages
                For Each st As String In taborder
                    If tb.Name = st Then
                        TabControl3.TabPages.Add(tb)
                    End If
                Next
            Next
        End If
        'TabControl3
        taborder = My.Settings.TabControl2.Split(",")
        If taborder.Length > 1 Then
            For Each tb As TabPage In TabControl3.TabPages
                For Each st As String In taborder
                    If tb.Name = st Then
                        TabControl2.TabPages.Add(tb)
                    End If
                Next
            Next
        End If


        Me.Text = "Manufacturing Data Collector - V" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor '& "." & My.Application.Info.Version.Revision

        'Αυτόματο login με την είσοδο στο πρόγραμμα
        If LoginToVaultToolStripMenuItem.DropDownItems.Count > 0 Then
            vault.LoginAuto(LoginToVaultToolStripMenuItem.DropDownItems.Item(0).Text, Me.Handle.ToInt32())
            If vault.IsLoggedIn = True Then
                LoginToVaultToolStripMenuItem.Enabled = False
                ToolStripStatusLabel2.Text = "Logged in to " & vault.Name
            Else
                ToolStripStatusLabel2.Text = "Not Logged in"
                MsgBox("Not logged in", , "Login to Vault")
            End If
        End If

        Me.PartNo.Select()
        Me.ActiveControl = PartNo
    End Sub

    Private Function UserInGroup(groupname As String) As Boolean
        Dim userID As Integer
        Dim check As Boolean
        check = False
        userID = vault.GetLoggedInWindowsUserID(vault.Name)
        If userID <> 0 Then
            Dim userMgr As IEdmUserMgr7
            userMgr = vault.CreateUtility(EdmUtility.EdmUtil_UserMgr)
            Dim user As IEdmUser8
            user = userMgr.GetUser(vault.GetObject(EdmObjectType.EdmObject_User, userID).Name)
            'Get the groups he is member of
            Dim groups() As Object
            groups = Nothing
            user.GetGroupMemberships(groups)
            'Display a message box with the group names
            'Dim message As String
            'message = "Groups:" + vbLf
            Dim i As Integer
            i = LBound(groups)
            While (i <= UBound(groups))
                Dim group As IEdmUserGroup7
                group = groups(i)
                'message = message + group.Name + vbLf
                If group.Name = groupname Then
                    check = True
                End If
                i = i + 1
            End While
            'MsgBox(message)
        End If
        Return check
    End Function


    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Me.Show()
        Dim returnValue, list As String()
        Dim check As Boolean
        check = False
        'Ανάγνωση εξωτερικών παραμέτρων
        'returnValue = Environment.GetCommandLineArgs() 'Ανάγνωση των command line arguments
        returnValue = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData
        If Not returnValue Is Nothing Then
            'if returnValue.lenght > 0 then
            list = returnValue(0).Split(",")
            PartNo.Text = list(0)
            populatePartno()
            PartNo.Enabled = False
            For Each item As ListViewItem In LViewSResults.Items
                If item.Text = list(0) And item.SubItems.Item(1).Text = list(1) Then
                    item.Selected = True
                    check = True
                    Exit For
                End If
            Next
            If check = True Then
                DisplayTree()
                LViewSResults.Enabled = False
            End If
        End If

        If UserInGroup("Engineering - Staff") = False Then
            CheckBox4.Visible = False
            CheckBox4.Enabled = False
        End If

    End Sub

    Private Sub Search_Click(sender As Object, e As EventArgs) Handles Search.Click
        'On Error GoTo ErrHand
        If PartNo.Text = "" Then
            Exit Sub
        End If
        Me.Cursor = Cursors.AppStarting
        populatePartno()
        Me.Cursor = Cursors.Arrow
        Exit Sub
ErrHand:
        Dim ename As String
        Dim edesc As String
        Me.Cursor = Cursors.Arrow
        vault.GetErrorString(Err.Number, ename, edesc)
        MsgBox(ename + vbLf + edesc)

    End Sub

    Private Sub populatePartno()
        Dim search As IEdmSearch5
        Dim file As IEdmFile5
        Dim item As New ListViewItem()
        Dim temp(5) As String
        Dim check As Boolean
        Dim verEnum As IEdmEnumeratorVersion5
        Dim pos As IEdmPos5
        Dim ver As IEdmRevision5
        Dim folder As IEdmFolder5
        folder = Nothing
        LViewSResults.Items.Clear()
        LViewWUsed.Items.Clear()
        LViewContains.Items.Clear()
        search = vault.CreateSearch
        search.AddVariable("PartNo", "%" & PartNo.Text)

        If MenuItem3.Checked = True Then
            search.State = "In Sync With ERP"
        End If
        search.FindHistoricStates = True
        Dim result, previousresult As IEdmSearchResult5
        previousresult = Nothing
        result = search.GetFirstResult
        While Not result Is Nothing
            check = True
            file = result
            If Not previousresult Is Nothing Then
                If result.Path = previousresult.Path Then
                    check = False
                End If
            End If
            previousresult = result
            Select Case System.IO.Path.GetExtension(result.Name).ToLower
                Case ".sldprt", ".sldasm"
                    file.GetEnumeratorVariable.GetVar("PartNo", "@", temp(0))
                    file.GetEnumeratorVariable.GetVar("Revision", "@", temp(1))
                    file.GetEnumeratorVariable.GetVar("Description", "@", temp(2))
                Case Else
                    result = search.GetNextResult
                    Continue While
            End Select
            For Each listitem As ListViewItem In LViewSResults.Items
                'MenuItem 5
                If temp(0) Like listitem.Text And temp(1) Like listitem.SubItems.Item(1).Text And temp(2) Like listitem.SubItems.Item(2).Text Then
                    check = False
                End If
            Next
            If check = True Then
                'Προσθέτω και τα revision στη λίστα
                verEnum = file
                pos = verEnum.GetFirstRevisionPosition
                Dim poEnum As IEdmEnumeratorVariable7
                Dim oVarData As EdmGetVarData
                Dim aoVars() As Object
                Dim aoCfgs() As String
                Dim iVarIdx As Integer
                Dim poVal As IEdmVariableValue6
                While Not pos.IsNull
                    item = New ListViewItem()
                    item.Text = temp(0)
                    ver = verEnum.GetNextRevision(pos)
                    poEnum = file.GetEnumeratorVariable
                    folder = vault.GetFolderFromPath(System.IO.Path.GetDirectoryName(result.Path))
                    poEnum.GetVersionVars(ver.VersionNo, folder.ID, aoVars, aoCfgs, oVarData)
                    iVarIdx = LBound(aoVars)
                    While (iVarIdx <= UBound(aoVars))
                        poVal = aoVars(iVarIdx)
                        Select Case poVal.VariableName
                            Case "Revision"
                                item.SubItems.Add(poVal.GetValue("@"))
                                item.SubItems.Add(ver.VersionNo)
                            Case "Description"
                                item.SubItems.Add(poVal.GetValue("@"))
                            Case "Description English"
                                item.SubItems.Add(poVal.GetValue("@"))
                                item.SubItems.Add(result.Path)
                        End Select
                        iVarIdx = iVarIdx + 1
                    End While
                    'MenuItem 5
                    If MenuItem5.Checked = True Then
                        For Each listitem As ListViewItem In LViewSResults.Items
                            If item.Text Like listitem.Text And item.SubItems.Item(1).Text Like listitem.SubItems.Item(1).Text Then
                                LViewSResults.Items.Remove(listitem)
                            End If
                        Next
                    End If
                    LViewSResults.Items.Insert(0, item)
                End While

            End If
            result = search.GetNextResult
        End While
    End Sub

    Private Sub DisplayTree()
        'On Error GoTo ErrHand
        If PartNo.Text = "" Then
            Exit Sub
        End If
        Me.Cursor = Cursors.AppStarting

        LViewHistory.Items.Clear()
        LViewCollect.Items.Clear()
        fileCollectionInDepth.Clear()
        fileCollectionOnlyParent.Clear()
        fileCollectionOnlyChanged.Clear()
        changesTxt.Clear()

        showPreview(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text)

        PopulateWhereUsed(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text)
        PopulateContains(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text)
        PopulateComments(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text)
        PopulateTree(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text)


        Me.Cursor = Cursors.Arrow
        Exit Sub
ErrHand:
        Dim ename As String
        Dim edesc As String
        vault.GetErrorString(Err.Number, ename, edesc)
        Me.Cursor = Cursors.Arrow
        MsgBox(ename + vbLf + edesc)
    End Sub

    Private Sub PopulateWhereUsed(path As String, ver As String)
        Dim file As IEdmFile5
        Dim folder As IEdmFolder5
        Dim ref As IEdmReference5
        Dim pos As IEdmPos5
        Dim item As New ListViewItem()
        If path = "" Then
            Exit Sub
        End If
        folder = Nothing
        LViewWUsed.Items.Clear()
        file = vault.GetFileFromPath(path, folder)
        ref = file.GetReferenceTree(folder.ID, ver)
        pos = ref.GetFirstParentPosition(ref.VersionRef, False)
        While Not pos.IsNull
            ref = ref.GetNextParent(pos)
            If UserInGroup("Engineering - Staff") Or (System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldasm" Or System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldprt") Then
                item = New ListViewItem()
                item.Text = ref.Name
                item.SubItems.Add(getVariable(ref, "PartNo"))
                item.SubItems.Add(getVariable(ref, "Revision"))
                item.SubItems.Add(ref.VersionRef & "/" & ref.File.CurrentVersion)
                item.SubItems.Add(getVariable(ref, "Description"))
                item.SubItems.Add(getVariable(ref, "Description English"))
                item.SubItems.Add(ref.FoundPath)
                LViewWUsed.Items.Add(item)
            End If
        End While
    End Sub

    Private Sub PopulateContains(path As String, ver As String)
        Dim file As IEdmFile5
        Dim folder As IEdmFolder5
        Dim ref As IEdmReference5
        Dim pos As IEdmPos5
        Dim item As New ListViewItem()
        Dim project As String
        project = ""
        If path = "" Then
            Exit Sub
        End If
        folder = Nothing
        LViewContains.Items.Clear()
        file = vault.GetFileFromPath(path, folder)
        ref = file.GetReferenceTree(folder.ID, ver)
        pos = ref.GetFirstChildPosition(project, False, True, 0)
        While Not pos.IsNull
            ref = ref.GetNextChild(pos)
            'If System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldasm" Or System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldprt" Then
            item = New ListViewItem()
            item.Text = ref.Name
            item.SubItems.Add(getVariable(ref, "PartNo"))
            item.SubItems.Add(getVariable(ref, "Revision"))
            item.SubItems.Add(ref.VersionRef & "/" & ref.File.CurrentVersion)
            item.SubItems.Add(getVariable(ref, "Description"))
            item.SubItems.Add(getVariable(ref, "Description English"))
            item.SubItems.Add(ref.FoundPath)
            LViewContains.Items.Add(item)
            'End If
        End While
    End Sub

    Private Sub PopulateComments(path As String, version As String)
        LViewHistory.Items.Clear()
        LViewHistory.Groups.Clear()
        Dim file As IEdmFile5
        Dim item As New ListViewItem()
        Dim verEnum As IEdmEnumeratorVersion5
        Dim pos, pos2 As IEdmPos5
        Dim ver As IEdmVersion5
        Dim rev As IEdmRevision5
        Dim folder As IEdmFolder5
        Dim revs As New List(Of Integer)
        Dim temp, groupid
        groupid = 0
        temp = ""
        folder = Nothing
        file = vault.GetFileFromPath(path, folder)
        verEnum = file

        pos = verEnum.GetFirstVersionPosition
        While Not pos.IsNull
            item = New ListViewItem()
            ver = verEnum.GetNextVersion(pos)
            item.Text = ver.VersionNo
            item.SubItems.Add(ver.Comment.Replace(vbCrLf, " "))
            item.SubItems.Add(userName(ver.UserID))
            item.SubItems.Add(ver.VersionDate)
            If Not (CheckBox3.Checked = False And ver.Comment = "") Then
                If (UserInGroup("Engineering - Staff") And CheckBox4.Checked = False) Or commentLanguage(ver.Comment, "Check") = "True" Then
                    If commentLanguage(item.SubItems(1).Text, "Check") = "True" Then
                        item.SubItems(1).Text = commentLanguage(item.SubItems(1).Text, "ALL")
                        item.Font = New Font(item.Font, FontStyle.Bold)
                    End If
                    LViewHistory.Items.Insert(0, item)
                End If
            End If
        End While
        pos2 = verEnum.GetFirstRevisionPosition
        While Not pos2.IsNull
            rev = verEnum.GetNextRevision(pos2)
            If temp <> rev.Name Then
                revs.Add(rev.VersionNo)
                LViewHistory.Groups.Insert(0, New ListViewGroup(revisionName(rev.Name), HorizontalAlignment.Left))
                temp = rev.Name
            Else
                revs.Item(revs.Count - 1) = rev.VersionNo
            End If
        End While

        For Each tmpitem As ListViewItem In LViewHistory.Items
            For i = 0 To revs.Count - 1
                If revs.Item(i) >= tmpitem.SubItems.Item(0).Text Then
                    tmpitem.Group = LViewHistory.Groups(revs.Count - 1 - i)
                    Exit For
                End If
            Next
        Next
    End Sub

    Public Function userName(id As Object) As String
        On Error GoTo ErrHand
        Dim result
        Dim userMgr As IEdmUserMgr5
        Dim user As IEdmUser5
        userMgr = vault
        user = userMgr.GetUser(id)
        result = user.Name
        Return result

        Exit Function
ErrHand:
        'Σε περίπτωση που το id είναι χρήστη που έχει διαγραφεί και άρα το userMgr.GetUser θα επιστρέψει error
        result = "Deleted User: " & id
        Return result
    End Function

    Public Function revisionName(name As String) As String
        If name.Contains("Issue") Then
            Return name
        Else
            Return "Revision " & name
        End If
    End Function

    Private Sub PopulateTree(path As String, ver As String, Optional level As Integer = 0)
        Dim file As IEdmFile5
        Dim folder As IEdmFolder5
        Dim ref, ref2, ref3 As IEdmReference5
        Dim pos, pos2 As IEdmPos5
        Dim project As String
        Dim item, item2, item3 As New ListViewItem()
        Dim check, topParent As Boolean
        Dim check2
        topParent = False
        If path = "" Then
            Exit Sub
        End If
        folder = Nothing
        project = ""
        file = vault.GetFileFromPath(path, folder)
        ref = file.GetReferenceTree(folder.ID, ver)
        If level = 0 Then
            item = addTreeLine(ref, level)
            topParent = True
            'Αποθήκευση σχολίων αλλαγών
            collectChangesSinceDate(ref, DateTimePicker1.Text, 0)
        End If
        'Εμφανίζονται πρώτα τα σχέδια στο δέντρο
        ref2 = ref
        pos = ref2.GetFirstParentPosition(ref2.VersionRef, False)
        check = False
        check2 = "Not Show"
        While Not pos.IsNull
            ref2 = ref2.GetNextParent(pos)
            If System.IO.Path.GetExtension(ref2.File.Name).ToLower = ".sldasm" Then
                check2 = "Not Show"
            Else
                If ref2.File.CurrentState.Name = "In Sync With ERP" Or MenuItem3.Checked = False Then
                    Select Case getVariable(ref2, "Document Category")
                        Case "DRW"
                            check2 = "Show"
                        Case "3GM"
                            check2 = "Collect"
                        Case "3RP"
                            check2 = "Collect"
                        Case "CMI"
                            check2 = "Collect"
                        Case Else
                            If MenuItem4.Checked = True Then
                                check2 = "Not Show"
                            Else
                                check2 = "Show"
                            End If
                    End Select
                Else
                    check2 = "Not Show"
                End If
            End If
            If (System.IO.Path.GetExtension(ref2.File.Name).ToLower = ".slddrw" And getVariable(ref2, "Language") = ComboLanguage.Text) And (ref2.File.CurrentState.Name = "In Sync With ERP" Or MenuItem3.Checked = False) Or check2 = "Show" Or check2 = "Collect" Then
                item2 = addTreeLine(ref2, level + 1)
                If check2 = "Collect" Then
                    fileCollectionInDepth.Add(ref2.FoundPath)
                    If topParent = True Then
                        fileCollectionOnlyParent.Add(ref2.FoundPath)
                    End If
                    item2.ForeColor = Color.DarkGreen
                End If
                If ref2.VersionRef <> ref2.File.CurrentVersion And MenuItem1.Checked = True Then
                    item2.ForeColor = Color.DarkRed
                    item2.Font = New Font(item2.Font, FontStyle.Bold)
                End If
                If checkDateChanged(ref2) = False And MenuItem6.Checked = True Then
                    item2.Font = New Font(item2.Font, FontStyle.Bold)
                    If check2 = "Collect" Then
                        fileCollectionOnlyChanged.Add(ref2.FoundPath)
                    End If
                    'Αποθήκευση σχολίων αλλαγών
                    collectChangesSinceDate(ref2, DateTimePicker1.Text, 1)
                End If
                ref3 = ref2.File.GetReferenceTree(folder.ID, ref2.VersionRef)
                pos2 = ref3.GetFirstParentPosition(ref2.VersionRef, False)
                While Not pos2.IsNull
                    ref3 = ref3.GetNextParent(pos2)
                    If ref3.File.CurrentState.Name = "In Sync With ERP" Or MenuItem3.Checked = False Then
                        check = True
                        item3 = addTreeLine(ref3, level + 2)
                        fileCollectionInDepth.Add(ref3.FoundPath)
                        If topParent = True Then
                            fileCollectionOnlyParent.Add(ref3.FoundPath)
                        End If
                        If checkDateChanged(ref3) = False And MenuItem6.Checked = True Then
                            item3.Font = New Font(item3.Font, FontStyle.Bold)
                            fileCollectionOnlyChanged.Add(ref3.FoundPath)
                            'Αποθήκευση σχολίων αλλαγών
                            collectChangesSinceDate(ref3, DateTimePicker1.Text, 1)
                        End If
                        item3.ForeColor = Color.DarkGreen
                        'temp3.NodeFont = New Font(temp3.NodeFont, FontStyle.Bold)
                    End If
                End While
            End If
        End While
        If check = False And MenuItem2.Checked = True Then
            item.Font = New Font(item.Font, FontStyle.Bold)
            item.ForeColor = Color.DarkOrange
        End If

        'Έπειτα τυπώνονται τα εξαρτήματα
        pos = ref.GetFirstChildPosition(project, False, True, 0)
        While Not pos.IsNull
            ref = ref.GetNextChild(pos)
            'Εμφανίζω μόνο τα part που είναι manufactured και όχι τα purchased και virtual
            If typeName(getVariable(ref, "Component Type")) = "Manufactured" Then
                item2 = addTreeLine(ref, level + 1)
                If ref.VersionRef <> ref.File.CurrentVersion And MenuItem1.Checked = True Then
                    item2.ForeColor = Color.DarkRed
                    item2.Font = New Font(item2.Font, FontStyle.Bold)
                End If
                If checkDateChanged(ref) = False And MenuItem6.Checked = True Then
                    item2.Font = New Font(item2.Font, FontStyle.Bold)
                    'Αποθήκευση σχολίων αλλαγών
                    collectChangesSinceDate(ref, DateTimePicker1.Text, 0)
                End If
                PopulateTree(ref.FoundPath, ref.VersionRef, level + 1)
            End If
        End While

    End Sub

    Private Function addTreeLine(ref As IEdmReference5, Optional level As Integer = 0) As ListViewItem
        Dim item, listitem As New ListViewItem
        item.Text = ref.File.Name
        item.IndentCount = level
        item.SubItems.Add(getVariable(ref, "PartNo"))
        item.SubItems.Add(getVariable(ref, "Revision", ref.VersionRef))
        item.SubItems.Add(ref.VersionRef & "/" & ref.File.CurrentVersion)
        item.SubItems.Add(getVariable(ref, "Description", ref.VersionRef))
        item.SubItems.Add(getVariable(ref, "Description English", ref.VersionRef))
        item.SubItems.Add(ref.File.CurrentState.Name)
        item.SubItems.Add(typeName(getVariable(ref, "Component Type")))
        item.SubItems.Add(getVariable(ref, "Component Type Subgroup"))
        item.SubItems.Add(ref.FoundPath)
        listitem = LViewCollect.Items.Add(item)
        Return listitem
    End Function

    Private Function checkDateChanged(item As IEdmReference5) As Boolean
        Dim verEnum As IEdmEnumeratorVersion5
        Dim pos As IEdmPos5
        Dim ver As IEdmRevision5
        ver = Nothing
        verEnum = item.File
        pos = verEnum.GetFirstRevisionPosition
        While Not pos.IsNull
            ver = verEnum.GetNextRevision(pos)
        End While
        If Not ver Is Nothing Then
            If DateDiff(DateInterval.Day, Date.Parse(ver.Time), Date.Parse(DateTimePicker1.Text)) > 0 Then
                Return True
            Else
                Return False
            End If
        Else
            If typeName(getVariable(item, "Component Type")) = "Manufactured" Then
                If DateDiff(DateInterval.Day, Date.Parse(getVariable(item, "Publication Date")), Date.Parse(DateTimePicker1.Text)) > 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                If getVariable(item, "Document Category") = "DRW" Then
                    If DateDiff(DateInterval.Day, Date.Parse(getVariable(item, "Publication Date")), Date.Parse(DateTimePicker1.Text)) > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return True
                End If

            End If
        End If
    End Function

    Private Sub CollectButton_Click(sender As Object, e As EventArgs) Handles CollectButton.Click
        'On Error GoTo ErrHand
        Dim printList As New List(Of String)
        If CheckBox2.Checked = True Then
            printList = fileCollectionOnlyChanged
        Else
            If CheckBox1.Checked = True Then
                printList = fileCollectionInDepth
            Else
                printList = fileCollectionOnlyParent
            End If
        End If
        If printList.Count <= 0 Then
            MsgBox("No drawings in state 'In sync with ERP' found")
            Exit Sub
        End If
        Dim file As IEdmFile5
        Dim descr As String
        Dim folder As IEdmFolder5
        Dim save As Boolean = True
        folder = Nothing
        descr = ""
        'Γίνεται η συλλογή των αρχείων
        Dim destFolder = ""
        Dim zipFileName = ""
        file = vault.GetFileFromPath(LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text, folder)
        Select Case ComboLanguage.Text
            Case "EN"
                descr = LViewSResults.SelectedItems.Item(0).SubItems.Item(4).Text
            Case "GR"
                descr = LViewSResults.SelectedItems.Item(0).SubItems.Item(3).Text
        End Select
        destFolder = TextboxCollectPath.Text & "\" & System.IO.Path.GetFileNameWithoutExtension(file.Name) & " - Rev " & LViewSResults.SelectedItems.Item(0).SubItems.Item(1).Text & " - " & ComboLanguage.Text & " - " & descr.Replace(vbCrLf, "").Replace("\", "-").Replace("/", "-") & "\"
        zipFileName = TextboxCollectPath.Text & "\" & System.IO.Path.GetFileNameWithoutExtension(file.Name) & " - Rev " & LViewSResults.SelectedItems.Item(0).SubItems.Item(1).Text & " - " & ComboLanguage.Text & " - " & descr.Replace(vbCrLf, "").Replace("\", "-").Replace("/", "-") & ".zip"

        If System.IO.File.Exists(zipFileName) Or System.IO.Directory.Exists(destFolder) Then
            If MsgBox("Files allready exist. Save will not continue if not deleted. Do you want to replace them?", MsgBoxStyle.YesNo, "Files allready exist") = MsgBoxResult.Yes Then
                If System.IO.File.Exists(zipFileName) Then
                    My.Computer.FileSystem.DeleteFile(zipFileName)
                End If
                If System.IO.Directory.Exists(destFolder) Then
                    For Each foundFile As String In My.Computer.FileSystem.GetFiles(destFolder)
                        My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Next
                    System.IO.Directory.Delete(destFolder, True)
                End If
            Else
                save = False
            End If
        End If

        If save = True Then
            System.IO.Directory.CreateDirectory(destFolder)
            For Each temp In printList
                file = vault.GetFileFromPath(temp, folder)
                file.GetFileCopy(Me.Handle, 0)
                ' Copy the file.
                If Not System.IO.File.Exists(destFolder & file.Name) Then
                    System.IO.File.Copy(temp, destFolder & file.Name, True)
                End If
            Next

            'Γράφω τα comments σε ένα αρχείο changes.txt
            If changesTxt.Count > 1 Then
                Dim obj As New System.IO.StreamWriter(destFolder & "changes.txt", True)
                For i = 0 To changesTxt.Count - 1
                    obj.WriteLine(changesTxt(i))
                Next
                obj.Close()
            End If

            'Προσθέτω σε φάκελο αν έχει επιλεγεί
            If CheckBox5.Checked = True Then
                ZipFile.CreateFromDirectory(destFolder, zipFileName, CompressionLevel.Optimal, True)
                For Each foundFile As String In My.Computer.FileSystem.GetFiles(destFolder)
                    My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                Next
                System.IO.Directory.Delete(destFolder, True)
                MsgBox("Drawings saved in: " & zipFileName)
            Else
                MsgBox("Drawings saved in folder: " & destFolder)
            End If
        End If
        Exit Sub
ErrHand:
        Dim ename As String
        Dim edesc As String
        vault.GetErrorString(Err.Number, ename, edesc)
        MsgBox(ename + vbLf + edesc)

    End Sub

    ''' <summary>
    ''' Returns the comment in the desired language or check if comment has language
    ''' </summary>
    ''' <param name="comment">The comment as retrieved from ePDM</param>
    ''' <param name="lang ">The lang to return (EN for English, GR for Greek, ALL for all languages, RAW entire comment, Check to check</param>
    ''' <remarks></remarks>    
    Private Function commentLanguage(comment As String, lang As String) As String
        Dim check As String
        Dim temp As String
        check = "False"
        If comment <> "" Then
            Select Case lang
                Case "GR"
                    If comment.Contains("<GR>") Then
                        temp = comment.Substring(InStr(comment, "<GR>") + 3)
                        If InStr(temp, "<GR>") > 0 Then
                            temp = Strings.Left(temp, InStr(temp, "<GR>") - 1)
                        End If
                        check = temp
                    End If
                Case "EN"
                    If comment.Contains("<EN>") Then
                        temp = comment.Substring(InStr(comment, "<EN>") + 3)
                        If InStr(temp, "<EN>") > 0 Then
                            temp = Strings.Left(temp, InStr(temp, "<EN>") - 1)
                        End If
                        check = temp
                    End If
                Case "ALL"
                    If comment.Contains("<GR>") Or comment.Contains("<EN>") Then
                        temp = comment.Substring(InStr(comment, "<GR>") + 3)
                        If InStr(temp, "<GR>") > 0 Then
                            temp = Strings.Left(temp, InStr(temp, "<GR>") - 1)
                        End If
                        check = temp
                        temp = comment.Substring(InStr(comment, "<EN>") + 3)
                        If InStr(temp, "<EN>") > 0 Then
                            temp = Strings.Left(temp, InStr(temp, "<EN>") - 1)
                        End If
                        check = check & " / " & temp
                        'check = comment.Replace(vbCrLf, " ")
                    End If
                Case "RAW"
                    check = comment.Replace(vbCrLf, " ")
                Case "Check"
                    If comment.Contains("<GR>") Or comment.Contains("<EN>") Or comment.Contains("!!") Then
                        check = "True"
                    End If
            End Select
        End If
        Return check
    End Function

    ''' <summary>
    ''' Collects all marked comments that indicate changes in part and drawings
    ''' </summary>
    ''' <param name="ref">File to retrieve comments</param>
    ''' <param name="datesince ">Date from which the comments are collected</param>
    ''' <param name="type">Type of file. 0: Part, 1: Drawing</param>
    ''' <remarks></remarks>    
    Private Sub collectChangesSinceDate(ref As IEdmReference5, datesince As Date, type As Integer)
        If changesTxt.Count = 0 Then
            Select Case ComboLanguage.Text
                Case "GR"
                    changesTxt.Add("Αλλαγές από " & DateTimePicker1.Text)
                Case "EN"
                    changesTxt.Add("Changes since " & DateTimePicker1.Text)
            End Select
        End If
        Select Case type
            Case 0
                'Λαμβάνω υπόψη μόνο τα σχόλια των Parts και Assemblies
                If System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldasm" Or System.IO.Path.GetExtension(ref.File.Name).ToLower = ".sldprt" Then
                    Select Case ComboLanguage.Text
                        Case "GR"
                            changesTxt.Add(vbCrLf & "Αλλαγές στο τεμάχιο με PartNo: " & getVariable(ref, "PartNo"))
                        Case "EN"
                            changesTxt.Add(vbCrLf & "PartNo " & getVariable(ref, "PartNo") & " Changes:")
                    End Select
                    If collectCommentsSinceDate(ref.File, datesince) = False Then
                        changesTxt.RemoveAt(changesTxt.Count - 1)
                    End If
                End If
            Case 1
                'Λαμβάνω υπόψη μόνο τα σχόλια των Drawings
                If System.IO.Path.GetExtension(ref.File.Name).ToLower = ".slddrw" Then
                    Select Case ComboLanguage.Text
                        Case "GR"
                            changesTxt.Add(vbCrLf & vbTab & "Αλλαγές στο σχέδιο του τεμαχίου με PartNo: " & getVariable(ref, "PartNo"))
                        Case "EN"
                            changesTxt.Add(vbCrLf & vbTab & "Drawing Changes for PartNo " & getVariable(ref, "PartNo"))
                    End Select
                    If collectCommentsSinceDate(ref.File, datesince) = False Then
                        changesTxt.RemoveAt(changesTxt.Count - 1)
                    End If
                End If
        End Select
    End Sub

    Function collectCommentsSinceDate(file As IEdmFile5, datesince As Date) As Boolean
        Dim verEnum As IEdmEnumeratorVersion5
        Dim pos As IEdmPos5
        Dim ver As IEdmVersion5
        Dim rev As IEdmRevision5
        Dim revs, vers As New List(Of Comments)
        Dim tmpcomment As New Comments
        Dim temp, previousrev
        Dim check As Boolean
        check = False
        previousrev = -1
        temp = ""
        verEnum = file
        'Γεμίζω τη λίστα με τα Revision
        pos = verEnum.GetFirstRevisionPosition
        While Not pos.IsNull
            rev = verEnum.GetNextRevision(pos)
            If temp <> rev.Name Then
                tmpcomment = New Comments
                tmpcomment.version = rev.VersionNo
                tmpcomment.comment = rev.Name
                revs.Insert(0, tmpcomment)
                temp = rev.Name
            Else
                revs.Item(0).version = rev.VersionNo
            End If
        End While
        'Γεμίζω τη λίστα με τα Version
        pos = verEnum.GetFirstVersionPosition
        While Not pos.IsNull
            ver = verEnum.GetNextVersion(pos)
            If Date.Compare(Date.Parse(ver.VersionDate), Date.Parse(datesince)) >= 0 And commentLanguage(ver.Comment, ComboLanguage.Text) <> "False" Then
                tmpcomment = New Comments
                tmpcomment.version = ver.VersionNo
                tmpcomment.comment = commentLanguage(ver.Comment, ComboLanguage.Text)
                vers.Insert(0, tmpcomment)
            End If
        End While

        For i = 0 To vers.Count - 1
            For j = 0 To revs.Count - 2
                If revs(j).version > vers(i).version And revs(j + 1).version < vers(i).version Then
                    If j <> previousrev Then
                        previousrev = j
                        changesTxt.Add(vbTab & "From " & revisionName(revs(j + 1).comment) & " To " & revs(j).comment)
                        check = True
                    End If
                    changesTxt.Add(vbTab & vbTab & vers(i).comment)
                End If
            Next
        Next
        Return check
    End Function

    ''' <summary>
    ''' Returns the variable value of the file regardless configuration
    ''' </summary>
    ''' <param name="ref">File to retrieve variable</param>
    ''' <param name="variable">Name of the variable to be retrieved</param>
    ''' <param name="version">Version of the file. Latest version the user can see if left blank or 0</param>
    ''' <remarks></remarks>    
    Private Function getVariable(ref As IEdmReference5, variable As String, Optional version As Integer = 0) As String
        Dim temp
        temp = ""
        If version = 0 Then
            Select Case System.IO.Path.GetExtension(ref.File.Name).ToLower
                Case ".pdf", ".docm", ".docx", ".doc", ".step"
                    ref.File.GetEnumeratorVariable.GetVar(variable, "", temp)
                Case ".dxf"
                    ref.File.GetEnumeratorVariable.GetVar(variable, "model", temp)
                Case ".slddrw", ".sldprt", ".sldasm"
                    ref.File.GetEnumeratorVariable.GetVar(variable, "@", temp)
                Case Else
                    ref.File.GetEnumeratorVariable.GetVar(variable, "", temp)
            End Select
        Else
            Dim verEnum As IEdmEnumeratorVersion5
            Dim folder As IEdmFolder5
            Dim poEnum As IEdmEnumeratorVariable7
            Dim oVarData As EdmGetVarData
            Dim aoVars() As Object
            Dim aoCfgs() As String
            Dim iVarIdx As Integer
            Dim poVal As IEdmVariableValue6
            poEnum = ref.File.GetEnumeratorVariable
            folder = vault.GetFolderFromPath(System.IO.Path.GetDirectoryName(ref.FoundPath))
            poEnum.GetVersionVars(version, folder.ID, aoVars, aoCfgs, oVarData)
            iVarIdx = LBound(aoVars)
            While (iVarIdx <= UBound(aoVars))
                poVal = aoVars(iVarIdx)
                If poVal.VariableName = variable Then
                    Select Case System.IO.Path.GetExtension(ref.File.Name).ToLower
                        Case ".pdf", ".docm", ".docx", ".doc", ".step"
                            temp = poVal.GetValue("")
                        Case ".dxf"
                            temp = poVal.GetValue("model")
                        Case ".slddrw", ".sldprt", ".sldasm"
                            temp = poVal.GetValue("@")
                        Case Else
                            temp = poVal.GetValue("")
                    End Select

                End If
                iVarIdx = iVarIdx + 1
            End While
        End If
        Return temp
    End Function

    ''' <summary>
    ''' Returns Component Type Name Of Given Id
    ''' </summary>
    ''' <param name="id">Component Type Id</param>
    ''' <remarks></remarks>
    Private Function typeName(id) As String
        Select Case id
            Case "1"
                Return "Manufactured"
            Case "2"
                Return "Purchaced"
            Case "3"
                Return "Virtual"
            Case Else
                If Not id Is Nothing Then
                    Return id.ToString
                Else
                    Return ""
                End If
        End Select
    End Function

    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Browse.Click
        FolderBrowserDialog1.ShowDialog()
        If FolderBrowserDialog1.SelectedPath <> "" Then
            TextboxCollectPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub PartNo_KeyDown(sender As Object, e As KeyEventArgs) Handles PartNo.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            'Προσθέτω τα revision του part στο combobox
            Search_Click(sender, New System.EventArgs())
        End If
    End Sub

    Private Sub ListView2_Click(sender As Object, e As EventArgs) Handles LViewSResults.Click
        Dim part
        part = LViewSResults.SelectedItems.Item(0).Text
        DisplayTree()

    End Sub

    Private Sub ComboLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboLanguage.SelectedIndexChanged
        If LViewSResults.SelectedItems.Count > 0 Then
            DisplayTree()
        End If
    End Sub

    Private Sub Refresh_Click(sender As Object, e As EventArgs) Handles Refresh.Click
        If LViewSResults.SelectedItems.Count > 0 Then
            DisplayTree()
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub LViewWUsed_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles LViewWUsed.MouseDoubleClick
        PartNo.Text = LViewWUsed.SelectedItems.Item(0).SubItems.Item(1).Text
        LViewWUsed.Items.Clear()
        LViewSResults.Items.Clear()
        LViewContains.Items.Clear()
        Search_Click(sender, New System.EventArgs())

    End Sub

    Private Sub MenuItem1_Click(sender As Object, e As EventArgs) Handles MenuItem1.Click
        If MenuItem1.Checked = True Then
            MenuItem1.Checked = False
        Else
            MenuItem1.Checked = True
        End If
        My.Settings.Menu1 = MenuItem1.Checked
    End Sub

    Private Sub MenuItem2_Click(sender As Object, e As EventArgs) Handles MenuItem2.Click
        If MenuItem2.Checked = True Then
            MenuItem2.Checked = False
        Else
            MenuItem2.Checked = True
        End If
        My.Settings.Menu2 = MenuItem2.Checked
    End Sub

    Private Sub MenuItem3_Click(sender As Object, e As EventArgs) Handles MenuItem3.Click
        If MenuItem3.Checked = True Then
            MenuItem3.Checked = False
        Else
            MenuItem3.Checked = True
        End If
        My.Settings.Menu3 = MenuItem3.Checked
    End Sub

    Private Sub MenuItem4_Click(sender As Object, e As EventArgs) Handles MenuItem4.Click
        If MenuItem4.Checked = True Then
            MenuItem4.Checked = False
        Else
            MenuItem4.Checked = True
        End If
        My.Settings.Menu4 = MenuItem4.Checked
    End Sub

    Private Sub MenuItem5_Click(sender As Object, e As EventArgs) Handles MenuItem5.Click
        My.Settings.Menu5 = MenuItem5.Checked
    End Sub

    Private Sub MenuItem6_Click(sender As Object, e As EventArgs) Handles MenuItem6.Click
        If MenuItem6.Checked = True Then
            MenuItem6.Checked = False
        Else
            MenuItem6.Checked = True
        End If
        My.Settings.Menu6 = MenuItem6.Checked
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        My.Settings.Comments = CheckBox3.Checked
        If LViewCollect.SelectedItems.Count > 0 Then
            PopulateComments(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, Split(LViewCollect.SelectedItems.Item(0).SubItems(3).Text, "/")(0))
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        My.Settings.RevComments = CheckBox4.Checked
        If LViewCollect.SelectedItems.Count > 0 Then
            PopulateComments(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, Split(LViewCollect.SelectedItems.Item(0).SubItems(3).Text, "/")(0))
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        showaboutbox()
    End Sub

    Private Sub ListView3_Click(sender As Object, e As EventArgs) Handles LViewCollect.Click
        If LViewCollect.SelectedItems.Count > 0 Then
            showPreview(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, LViewCollect.SelectedItems.Item(0).SubItems(3).Text.Split("/")(0))

            PopulateWhereUsed(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, Split(LViewCollect.SelectedItems.Item(0).SubItems(3).Text, "/")(0))
            PopulateContains(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, Split(LViewCollect.SelectedItems.Item(0).SubItems(3).Text, "/")(0))
            PopulateComments(LViewCollect.SelectedItems.Item(0).SubItems(9).Text, Split(LViewCollect.SelectedItems.Item(0).SubItems(3).Text, "/")(0))
        End If
    End Sub

    Public Sub showPreview(path As String, version As String)
        If checkLocalVersion(path, version) = False Then
            If AutoUpdatePreviewToolStripMenuItem.Checked = True Then
                Dim file As IEdmFile5
                file = vault.GetFileFromPath(path)
                Dim verEnum As IEdmEnumeratorVersion5
                verEnum = file
                Dim ver As IEdmVersion5
                ver = verEnum.GetVersion(version)
                ver.GetFileCopy(Me.Handle)
                showPreview(path, version)
            Else
                Button3.Visible = True
            End If
        Else
            Button3.Visible = False
        End If
        PictureBox1.Image = ResizeImage(GetWindowsPreviewBitMap(path), PictureBox1.Size, True)
    End Sub

    Public Function checkLocalVersion(path As String, ver As String) As Boolean
        Dim file As IEdmFile5
        file = vault.GetFileFromPath(path)
        If file Is Nothing Then
            Exit Function
        End If

        If ver <> file.GetLocalVersionNo(path) Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Shared Function ResizeImage(ByVal image As Image, ByVal size As Size, Optional ByVal preserveAspectRatio As Boolean = True) As Image
        If image IsNot Nothing Then
            Dim newWidth As Integer
            Dim newHeight As Integer
            If preserveAspectRatio Then
                Dim originalWidth As Integer = image.Width
                Dim originalHeight As Integer = image.Height
                Dim percentWidth As Single = CSng(size.Width) / CSng(originalWidth)
                Dim percentHeight As Single = CSng(size.Height) / CSng(originalHeight)
                Dim percent As Single = If(percentHeight < percentWidth,
                        percentHeight, percentWidth)
                newWidth = CInt(originalWidth * percent)
                newHeight = CInt(originalHeight * percent)
            Else
                newWidth = size.Width
                newHeight = size.Height
            End If
            Dim newImage As Image = New Bitmap(newWidth, newHeight)
            Using graphicsHandle As Graphics = Graphics.FromImage(newImage)
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight)
            End Using
            Return newImage
        Else : Return Nothing
        End If
    End Function

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            CheckBox1.Checked = True
            CheckBox1.Enabled = False
        Else
            CheckBox1.Enabled = True
        End If
    End Sub


    Private Sub LoginToVaultToolStripMenuItem_DropDownItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles LoginToVaultToolStripMenuItem.DropDownItemClicked
        vault.LoginAuto(e.ClickedItem.Text, Me.Handle.ToInt32())
        If vault.IsLoggedIn = True Then
            LoginToVaultToolStripMenuItem.Enabled = False
            ToolStripStatusLabel2.Text = "Logged in to " & vault.Name
        Else
            ToolStripStatusLabel2.Text = "Not Logged in"
            MsgBox("Δεν έχετε κάνει login", , "Login to Vault")
        End If
    End Sub

    Private Sub RememverColumnWidthAndOrderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RememverColumnWidthAndOrderToolStripMenuItem.Click
        My.Settings.KeepListChanges = RememverColumnWidthAndOrderToolStripMenuItem.Checked
    End Sub

    Private Sub ResetAllColumnWidthAndOrderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetAllColumnWidthAndOrderToolStripMenuItem.Click
        resetSettings = True
        My.Settings.List1Order = ""
        My.Settings.List1Width = ""
        My.Settings.List2Order = ""
        My.Settings.List2Width = ""
        My.Settings.List3Order = ""
        My.Settings.List3Width = ""
        My.Settings.List4Order = ""
        My.Settings.List4Width = ""
        My.Settings.List5Order = ""
        My.Settings.List5Width = ""
        My.Settings.TabControl2 = ""
        My.Settings.TabControl3 = ""
        My.Settings.SplitContainer = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim message
        message = ""
        For i = 0 To changesTxt.Count - 1
            message = message & changesTxt(i) & vbCrLf
        Next
        'If changesTxt.Count < 2 Then
        '    message = "No changes found"
        'End If
        MsgBox(message)
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        TabControl2.TabPages.Add(TabControl3.SelectedTab)
    End Sub

    Private Sub MoveActiveTabToRightControlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveActiveTabToRightControlToolStripMenuItem.Click
        TabControl3.TabPages.Add(TabControl2.SelectedTab)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If SplitContainer3.Panel2Collapsed = True Then
            SplitContainer3.Panel2Collapsed = False
            My.Settings.SplitContainer = False
        Else
            SplitContainer3.Panel2Collapsed = True
            My.Settings.SplitContainer = True
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim path, version As String
        Dim file As IEdmFile5
        If LViewCollect.SelectedItems.Count > 0 Then
            path = LViewCollect.SelectedItems.Item(0).SubItems.Item(9).Text
            version = LViewCollect.SelectedItems.Item(0).SubItems.Item(3).Text.Split("/")(0)
        Else
            If LViewSResults.SelectedItems.Count > 0 Then
                path = LViewSResults.SelectedItems.Item(0).SubItems.Item(5).Text
                version = LViewSResults.SelectedItems.Item(0).SubItems.Item(2).Text
            End If
        End If
        file = vault.GetFileFromPath(path)
        Dim verEnum As IEdmEnumeratorVersion5
        verEnum = file
        Dim ver As IEdmVersion5
        ver = verEnum.GetVersion(version)
        ver.GetFileCopy(Me.Handle)

        showPreview(path, version)
    End Sub

    Private Sub AutoUpdatePreviewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoUpdatePreviewToolStripMenuItem.Click
        My.Settings.UpdatePreview = AutoUpdatePreviewToolStripMenuItem.Checked
    End Sub

    Private Sub showaboutbox()
        AboutBox1.Show()
        AboutBox1.TextBoxDescription.Text =
                "V0.11.9 Added 'Add to zip' for saving." & vbCrLf & _
                "V0.11.8 Added storing tabpages history" & vbCrLf & _
                "V0.11.7 Added preview thumbnail, changeable panels in tabcontrols" & vbCrLf & _
                "V0.11.6 Minor menu Text changes" & vbCrLf & _
                "V0.11.5 Minor fixes" & vbCrLf & _
                "V0.11.4 Changed menus" & vbCrLf & _
                "V0.11.3 Minor bug fixes" & vbCrLf & _
                "V0.11.2 Added checkbox to show only revision comments" & vbCrLf & _
                "V0.11.1 Fixed changes txt" & vbCrLf & _
                "V0.11.0 Added collect only changed drawings, added changes txt" & vbCrLf & _
                "V0.10.5 Added Revisions in comments" & vbCrLf & _
                "V0.10.4 Added contains tab, fixed '!!' comments display" & vbCrLf & _
                "V0.10.3 Users not in Engineering - Staff will see only comments starting with '!!', added menu to hide blank comments" & vbCrLf & _
                "V0.10.2 Design updates, fixed changed date check" & vbCrLf & _
                "V0.10.1 Design updates" & vbCrLf & _
                "V0.10.0 Fixed comments bug. Design updates" & vbCrLf & _
                "V0.9.0 Design updates, abandoned treelistview. Function to check last edited date. Comments from history. Added version to getVariable." & vbCrLf & _
                "V0.8.9 Collect files function coded from scratch." & vbCrLf & _
                "V0.8.8 Added menu item to show only manufacturing data documents, added help menu" & vbCrLf & _
                "V0.8.7 Fixed bug with duplicate files error in collect, Added menu item to show only latest revisioned versions in search results" & vbCrLf & _
                "V0.8.6 Show .stp, .stl, .step, .sat, files in tree" & vbCrLf & _
                "V0.8.5 Adden menuitem show only drawings 'In sync with ERP'" & vbCrLf & _
                "V0.8.3 Added click to search parents, click to open in tree" & vbCrLf & _
                "V0.8.2 Added 'Used In' list, Fixed colouring in tree" & vbCrLf & _
                "V0.8.1 Inserted Menu" & vbCrLf & _
                "V0.7.7 Recieve Command Line parameters" & vbCrLf & _
                "V0.7.6 Minor TreeList changes" & vbCrLf & _
                "V0.7.4 Added TreeList View" & vbCrLf & _
                "V0.7.3 Minor ui changes" & vbCrLf & _
                "V0.7.2 Minor ui changes" & vbCrLf & _
                "V0.7.1 Fixed treeview" & vbCrLf & _
                "V0.7 Changed way to display results" & vbCrLf & _
                "V0.6 When partno typed revision is autocopmleted." & vbCrLf & vbCrLf & _
                "V0.5 Corrected bug when folder exists." & vbCrLf & _
                "V0.4 Changed Name to MDC (Manufacturing Data Collector)" & vbCrLf & _
                "V0.3 Search with enter key." & vbCrLf & _
                "V0.2 Added PartNo, Issue, DescriptionEN in search results list. Fixed Tab Sequence. Update before app starts." & vbCrLf & _
                "V0.1 Added autologin, Autoresize listview with window" & vbCrLf & _
                "V0.0 First release"
    End Sub
End Class

Public Class Comments
    Public Property version As Integer
    Public Property comment As String
End Class
