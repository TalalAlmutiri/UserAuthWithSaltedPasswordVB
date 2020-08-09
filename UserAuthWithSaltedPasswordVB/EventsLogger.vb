Public Class EventsLogger
    Public Shared Sub WriteLog(ByVal text As String)
        Try

            Using log As EventLog = New EventLog("Application")
                log.Source = "Application"
                log.WriteEntry(text, EventLogEntryType.[Error], 234, CShort(3))
            End Using

        Catch
            Throw
        End Try
    End Sub
End Class
