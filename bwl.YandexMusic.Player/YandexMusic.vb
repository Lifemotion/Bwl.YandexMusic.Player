Imports System.Runtime.InteropServices
Imports MouseKeyboardActivityMonitor.WinApi

Public Class YandexMusic
    Private WithEvents keyhook As New KeyboardHookListener(New GlobalHooker())
    Private Const FEATURE_DISABLE_NAVIGATION_SOUNDS As Integer = 21
    Private Const SET_FEATURE_ON_THREAD As Integer = &H1
    Private Const SET_FEATURE_ON_PROCESS As Integer = &H2
    Private Const SET_FEATURE_IN_REGISTRY As Integer = &H4
    Private Const SET_FEATURE_ON_THREAD_LOCALMACHINE As Integer = &H8
    Private Const SET_FEATURE_ON_THREAD_INTRANET As Integer = &H10
    Private Const SET_FEATURE_ON_THREAD_TRUSTED As Integer = &H20
    Private Const SET_FEATURE_ON_THREAD_INTERNET As Integer = &H40
    Private Const SET_FEATURE_ON_THREAD_RESTRICTED As Integer = &H80
    Private _serv As MService
    <DllImport("urlmon.dll")> _
    <PreserveSig> _
    Private Shared Function CoInternetSetFeatureEnabled(FeatureEntry As Integer, <MarshalAs(UnmanagedType.U4)> dwFlags As Integer, fEnable As Boolean) As <MarshalAs(UnmanagedType.[Error])> Integer
    End Function

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            keyhook.Stop()
        Catch ex As Exception
        End Try
        Try
            keyhook.Dispose()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim feature = FEATURE_DISABLE_NAVIGATION_SOUNDS
        CoInternetSetFeatureEnabled(feature, SET_FEATURE_ON_PROCESS, True)
        WebBrowser1.ScriptErrorsSuppressed = True
        WebBrowser1.Navigate("http://music.yandex.ru/")
        _serv = MService.yandex
        keyhook.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ExecuteCommand(YMCommand.play)
    End Sub

    Private Sub ExecuteCommand(command As YMCommand)
        Select Case _serv
            Case MService.yandex
                Select Case command
                    Case YMCommand.play
                        Js("var b=$("".b-jambox__play .b-jambox__button-i"").trigger(""click"");")
                    Case YMCommand.prev
                        Js("var b=$("".b-jambox__prev .b-jambox__button-i"").trigger(""click"");")
                    Case YMCommand.forward
                        Js("var b=$("".b-jambox__next .b-jambox__button-i"").trigger(""click"");")
                    Case YMCommand.vup

                    Case YMCommand.vdown
                End Select
            Case MService.vk
                Select Case command
                    Case YMCommand.play
                        Js("var b=$("".next.ctrl"").trigger(""click"");")
                    Case YMCommand.prev
                        Js("var b=$("".b-jambox__prev .b-jambox__button-i"").trigger(""click"");")
                    Case YMCommand.forward
                        Js("var b=$("".b-jambox__next .b-jambox__button-i"").trigger(""click"");")
                    Case YMCommand.vup

                    Case YMCommand.vdown
                End Select
        End Select
    End Sub

    Private Sub Js(script)
        Try
            WebBrowser1.Navigate("javascript: " + script)
        Catch ex As Exception

        End Try
    End Sub

    Private Enum YMCommand
        play
        forward
        prev
        vup
        vdown
    End Enum
    Private Enum MService
        yandex
        vk
    End Enum
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ExecuteCommand(YMCommand.prev)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ExecuteCommand(YMCommand.forward)
    End Sub

    Private Sub keyhook_KeyDown(sender As Object, e As KeyEventArgs) Handles keyhook.KeyDown
        If (e.KeyCode = Keys.MediaPlayPause) Then ExecuteCommand(YMCommand.play)
        If (e.KeyCode = Keys.MediaNextTrack) Then ExecuteCommand(YMCommand.forward)
        If (e.KeyCode = Keys.MediaPreviousTrack) Then ExecuteCommand(YMCommand.prev)
    End Sub

    Private Sub scripts_Tick(sender As Object, e As EventArgs) Handles scripts.Tick
        If _serv = MService.yandex Then
            Js("javascript: var b=document.title=$("".js-player-artist"").html()+ "" - "" + $("".js-player-title"").html();")
        End If
        Dim title = WebBrowser1.DocumentTitle
        If InStr(title, "молчит") = 0 And InStr(title, "Яндекс") = 0 Then
            Me.Text = title
        End If
        ' Js("var b=$("".b-jambox__next .b-jambox__button-i"").trigger(""click"");")
        '    Dim b = WebBrowser1.DocumentTitle
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        scripts.Stop()
        WebBrowser1.Navigate("http://music.yandex.ru/")
        _serv = MService.yandex
        scripts.Start()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        scripts.Stop()
        WebBrowser1.Navigate("http://vk.com/audio")
        _serv = MService.vk
        scripts.Start()
    End Sub

    Private Sub keyhook_KeyPress(sender As Object, e As KeyPressEventArgs) Handles keyhook.KeyPress

    End Sub
End Class
