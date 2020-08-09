Imports System.Data.SqlClient

Public Class Users
    Private user_name As String
    Private user_Pwd As String
    Private user_Salt As String

    Public Property Username As String
        Get
            Return user_name
        End Get
        Set(value As String)
            user_name = value
        End Set
    End Property

    Public Property UserPwd As String
        Get
            Return user_Pwd
        End Get
        Set(value As String)
            user_Pwd = value
        End Set
    End Property

    Public Property UserSalt As String
        Get
            Return user_Salt
        End Get
        Set(value As String)
            user_Salt = value
        End Set
    End Property

    Public Shared Function GetUserInfo(ByVal InputUsername As String) As Users
        Try
            Dim sql As String = "SELECT * FROM UsersInfo WHERE Username=@Username"
            Dim parameters As SqlParameter() = New SqlParameter() {
                New SqlParameter("@Username", InputUsername)
            }
            Dim table As DataTable = DbHelper.ExecuteQuery(sql, CommandType.Text, parameters)

            If table.Rows.Count <= 0 Then
                Return Nothing
            Else
                Dim users As Users = New Users()
                users.Username = table.Rows(0)("Username").ToString()
                users.UserPwd = table.Rows(0)("HashedPwd").ToString()
                users.UserSalt = table.Rows(0)("Salt").ToString()
                Return users
            End If

        Catch ex As Exception
            EventsLogger.WriteLog(ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Shared Function Insert(ByVal Username As String, ByVal Pwd As String, ByVal Salt As String) As Boolean
        Try
            'Inserting a new user to a database
            Dim sql As String = "INSERT INTO UsersInfo VALUES(@Username,@Pwd,@Salt)"
            Dim parameters As SqlParameter() = New SqlParameter() {
                New SqlParameter("@Username", Username),
                New SqlParameter("@Pwd", Pwd),
                New SqlParameter("@Salt", Salt)
            }

            If DbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            EventsLogger.WriteLog(ex.ToString())
            Return False
        End Try
    End Function
End Class
