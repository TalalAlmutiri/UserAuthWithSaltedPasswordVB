Imports System.IO
Imports System.Security.Cryptography

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
