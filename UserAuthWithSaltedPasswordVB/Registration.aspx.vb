Public Class Registration
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        'You can do more validation rules

        'Password must be at least 8 characters, password must contain an uppercase letter, password must contain a number
        Dim pattern As String = "^(?=.{8,20}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$"

        If Not Regex.IsMatch(txtPassword.Value, pattern) Then
            lbMsg.Text = "Invalid password format"
        Else
            'Generate a random salt
            Dim salt As String = CryptographyHelper.GenerateRandomSalt(32)
            'Hashing password and salt
            Dim pwd As String = CryptographyHelper.CreateSHAHashWithSalt(txtPassword.Value, salt)

            ' Encrypt the salt before storing to the database for more security 
            Dim encryptedSalt As String = CryptographyHelper.Encrypt(salt)

            If Users.Insert(txtUsername.Value, pwd, encryptedSalt) Then
                lbMsg.Text = "Inserted successfully"
            Else
                lbMsg.Text = "Error"
            End If
        End If
    End Sub
End Class