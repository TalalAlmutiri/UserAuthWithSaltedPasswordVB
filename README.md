# UserAuthWithSaltedPasswordVB
A web application using ASP.net and VB.net to implement salt hashing for protecting user passwords on user registration and login
This app work to add a salt to a user password then hashing the salt with the password as one input. Finally, the hashcode encrypted using AES Advanced Encryption Standard before storing to the database

A cryptographic salt is made up of random bits added to each password instance before its hashing. It works to force the passwords uniqueness, increase their complexity without increasing user requirements, and to mitigate password attacks like rainbow tables

ref: https://auth0.com/blog/adding-salt-to-hashing-a-better-way-to-store-passwords/

![Salted](https://user-images.githubusercontent.com/62042702/89741807-15e0dc00-da9d-11ea-94cb-e75daae08b5f.png)

لمزيد م المعلومات
https://3alam.pro/talal-almutairi/articles/user-protection-with-salted-password-hashing


Database script

    CREATE TABLE [dbo].[UsersInfo](
      [Username] [varchar](20) NOT NULL,
      [HashedPwd] [varchar](200) NULL,
      [Salt] [varchar](100) NULL,
     CONSTRAINT [PK_UsersInfo] PRIMARY KEY CLUSTERED 
    (
      [Username] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    GO


CryptographyHelper.vb

    Public Class CryptographyHelper
    Private Shared Function ByteToHexString(ByVal Data As Byte()) As String
        Dim sBuilder As StringBuilder = New StringBuilder()

        Try
            Dim i As Integer

            For i = 0 To Data.Length - 1
                sBuilder.Append(Data(i).ToString("x2"))
            Next

        Catch ex As Exception
            EventsLogger.WriteLog(ex.ToString())
            Return ""
        End Try

        Return sBuilder.ToString()
    End Function

    Public Shared Function CreateSHAHashWithSalt(ByVal Password As String, ByVal Salt As String) As String
        Try
            Dim sha512 As SHA512 = New SHA512CryptoServiceProvider()
            Dim hash As Byte() = sha512.ComputeHash(Encoding.UTF8.GetBytes(Password & Salt))
            Return ByteToHexString(hash)
        Catch ex As Exception
            EventsLogger.WriteLog(ex.ToString())
            Return ""
        End Try
    End Function

    Public Shared Function GenerateRandomSalt(ByVal KeyLength As Integer) As String
        Try
            Dim data As Byte() = New Byte(KeyLength - 1) {}

            Using rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
                rng.GetBytes(data)
            End Using

            'The length of 32 chars will be 44 according to base64 string.
            'Base64 formula to calculate output = CEILING.MATH(4*n/3)
            'for 32 chars: CEILING.MATH(4 * 32 / 3) = 43
            'if Len(output) % 4 != 0 then the base64 will add (padding) '=' until equals Len(output) % 4 = 0
            Return Convert.ToBase64String(data)
        Catch ex As Exception
            EventsLogger.WriteLog(ex.ToString())
            Return ""
        End Try
    End Function

    Private Shared ReadOnly keyStr As String = ConfigurationManager.AppSettings("Key")
    Private Shared ReadOnly ivStr As String = ConfigurationManager.AppSettings("IV")

    'AES Advanced Encryption Standard
    Public Shared Function Encrypt(ByVal plainText As String) As String
        Dim key As Byte() = ASCIIEncoding.ASCII.GetBytes(keyStr)
        Dim iV As Byte() = ASCIIEncoding.ASCII.GetBytes(ivStr)
        Dim cipher As Byte()
        'Create a new RijndaelManaged.
        Using aes As RijndaelManaged = New RijndaelManaged()
            aes.Key = key
            aes.IV = iV
            Dim encryptorTran As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)

            Using mStream As MemoryStream = New MemoryStream()

                'Creating a stream that links data streams to cryptographic transformations.
                Using crystm As CryptoStream = New CryptoStream(mStream, encryptorTran, CryptoStreamMode.Write)

                    Using sw As StreamWriter = New StreamWriter(crystm)
                        sw.Write(plainText)
                    End Using

                    cipher = mStream.ToArray()
                End Using
            End Using
        End Using

        Return (Convert.ToBase64String(cipher))
    End Function

    Friend Shared Function Decrypt(ByVal cipherText As String) As String
        Dim key As Byte() = ASCIIEncoding.ASCII.GetBytes(keyStr)
        Dim iV As Byte() = ASCIIEncoding.ASCII.GetBytes(ivStr)
        Dim plaintext As String = Nothing

        Using aes As RijndaelManaged = New RijndaelManaged()
            aes.Key = key
            aes.IV = iV
            Dim encryptorTran As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)
            Dim cipher As Byte() = Convert.FromBase64String(cipherText)

            Using mStream As MemoryStream = New MemoryStream(cipher)

                Using crystm As CryptoStream = New CryptoStream(mStream, encryptorTran, CryptoStreamMode.Read)

                    Using reader As StreamReader = New StreamReader(crystm)
                        plaintext = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using

        Return plaintext
    End Function

    End Class

    
 
 Registration Form
 
 ![Reg](https://user-images.githubusercontent.com/62042702/89742040-082c5600-da9f-11ea-999a-dab995d2a854.png)
 
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
          
  Login Form
  
  ![Log](https://user-images.githubusercontent.com/62042702/89742054-27c37e80-da9f-11ea-890e-6a9211b14b09.png)

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

