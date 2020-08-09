Imports System.Data.SqlClient

Public Class DbHelper
    'You need to change connection string from Web.config
    Private Shared ReadOnly connectionString As String = ConfigurationManager.ConnectionStrings("DbConnecion").ConnectionString
    ''' <summary>
    ''' Execute Select query and return results as a DataTable
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns>DataTable</returns>
    Public Shared Function ExecuteQuery(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As DataTable
        Dim table As DataTable = New DataTable()

        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    Dim adapter As SqlDataAdapter = New SqlDataAdapter(cmd)
                    adapter.Fill(table)
                End Using
            End Using

        Catch ex As Exception
            EventsLogger.WriteLog(ex.Message)
            Return Nothing
        End Try

        Return table
    End Function

    ''' <summary>
    ''' Executes a SQL statement and returns the number of rows affected. NonQuery (Insert, update, and delete)
    ''' </summary>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="parameters"></param>
    ''' <returns>Boolean</returns>
    Public Shared Function ExecuteNonQuery(ByVal cmdText As String, ByVal cmdType As CommandType, ByVal parameters As SqlParameter()) As Boolean
        Dim value = 0

        Try

            Using con As SqlConnection = New SqlConnection(connectionString)

                Using cmd As SqlCommand = New SqlCommand(cmdText, con)
                    con.Open()
                    cmd.CommandType = cmdType
                    If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                    value = cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            EventsLogger.WriteLog(ex.Message)
            Return False
        End Try

        If value < 0 Then
            Return False
        Else
            Return True
        End If
    End Function
End Class
