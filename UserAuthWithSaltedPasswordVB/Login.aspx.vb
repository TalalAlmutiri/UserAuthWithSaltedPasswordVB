Imports System.Data.SqlClient

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        'Password must be at least 8 characters, password must contain an uppercase letter, password must contain a number
        Dim pattern As String = "^(?=.{8,20}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$"

        If Not Regex.IsMatch(txtPassword.Value, pattern) Then
            lbMsg.Text = "Invalid password format"
        Else
            Dim users As Users = Users.GetUserInfo(txtUsername.Value)

            If users Is Nothing Then
                lbMsg.Text = "Username or password incorrect"
            Else
                ' Decrypt the salt
                Dim decryptedSalt As String = CryptographyHelper.Decrypt(users.UserSalt)
                Dim loginSaltedPwd As String = CryptographyHelper.CreateSHAHashWithSalt(txtPassword.Value, decryptedSalt)

                If String.Compare(loginSaltedPwd.Trim(), users.UserPwd.Trim(), False) = 0 Then
                    lbMsg.Text = "Login successful"
                Else
                    lbMsg.Text = "Username or password incorrect"
                End If
            End If
        End If
    End Sub
End Class